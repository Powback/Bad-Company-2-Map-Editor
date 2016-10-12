# General usage of the script: In the file explorer, drag and drop one or several files and/or folders onto the script.
# It will unpack fbrb archives into FbRB folders; and pack FbRB folders back into fbrb archives.
# You can also drag non-fbrb folders onto the script and specify whether you want to unpack or pack all fbrb folders/archives within.
# There are some options in this script:
#   -Use a temporary file on the hard drive if the script fails at packing.
#   -Specify the gzip compression level when packing.
#   -Specify a location to unpack files to. By default, FbRB folders are placed in the same directory as the corresponding fbrb archives.
# Note: The script will only pack files with known extensions; e.g. XML files in a FbRB folder will be ignored.
#       Clutter in FbRB folders is therefore not an issue.


compressionLevel=6  # Takes values between 0 and 9, 0=unzipped, 1=weak compression, 9=strong compression;
                    # The vanilla files are either 0 or 6, with some variance (probably to fit on one disk).

useTmpFile=0        # Make a temporary file on the hard drive for packing in case of memory issues.
                    # A 4 GB system could handle all files without, except for streaming_sounds-00.fbrb.
                    # No significant change in performance when packing mp_common level-00.


# If you want a dump of all files and do not care about re-packaging those files, change the unpack folder to an absolute path.
# All fbrb archives are then extracted into the same folder.
unpackFolder="../../Resources"
#unpackfolder=r"D:\hexing\bc2 full dump"



######################################################

# fbrb format:
#   Big endian.
#
#   Header:
#       4 bytes file magic: FbRB
#       4 bytes int: Size of (compressed) metadata section.
#   The remaining file consists of two sections: Metadata and payload.
#   Metadata is always gzipped. Payload may be uncompressed or gzipped depending on a metadata flag.
#   Unzip the metadata before continuing. If the payload is zipped (check the meta), unzip it as well. The meta only refers to the uncompressed payload.
#
#   Metadata format:
#       Header:
#           4 bytes (unknown purpose): 00000002
#           4 bytes int: Size of string section.
#
#       String section. Contains file paths and extensions.
#
#       4 bytes int: Number of files.
#       File metadata entries (one entry per file):
#           6 ints with 4 bytes each. => 24 bytes per entry.
#
#           1) Relative file path offset (in the string section).
#           2) Not-deleted flag. Either 00000000 (deleted) or 00010000 (not deleted):
#               deleted => file size 0. The reverse implication is not always true.
#           3) Absolute payload offset in the uncompressed payload section.
#           4) Payload size in the uncompressed payload section.
#           5) Alternative payload size. Same value as 4) except for one case in Package, for reasons unknown.
#           6) Relative filetype offset (in the string section):
#               (The files themselves already have one of these 4 extensions: bin, res, dbmanifest, dbx.)
#               Case1: If the not-deleted flag is 0, the filetype is "*deleted*" (and the file is empty).
#               Case2: Otherwise, if the file extension is not res, the filetype is "<non-resource>".
#               Case3: Otherwise, (not-deleted and res extension), there are many different filetypes possible (including "<non-resource>").
#
#               To make the unpacking lossless, I do the following:
#                   In case1 I combine the extension and filetype.
#                   In case2 I omit the filetype and just keep the extension.
#                   In case3 I omit the extension and just keep the filetype (but turn "<non-resource>" into "nonres").
#               When packing, do the reverse.
#               
#       1 byte bool: 1 if the payload is gzip compressed, 0 if the payload is not compressed.
#       4 bytes int: Total size of uncompressed payload section.



######################################################
######################################################
######################################################
BUFFSIZE=1000000 # Speeds up writing to the fbrb archive.

from struct import pack,unpack
import gzip
from cStringIO import StringIO
import sys, os
import tempfile


def lp(path): # Fixes issues with long paths.
    if len(path)<=247 or path[:4]=='\\\\?\\': return path
    return unicode('\\\\?\\' + os.path.normpath(path))

def MakeLongDirs(path):
    """Create folders even with long path names."""
    path=os.path.normpath(path)
    # Create one folder level manully because makedirs might fail.
    pathParts=path.split("\\")
    manualPart="\\".join(pathParts[:2])
    if not os.path.isdir(manualPart): os.makedirs(manualPart)
    
    # Now handle the rest, including extra long path names.
    folderPath=lp(os.path.dirname(path))
    if not os.path.isdir(folderPath): os.makedirs(folderPath)
    
def open2(path,mode="rb"):
    """When writing to files, create folders if necessary."""
    if mode=="wb": MakeLongDirs(path)
    return open(lp(path),mode)

def ReadNullTerminatedString(f, offset=None):
    if offset!=None: f.seek(offset)
    rv=""
    while 1:
        char = f.read(1)
        if char=="\0": return rv
        rv+=char
def PrintPath(path):
    if path[:4]=='\\\\?\\': print path[:4]
    else: print path

# FbRB folder extensions vs fbrb archive filetype.
validExtensions=dict(swfmovie='SwfMovie',dx10pixelshader='Dx10PixelShader',havokphysicsdata='HavokPhysicsData',
treemeshset='TreeMeshSet',terrainheightfield='TerrainHeightfield',itexture='ITexture',animtreeinfo='AnimTreeInfo',
irradiancevolume='IrradianceVolume',visualterrain='VisualTerrain',skinnedmeshset='SkinnedMeshSet',
dx10vertexshader='Dx10VertexShader',aimanimation='AimAnimation',occludermesh='OccluderMesh',
dx9shaderdatabase='Dx9ShaderDatabase',wave='Wave',sootmesh='SootMesh',terrainmaterialmap='TerrainMaterialMap',
rigidmeshset='RigidMeshSet',compositemeshset='CompositeMeshSet',watermesh='WaterMesh',visualwater='VisualWater',
dx9vertexshader='Dx9VertexShader',dx9pixelshader='Dx9PixelShader',dx11shaderdatabase='Dx11ShaderDatabase',
dx11pixelshader='Dx11PixelShader',grannymodel='GrannyModel',ragdollresource='RagdollResource',
grannyanimation='GrannyAnimation',weathersystem='WeatherSystem',dx11vertexshader='Dx11VertexShader',terrain='Terrain',
impulseresponse='ImpulseResponse',binkmemory='BinkMemory',deltaanimation='DeltaAnimation',
dx10shaderdatabase='Dx10ShaderDatabase',meshdata='MeshData',xenonpixelshader='XenonPixelShader',
xenonvertexshader='XenonVertexShader',xenontexture='XenonTexture',pathdatadefinition='PathDataDefinition',
nonres='<non-resource>',dbx='<non-resource>',dbxdeleted='*deleted*',resdeleted='*deleted*',bin='<non-resource>',
dbmanifest='<non-resource>')


def Pack(folderPath, compressionLevel=compressionLevel, useTmpFile=useTmpFile):
    """Takes absolute folder path with folder ending on " FbRB" and creates an fbrb archive in the same directory."""
    
    folderPath=lp(folderPath)
    if not os.path.isdir(folderPath) or folderPath[-5:]!=" FbRB": return
    PrintPath(folderPath) ###################
    
    topLevelLength=len(folderPath)+1 # For the RELATIVE pathnames to put in the FbRB.

    strings="" # The list of strings at the beginning of the metadata. To be filled out.
    filetypeDic=dict() # filetype vs offset. Keep track of all filetypes to omit string duplicates in the metadata.
    entries="" # 24 bytes each, 6 ints. To be filled out.
    fileCount=0
    payloadOffset=0 # Where the uncompressed payload starts; sum of all filelengths so far.
    
    if useTmpFile: payload=tempfile.TemporaryFile()
    else: payload=StringIO()
    if compressionLevel:
        payloadHandle=gzip.GzipFile(fileobj=payload,mode="wb",compresslevel=compressionLevel,filename="")
    else:
        payloadHandle=payloadStream


    for dir0, dirs, fnames in os.walk(folderPath):
        for fname in fnames:
            # Validate file.
            rawFilename,extension = os.path.splitext(fname)
            extension=extension[1:].lower()
            if extension not in validExtensions: continue
            fileCount+=1

            # Determine the filename (including relative path) to use inside the archive.
            # Restore filename extensions to res, dbx, bin, dbmanifest.
            relativeDir = dir0.replace("\\","/")[topLevelLength:]
            if relativeDir: relativeDir+="/"
            if extension=="dbxdeleted": relativePath = relativeDir+fname[:-7]
            elif extension in ("dbx","bin","dbmanifest"): relativePath = relativeDir+fname
            else: relativePath = relativeDir+rawFilename+".res"
            relativePath=str(relativePath) # Fix unicode issues.

            # Write path to strings section.
            pathOffset = len(strings) # Current offset in the strings section.
            strings+=relativePath+"\0"

            # Write filetype to strings section.
            # Check if the filetype has been used before. If so, refer to the string already in use.
            filetype = validExtensions[extension]
            if filetype in filetypeDic:
                filetypeOffset=filetypeDic[filetype]
            else:
                filetypeOffset=len(strings)
                filetypeDic[filetype]=filetypeOffset
                strings+=filetype+"\x00"

            undeleteFlag = 0 if extension.endswith("deleted") else 0x10000

            # Write to payload section and retrieve payload size along the way.
            f=open(dir0+"\\"+fname,"rb")
            payloadHandle.write(f.read())
            payloadSize = f.tell()
            f.close()
            
            # Add the meta entry.
            entries+=pack(">6I", pathOffset, undeleteFlag, payloadOffset, payloadSize, payloadSize, filetypeOffset)
            payloadOffset+=payloadSize
            
    if compressionLevel:
        zippedFlag="\x01"
        payloadHandle.close() # The payload stream itself now contains all the data so the additional handle is not needed anymore.
    else: 
        zippedFlag="\x00"

    # Make decompressed metadata, then compress it and store it as *metadata* variable.
    metaStream=StringIO()
    zippedMetaHandle=gzip.GzipFile(fileobj=metaStream,mode="wb",compresslevel=compressionLevel or 1)
    zippedMetaHandle.write("\x00\x00\x00\x02"+pack(">I",len(strings))+strings+pack(">I",fileCount)+entries+zippedFlag+pack(">I",payloadOffset))
    zippedMetaHandle.close()
    metadata=metaStream.getvalue()
    metaStream.close()
    
    # Create the fbrb archive.
    out=open(folderPath[:-5]+".fbrb","wb")
    payload.seek(0)
    out.write("FbRB"+pack(">I",len(metadata)))
    out.write(metadata)
    while 1:
        buff = payload.read(BUFFSIZE)
        if buff: out.write(buff)
        else: break
    out.close()
    payload.close()


class FileMeta:
    def __init__(self, values):
        self.pathOffset = values[0]+8 # Absolute offset due to adding 8.
        self.undelete = values[1]
        self.payloadOffset = values[2]
        self.payloadSize = values[3]
        self.altPayloadSize = values[4]
        self.filetypeOffset = values[5]+8

def Unpack(archivePath, targetFolder=unpackFolder):
    """Takes absolute file path of an fbrb archive and extracts all contents into the target folder, with an additional " FbRB" suffix.
    If targetFolder is an empty string, extract in the directory of the original files."""
    
    archivePath=lp(archivePath)
    # Check that archivePath leads to a valid FbRB archive.
    if archivePath[-5:].lower()!=".fbrb": return
    f=open(archivePath,"rb")
    if f.read(4)!="FbRB":
        f.close()
        return
    PrintPath(archivePath) ###################

    # Determine the target folder for later on.
    if targetFolder: targetFolder = os.path.normpath(lp(targetFolder)+" FbRB") # Use the given unpackFolder.
    else:            targetFolder = os.path.normpath(archivePath[:-5]+" FbRB") # Create folder in the same directory as archive.
    MakeLongDirs(targetFolder) # Create something here in case the fbrb is completely empty.

    # Read the archive. An fbrb archive contains two gzipped files (metadata and payload).
    compressedMetaSize = unpack(">I",f.read(4))[0]
    zippedMetadata, zippedPayload = StringIO(f.read(compressedMetaSize)), StringIO(f.read())
    f.close()

    # Unzip meta in memory and make it accessible as a stream.
    metadataHandle = gzip.GzipFile(mode='rb', fileobj=zippedMetadata)
    metadata = StringIO(metadataHandle.read())
    metadataHandle.close()

    # Parse metadata.
    unknown2, stringSize=unpack(">II",metadata.read(8))
    assert unknown2==2
    metadata.seek(stringSize,1)
    fileCount=unpack(">I",metadata.read(4))[0]
    entries = [FileMeta(unpack(">6I", metadata.read(24))) for fileNumber in xrange(fileCount)]
    zipped = metadata.read(1)=="\x01"
    totalPayloadSize = unpack(">I", metadata.read(4))[0]
    
    # Make payload accessible as stream.
    if zipped:
        payloadHandle = gzip.GzipFile(mode='rb', fileobj=zippedPayload)
        payload = StringIO(payloadHandle.read())
        payloadHandle.close()
    else:
        payload = zippedPayload


    # Go through the file meta and write the files to the target folder.
    for entry in entries:
        path = ReadNullTerminatedString(metadata, entry.pathOffset)
        _, extension = os.path.splitext(path) # Original file extensions: bin, res, dbmanifest, dbx
        filetype = ReadNullTerminatedString(metadata, entry.filetypeOffset).lower() # I prefer lowercase file extensions.

        # Incorporate the file type into the file extension to make the unpacking lossless.
        if filetype=="*deleted*": path += "deleted"
        elif filetype=="<non-resource>":
            if extension==".res": path=path[:-len(extension)]+".nonres"
            else: pass # Keep the original file extension.
        else: path=path[:-len(extension)]+"."+filetype

        # Write to file.
        out=open2(os.path.join(targetFolder,os.path.normpath(path)),"wb")
        payload.seek(entry.payloadOffset)
        out.write(payload.read(entry.payloadSize))
        out.close()



def main():
    # Features:
    #   1) Dump contents of FbRB archive.
    #   2) Pack FbRB folder into archive.
    #   3) Apply 1 or 2 on several items at once.

    inputPaths = [lp(p) for p in sys.argv[1:]]
    os.chdir(os.path.split(sys.argv[0])[0]) # Fixes permission issues.

    mode=""
    for path in inputPaths:
        if os.path.isdir(path) and path[-5:]==" FbRB": Pack(path)
        elif os.path.isfile(path): Unpack(path)
        else:
            # User has specified a non-FbRB folder. Handle all fbrb within this folder according to user input.
            if not mode: mode=raw_input("(u)npack or (p)ack everything from selected folder(s)?\r\n")
            if mode.lower()=="u":
                for dir0,dirs,filenames in os.walk(path):
                    for filename in filenames:
                        Unpack(dir0+"\\"+filename)
            elif mode.lower()=="p":
                for dir0,dirs,files in os.walk(path):
                    Pack(dir0)


try:
    main()
except Exception, e:
    raw_input(e)
