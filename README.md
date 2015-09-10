# Bad-Company-2-Map-Editor

This is a Unity 3D project that is in development. 

Currently, it allows basic loading of decompiled dbx files and converted 3d models. You can't export the maps yet.

# Goals
The main goal here is to create a fully functional 3D map editor that allows you to both edit existing maps and create entirely new ones from scratch.

# Features
* Map Data loading
* Model Loading
* Terrain Loading
* HavokEntity Loading


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


# Thanks to:
* Frankelstner - For all his work in the BF modding scene. Also the Unarchiver and converter scripts.
* Emulator Neuxus - For Rome, Venice Unleashed and Rime
* sand2710 - For the .meshdata importer. 
* Badbaubau - For helping me with his BC2 modding experience.
