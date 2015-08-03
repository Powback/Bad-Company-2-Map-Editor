import bpy
import os
import meshdata_import

from bpy.props import (BoolProperty,
                       FloatProperty,
                       StringProperty,
                       EnumProperty,
                       )

from bpy_extras.io_utils import (ImportHelper)

from meshdata.meshdata_import import Meshdata

path = 'D:/frostbite/BC2 Dump FbRB'  # set this path

for dirName, subdirList, fileList in os.walk(path, topdown=False):
    print('Found directory: %s' % dirName)
    for f in fileList:
        if f.endswith('.meshdata'):
            mesh_file= os.path.join(dirName,f)
            obj_file = mesh_file.replace('.meshdata','.obj')

            bpy.ops.object.select_all(action='SELECT')
            bpy.ops.object.delete()

            uv_offset = bpy.props.IntProperty(name="UV offset", default=0)
            bone_weight_offset = bpy.props.IntProperty(name="bone weights offset", default=0)
            use_float = BoolProperty(
                name="use 32 bit float",
                description="enable floats (default is 16 bit half float)",
                default=False,
                )

            
            meshdata = Meshdata(mesh_file)
            meshdata.blenderCreate(0, 0, 0)
            bpy.ops.object.select_all(action='SELECT')
            bpy.ops.export_scene.obj(filepath=obj_file)