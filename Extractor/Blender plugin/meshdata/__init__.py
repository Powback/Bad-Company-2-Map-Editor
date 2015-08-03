## sand2710 - 2015

bl_info = {
    "name": "BC2 meshdata format",
    "location": "File > Import-Export",
    "version": (0, 1),
    "category": "Import-Export"}

import bpy
from bpy.props import (BoolProperty,
                       FloatProperty,
                       StringProperty,
                       EnumProperty,
                       )

from bpy_extras.io_utils import (ImportHelper)

# if "bpy" in locals():
#     import imp
#     if "Meshdata" in locals():
#         imp.reload(meshdata)

from meshdata.meshdata_import import Meshdata

class ImportMeshdata(bpy.types.Operator, ImportHelper):
    """Import BC2 meshdata"""

    bl_idname = "import.meshdata"
    bl_label = 'Import BC2 meshdata'
    bl_options = {'PRESET', 'UNDO'}

    filename_ext = ".meshdata"
    filter_glob = StringProperty(default="*.meshdata",options={'HIDDEN'},)

    # filepath= StringProperty(name="File Path", description="Filepath
    #     used for importing the yourformatname file", maxlen=1024, default="")

    use_float = BoolProperty(
            name="use 32 bit float",
            description="enable floats (default is 16 bit half float)",
            default=False,
            )

    uv_offset = bpy.props.IntProperty(name="UV offset", default=0)
    bone_weight_offset = bpy.props.IntProperty(name="bone weights offset", default=0)
    
    def execute(self, context):
       meshdata = Meshdata(self.filepath)
       meshdata.blenderCreate(self.use_float, self.uv_offset, self.bone_weight_offset)
       return {'FINISHED'}

def menu_func_import(self, context):
    self.layout.operator(ImportMeshdata.bl_idname, text="BC2 (.meshdata)")

def register():
    bpy.utils.register_module(__name__)
    bpy.types.INFO_MT_file_import.append(menu_func_import)

def unregister():
    bpy.utils.unregister_module(__name__)
    bpy.types.INFO_MT_file_import.remove(menu_func_import)

if __name__ == "__main__":
    register()

