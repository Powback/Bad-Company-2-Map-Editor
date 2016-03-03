# Bad-Company-2-Map-Editor

The Battlefield: Bad Company 2 map editor is a Unity project that tries to replicate the way the Frostbite engine works.
It supports loading and saving of extracted BC2 files with native terrain, model and texture loading.

The files produced can be played online using [Project Rome](http://forums.veniceunleashed.net/bf/index.php) by [Emulator Nexus](http://emulatornexus.com).

It currently does not extract and produce the ready-to-go files that are required. Some slight modification of the produced files are required in order for them to work online. This will be fixed in later updates.

# Goals:
The goal is to replicate the Frostbite 1.5 engine and produce entierly new maps for the game Battlefield: Bad company 2.

# Why Unity?
Unity provides a fast rendering engine with a lot of room for expansion. It loads quickly and is extremely customizable. While it's mostly used for game development, it's an excellent platform for projects such as this one.

# How to install:
\* This process requires [Python 2.7](https://www.python.org/download/releases/2.7/)

The project works mostly out of the box. However, there are a few preperations you have to do.
* Extract the BC2 game files
  * Open Archive.py and change the unpack directory to a location you remember
  * Drag and drop your Battlefield: Bad Company 2 game directory to the archive.py script.
    * This will produce a folder called something like "Battlefield Bad Company 2 FbRB"
  * Drag this directory to the dbx.py script. This will convert all the dbx files to xml for easy editing
  * Move the fbrb directory into the main project location (Bad-Company-2-Map-Editor-Master). It should be next to Assets, Projectsettings and so on
  * Rename the directory to "Resources". (This will not be required in the future)

  The initial setup is now completed. This process will only have to be done once.
  You can now open "Battlefield-Bad-Company-2-Master" in Unity.

# How to use
  You can load maps by setting the level name into the "Level Name" text box located inside the _GM GameObject.
  
  You can find the level names inside Resources/levels.
  
  The level names are the ones that look like "mp_001CQ". You will get a map-preview when the editor finds the selected
 map.
 
  Press play on the top.
  
    The editor will now load the selected map. Be patient while it loads.
    
  Press on the scene tab in order to navigate through the map. Hold the right mouse button in order to change view-direction.
  
  You can select each object by pressing it. To the right you can modify the selected object's properties.
  
  I encurage you to just try yourself forward in order to understand what does what.

# Thanks to:
* Frankelstner - For all his work in the BF modding scene. Also the Unarchiver and converter scripts.
* Emulator Neuxus - For Rome, Venice Unleashed and Rime
* sand2710 - For the original .meshdata importer. 
* Nasipal and Badbaubau - For helping me figure out how Frostbite works.
