# Bad-Company-2-Map-Editor

This is a Unity 3D project that is in development. 

Currently, it allows basic loading of decompiled dbx files and converted 3d models. 

# Goals
The main goal here is to create a fully functional 3D map editor that allows you to both edit existing maps and create entirely new ones from scratch. It will hopefully be possible to port maps in the future.


# Progress
* 70% - Map loading 
  * Correctly imports all data, but it still has to be placed into the correct containers.
  * Will be completed once I bother. It's a boring task. Feel free to help out!
  * Just create a new script in the scripts/type folder. You just need to copy the Instance field and array names in there. 
* 90% - Object Loading 
  * Sometimes places objects akwardly. Might have something to do with scale. Only affects certain items. 
* 100% - Fix rotation 
  * Rotation seems to work fine.
* 0% - Terrain loading 
  * Needs a lot of research. Help would be appreciated.
* 0% - Saving  
  * Will be worked on when the above are finished.

# Planned
* Importing / exporting textures 
  * Needs more research.
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
Method 1 - Automatic.
Download the files located in the File Branch and place them in the assets/resources folder.

Method 2 - Manually.
In order to load the data, you will need to export your BC2 files and convert them to the correct format using the Phyton scripts located in the Export folder. 

Simply drag and drop your BC2 directory into the FBRP export script. Then drag the exported file folder into the DBX converter. 

You should be able to load the maps now. To import the models you will need to install and activate the blender addon located in the Export folder. Just open blender and press ctrl+f11, open file, select the convert script and change the folder to where you exported the data. I recommend that you launch blender from the commandline using "blender.exe -d". The script crash at certain files. Opening blender from the command line will show the console after blender has crashed. Then you can just delete the files that are causing the crash.

Video tutorial coming soon.
