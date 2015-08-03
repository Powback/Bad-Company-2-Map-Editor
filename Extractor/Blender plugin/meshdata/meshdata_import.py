#!/usr/bin/python
## sand2710 - 2015

import time

import sys
import struct
import os
#import binascii
import random

# blender only
import bpy

# def enum(**enums):
#     return type('Enum', (), enums)

# meshType = enum(RIGID=0, SKINNED=1, COMPOSITE=2, TREE=4)

# so far all objects are 3 on pc
# primitiveType = enum(TRIANGLELIST=3)

def H2F(H):
    F = int((H & 0x8000) << 16) | int(((H & 0x7fff) << 13) + 0x38000000)
    return struct.unpack("f", struct.pack("I", F))[0]

def U82F(B):
    return 1.0 / 255 * B

# # too slow
# def H2F(float16):
#     s = int((float16 >> 15) & 0x00000001)    # sign
#     e = int((float16 >> 10) & 0x0000001f)    # exponent
#     f = int(float16 & 0x000003ff)            # fraction
    
#     if e == 0:
#         if f == 0:
#             return int(s << 31)
#         else:
#             while not (f & 0x00000400):
#                 f = f << 1
#                 e -= 1
#                 e += 1
#                 f &= ~0x00000400
#         #print(s,e,f)
#     elif e == 31:
#         if f == 0:
#             return int((s << 31) | 0x7f800000)
#         else:
#             return int((s << 31) | 0x7f800000 | (f << 13))
        
#         e = e + (127 -15)
#         f = f << 13
#         return int((s << 31) | (e << 23) | f)
    
class Subset:

    def __init__(self, fh, offset):
        self.useFloat = False
        self.uvOffset = 0
        self.boneWeightsOffset = 0
        
        self.size = 0
        self.offset = -1
        self.name = ""
        self.primitiveCount = -1
        self.indexOffset = -1
        self.vertexOffset = -1
        self.vertexCount = -1
        self.vertexStride = -1
        self.primitiveType = -1
        self.bonesPerVertex = -1
        self.boneCount = -1
        # u0 streaming enabled?
        self.u0 = -1
        self.bones = []

        self.fh = fh
        
        self.offset = offset
        print("   file offset: " + str(self.offset))
        byte = fh.read(1).decode('ascii')
        self.size += 1
        while ord(byte) != 0:
            self.name += str(byte)
            byte = fh.read(1).decode('ascii')
            self.size += 1
        print("   name: " + self.name)
        header = fh.read(22)
        self.size += 22
        self.primitiveCount, self.vertexCount, self.indexOffset, \
            self.vertexOffset, self.vertexStride, self.primitiveType, \
            self.bonesPerVertex, self.boneCount, self.u0 \
            = struct.unpack("<I I I I B B B B H", header)
        print("   primitive count: "+str(self.primitiveCount))
        print("   index offset: "+str(self.indexOffset))
        print("   vertex offset: "+str(self.vertexOffset))
        print("   vertex count: "+str(self.vertexCount))
        print("   vertex stride: "+str(self.vertexStride))
        print("   primitive type: "+str(self.primitiveType))
        print("   bones per vertex: "+str(self.bonesPerVertex))
        # bone count and bone count 2 have always same value
        # the first is a byte, probable the subset header end here
        # bone count 2 is a short
        print("   bones count: "+str(self.boneCount))
        print("   bones count 2?: "+str(self.u0))
        for i in range(0, self.boneCount):
            boneId = struct.unpack("<H", fh.read(2))
            self.size += 2
            self.bones.append(boneId[0])
        print("   bones: "+str(self.bones))

    def getV3(self,offset):
        ve = []
        if self.vertexStride < offset:
            return ve
        self.fh.seek(self.vxOffset, 0)
        for i in range(0, self.vertexCount):
            if self.useFloat:
                ix, iy, iz = struct.unpack_from("<f f f", self.fh.read(self.vertexStride), offset)
                ve.append(((ix), -(iz), (iy)))
            else:
                ix, iy, iz = struct.unpack_from("<H H H", self.fh.read(self.vertexStride), offset)
                # ve.append((H2F(ix), H2F(iy), H2F(iz)))
                # x, -z, y (zy swap and negative z are adjustment for blender)
                ve.append((H2F(ix), -H2F(iz), H2F(iy)))
        return ve

    # def getWeightsOLD(self, offset):
    #     # { boneId0: { weight0: [ vertexId0, ... vxN  ], ... wN }, ... bN } 
    #     w = {}
        
    #     self.fh.seek(self.vxOffset, 0)
    #     for vxId in range(0, self.vertexCount):
    #         vxdata = self.fh.read(self.vertexStride)
    #         for bone_i in range(0, self.boneCount):
    #             boneId = self.bones[bone_i]
    #             weight = struct.unpack_from("B", vxdata, offset + bone_i)[0]
    #             if boneId in w:
    #                 if weight in w[boneId]:
    #                     w[boneId][weight].append(vxId) # bone and weight exist, append new vertex
    #                 else:
    #                     if type(w[boneId]) is dict:
    #                         w[boneId][weight] = [vxId] # there's already some weight create a new vx list
    #                     else:
    #                         w[boneId] = {weight:[vxId]} # there were no weights for this boneid, add one
    #             else:
    #                 w[boneId] = {weight:[vxId]} # there's was not boneId, create it
    #     return w

    def saveWeight(self, w, vxId, boneId, weight):
        if boneId in w:
            if weight in w[boneId]:
                w[boneId][weight].append(vxId) # bone and weight exist, append new vertex
            else:
                if type(w[boneId]) is dict:
                    w[boneId][weight] = [vxId] # there's already some weight create a new vx list
                else:
                    w[boneId] = {weight:[vxId]} # there were no weights for this boneid, add one
        else:
            w[boneId] = {weight:[vxId]} # there's was not boneId, create it
        
    def getWeights(self, iOffset, wOffset):
        # { boneId0-N(grpId): { weight0-255): [ vertexId0-N, ... vxIdN  ], ... wN }, ... bIdN }
        w = {}
        self.fh.seek(self.vxOffset, 0)
        for vxId in range(0, self.vertexCount):
            vxdata = self.fh.read(self.vertexStride)
            for i in range(0, self.bonesPerVertex):
                bi = struct.unpack_from("b", vxdata, iOffset + i)[0]
                bw = struct.unpack_from("B", vxdata, wOffset + i)[0]
                self.saveWeight(w, vxId, bi, bw)
        return w
    
    def getColors(self, offset):
        c = []
        self.fh.seek(self.vxOffset, 0)
        for vxId in range(0, self.vertexCount):
            vxdata = self.fh.read(self.vertexStride)
            R, G, B, A = struct.unpack_from("<B B B B", vxdata, offset)
            c.append((U82F(R), U82F(G), U82F(B)))
        return c
    
    def getV2(self, offset):
        ve = []
        if self.vertexStride < offset:
            return ve
        self.fh.seek(self.vxOffset, 0)
        for i in range(0, self.vertexCount):
            iu, iv = struct.unpack_from("<H H", self.fh.read(self.vertexStride), offset)
            # v = 1.0 - value (v values negative relative to 1.0)
            ve.append((H2F(iu), 1.0 - H2F(iv)))
        return ve

    def getIndices(self):
        fa = []
        self.fh.seek(self.idxOffset, 0)
        for i in range(0, self.primitiveCount):
            i1, i2, i3 = struct.unpack("<H H H", self.fh.read(6))
            fa.append((i1, i2, i3))
        return fa
    
class Meshdata:

    def getUnknownChunk(self):
        self.unknownChunk = self.fh.read(28)
        self.size += 28

    def getSubsets(self):
        for i in range(0, self.subsetCount):
            print("subset[{0:02d}]:".format(i))
            subset = Subset(self.fh, self.fh.tell())
            self.subset.append(subset)
            self.size += subset.size

    def getSubsetExtraBytes(self):
        for i in range(0, self.subsetCount):
            extrabyte = struct.unpack("<B", self.fh.read(1))[0]
            self.size += 1
            print("subset[{0:02d}].extraByte: {1}".format(i, int(extrabyte)))
            self.subsetExtraByte.append(extrabyte)

    def getHeader(self):
        if self.fh != None:
            self.fh.seek(0, 0)
            header = self.fh.read(7)
            self.size += 7
            self.meshType, self.u0, self.subsetCount \
                = struct.unpack("<I H B", header)
            print("mesh type: "+str(self.meshType))
            print("subset count: "+str(self.subsetCount))

    def getVerticesSize(self):
        maxv = 0
        for i in range(0, self.subsetCount):
            subset = self.subset[i]
            if subset.vertexOffset + subset.vertexCount * subset.vertexStride > maxv:
                maxv = subset.vertexOffset + subset.vertexCount * subset.vertexStride
        return maxv
    
    def getIndicesSize(self):
        maxi = 0
        for i in range(0, self.subsetCount):
            subset = self.subset[i]
            # 3 is the number of vertices per primitive (triangles)
            # 2 is bytes for sizeof(uint16_t)
            if (subset.indexOffset + subset.primitiveCount * 3) * 2 > maxi:
                maxi = (subset.indexOffset + subset.primitiveCount * 3) * 2
        return maxi

    def __exit__(self, type, value, traceback):
        if self.fh != None:
            self.fh.close()

    def adjustVxOffs(self):
        for i in range(0, self.subsetCount):
            subset = self.subset[i]
            subset.vxOffset = self.dataOffset + subset.vertexOffset

    def adjustIdxOffs(self):
        verticesSize = self.getVerticesSize()
        for i in range(0, self.subsetCount):
            subset = self.subset[i]
            # * 2 bytes (sizeof(uint16_t))
            subset.idxOffset = self.dataOffset + verticesSize + (subset.indexOffset * 2)

    def __init__(self, path):
        self.time_start = time.time()

        self.size = 0
        self.meshType = -1
        self.u0 = -1
        self.subsetCount = -1
        self.subset = []
        self.subsetExtraByte = []
        self.unknownChunk = ""

        self.path = path
        self.fh = open(path, "rb")

        self.getHeader()
        self.getSubsets()
        
        # TODO:
        # some file doesn't have extra bytes
        # but didn't found a way to know yet
        # just a workaround, the calculated size
        # of vertices and indices is right
        # set the offset relative to the end of the file
        # vxstart = eof-sizeof(vx+idx)
        self.getSubsetExtraBytes()
        
        self.getUnknownChunk()
        self.dataOffset = self.fh.tell()
        self.size += self.getVerticesSize()
        self.size += self.getIndicesSize()
        print("data offset: "+str(self.dataOffset))
        self.adjustVxOffs()
        self.adjustIdxOffs()
        for i in range(0, self.subsetCount):
            subset = self.subset[i]
            print("subset[{0:02d}].vxOffset:  0x{1:08x}".format(i, subset.vxOffset))
        for i in range(0, self.subsetCount):
            subset = self.subset[i]
            print("subset[{0:02d}].idxOffset: 0x{1:08x}".format(i, subset.idxOffset))
            
    def blenderCreate(self, useFloat, uvOffset, boneWeightsOffset):

        for i in range(0,self. subsetCount):
            subset = self.subset[i]
            subset.useFloat = useFloat
            if "ZOnly" in subset.name:
                continue
            prefix = ""
            coords = subset.getV3(0)
            faces = subset.getIndices()
            me = bpy.data.meshes.new(prefix+subset.name+"_Mesh")

            ob = bpy.data.objects.new(prefix+str(i).zfill(2)+'_'+subset.name, me)
            ob.location = (0, 0, 0)
            
            bpy.context.scene.objects.link(ob)
            me.from_pydata(coords, [], faces)            
            me.update(calc_edges=True)

            # vertex colors
            
            # for xoffset in range(40, 48-4):
            #     rgb = subset.getColors(xoffset)
            #     rgbvxmap = ob.data.vertex_colors.new("Colormap{0}".format(xoffset))
            #     j = 0
            #     for poly in me.polygons:
            #         k = 0
            #         for idx in poly.loop_indices:
            #             rgbvxmap.data[idx].color = rgb[faces[j][k]]
            #             k += 1
            #         j += 1
                    
            # f1=open('/tmp/testfile', 'w+')
            # print("weigths:", file=f1)
            # print(weights, file=f1)
            # f1.close()

            # vertex bones weights

            if boneWeightsOffset > 0:
                weights = subset.getWeights(8, boneWeightsOffset) # 0x12
                for grpId, wv in weights.items():
                    grpName = "Bone_"+str(boneWeightsOffset)+"_"+str(grpId).zfill(3)
                    if not grpName in ob.vertex_groups:
                        ob.vertex_groups.new(grpName)
                        for w, vx  in wv.items():
                            ob.vertex_groups[grpName].add(vx, U82F(w), 'REPLACE')
            
            mat = bpy.data.materials.new(subset.name)
            mat.diffuse_color = (random.uniform(0.0, 1.0), \
                                 random.uniform(0.0, 1.0), \
                                 random.uniform(0.0, 1.0))
            me.materials.append(mat)

            for f in me.polygons:
                f.material_index = 0
                f.use_smooth = 1

            if uvOffset > 0:
                vert_uvs = subset.getV2(uvOffset) # 0x28
                if len(vert_uvs) > 0:
                    me.uv_textures.new("UV"+str(uvOffset))
                    uvlist = [uv for pair in [vert_uvs[l.vertex_index] for l in me.loops] for uv in pair]
                    me.uv_layers[-1].data.foreach_set("uv", uvlist)
                
            # vert_uvs = subset.getV2(0x30)
            # if len(vert_uvs) > 0:
            #     me.uv_textures.new("UV1")
            #     uvlist = [uv for pair in [vert_uvs[l.vertex_index] for l in me.loops] for uv in pair]
            #     me.uv_layers[-1].data.foreach_set("uv", uvlist)

        print("useFloat: {0}".format(useFloat))
        print("Finished in: {:.4f} sec".format(time.time() - self.time_start))

    
def process_meshdata(path):
    print("meshdata file: " + path)
    si = os.stat(path)
    print("file size: "+str(si.st_size))
    meshdata = Meshdata(path)
    print("mesh calculated size: "+str(meshdata.size))
    print("tried to do some shit")
    # for i in range(6,48):
    #meshdata.blenderCreate()

def main(argv):
    for arg in argv:
        process_meshdata(arg)

if __name__ == "__main__":
    random.seed()
    # fileSel = FileSelector()
    # fileSel.invoke(bpy.context, None)
    
    # main(sys.argv[1:])
    # main(["fbrb/Objects/Vehicles/Land/T90/T90_Mesh_lod0_data.meshdata"])
    # main(["fbrb/Objects/Vehicles/Sea/Pbl/Pbl_Mesh_lod0_data.meshdata"])
    # main(["fbrb/Characters/Us/US_Models/US_01_Body_Mesh_lod0_data.meshdata"])
    # main(["fbrb/Characters/Animals/Parrot/Parrot_01_Mesh_lod0_data.meshdata"])
    
