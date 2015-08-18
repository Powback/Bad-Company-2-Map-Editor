# Bad-Company-2-Map-Editor

This is a Unity 3D project that is in development. 

Currently, it allows basic loading of decompiled dbx files and converted 3d models. 

# Goals
The main goal here is to create a fully functional 3D map editor that allows you to both edit existing maps and create entirely new ones from scratch. It will hopefully be possible to port maps in the future.

# Completed
* HavokEntity
  * Correctly imports all HavokEntites at correct locations. Actual physcis is not needed.
* Texture importing
  * All textures should be exported and converted. Some textures seem to be missing, but it's likely that I just can't find the correct one.


# Progress
* 70% - Map loading 
  * Correctly imports all data, but it still has to be placed into the correct containers.
  * Will be completed once I bother. It's a boring task. Feel free to help out!
  * Just create a new script in the scripts/type folder. You just need to copy the Instance field and array names in there. 
* 90% - Object Loading 
  * Sometimes places objects akwardly. Might have something to do with scale. Only affects certain items. 
* 100% - Fix rotation 
  * Rotation seems to work fine.
* 0% - Saving  
  * Will be worked on when the above are finished.
  * 
  
#Need more work
 * Terrain loading
   * Currently loads terrain, but the terrain file has to be loaded in Photoshop with a header of 50, flip horizontal, rotate 90 degrees ccw and saved with a 0 byte header. To load the map in BC2 I think the header has to be re-added. Untested. It's also jittery, not sure if this will result in a jittery BC2 terrain.

# Planned
* Custom textures
* importing lightning 
  * Needs more research.
* importing weather. 
  * Needs more research.
* Basic position/rotation editing
  * More spesifically, a UI interface for it. Allowing the editor to be ran without the Unity Editor.
* Advanced editing
  * Adding and removing objects. 
* Creating maps from scratch.
  * Will need to import the standard map objects (HUD, VO) and place them correctly. More research needed.
 
# Future
* Texture editing
  * Will need to research how frostbite handles object textures and shaders.
* Custom models 
  * Need to convert objects to .meshdata and correctly export all needed data.
* BF3 map loading with Rime
  * Can hopefully allow you to port BC2 maps to BF3 and vice versa.
* Load Source Engine maps
  * Hopefully it will be possible to import Source Engine maps in the future.
  
# Why Unity?
Unity provides a fast rendering engine with a lot of room for expansion. It loads quickly and is extremely customizable. While it's mostly used for game development, it's an excellent platform for projects such as this one.

# How to use
Method 1 - Automatic. Coming soon!
Download the files located in the File Branch and place them in the assets/resources folder.

Method 2 - Manually.
In order to load the data, you will need to export your BC2 files and convert them to the correct format using the Phyton scripts located in the Export folder. 

Edit arcive.py and change the Unpack folder to a different directory. This will save you some time later.
Then simply drag and drop your BC2 directory into the FBRP export script. Then drag the exported file folder into the DBX converter. 

You probably want to open up CMD at that directory and type "del /s /q *.dbx" to save some space. 

You should be able to load the maps now. To import the models you will need to install and activate the blender addon located in the Export folder. Then just open blender and press ctrl+f11, open file, select the convert script and change the folder to where you exported the data. I recommend that you launch blender from the commandline using "blender.exe -d". The script crash at certain files. Opening blender from the command line will show the console after blender has crashed. Then you can just delete the files that are causing the crash. These files are mostly located in Objects/Objects/Buildings/somehouse/MatMask
We don't use these files yet, so they are safe to ignore.

You should then place all these directories in the Resources folder of your project. It should look something like Resources/AI, Resources/Animations, Resources/awards

Video tutorial coming soon.
