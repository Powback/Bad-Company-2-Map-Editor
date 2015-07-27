# Bad-Company-2-Map-Editor

This is a Unity 3D project that is in development. 

Currently it allows basic loading of decompiled dbx files and converted 3d models. 

# Goals
The main goal here is to create a fully functional 3D map editor that allows you to both edit existing maps and create entierly new ones from scratch. 

Since the Frostbite 2+ map layout is pretty similar, it's not impossible that porting maps can become a reality.

# Progress
* 50% - Map loading 
  * Correctly imports all data, but it still has to be placed into the correct containers.
* 50% - Object Loading 
  * Spawns the correct objects at the correct position but with wrong rotation. 
* 0% - Fix rotation 
  * Working on figuring this out. Frostbite handles rotation very weirdly.
* 0% - Terrain loading 
  * Needs a lot of research. Help would be appreciated.
* 0% - Saving  
  * WIll be worked on when the above are correct.
* 0% - Importing / exporting textures 
  * Needs more research.
* 0% - importing lightning 
  * Needs more research.
* 0% - importing weather. 
  * Needs more research.

# Planned
* Creating maps from scratch.
  * Will need to import the standard map objects (HUD, VO) and place them correctly. More research needed.

# Future
* Texture editing
  * Will need to research how frostbite handles object textures and shaders.
* Custom models 
  * Need to convert objects to .meshdata and correctly export all needed data.
* BF3 map loading with Rime
  * Can hopefully allow you to port BC2 maps to BF3 and vice versa.
