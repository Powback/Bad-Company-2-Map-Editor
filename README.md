# Bad-Company-2-Map-Editor

This project is using Unity to load Battlefield: Bad Company 2 gamefiles. You are able modify and save levels.

# Goals
The main goal here is to create a fully functional 3D map editor that allows you to both edit existing maps and create entirely new ones from scratch.

# Why Unity?
Unity provides a fast rendering engine with a lot of room for expansion. It loads quickly and is extremely customizable. While it's mostly used for game development, it's an excellent platform for projects such as this one.

# How to install.
Method 1 - Automatic. Coming soon!
Download the files located in the File Branch and place them in the assets/resources folder.

Method 2 - Manually.

In order to load the data, you will need to export your BC2 files and convert them to the correct format using the Phyton scripts located in the Export folder. 

Edit arcive.py and change the Unpack folder to a different directory. This will save you some time later.
Then simply drag and drop your BC2 directory into the FBRP export script. Then drag the exported root folder into the DBX converter. 

You probably want to open up CMD at that directory and type "del /s /q *.dbx" to save some space. 

You should then place all these directories in the Resources folder of your project. It should look something like Resources/AI, Resources/Animations, Resources/awards

That's pretty much it.

# Thanks to:
* Frankelstner - For all his work in the BF modding scene. Also the Unarchiver and converter scripts.
* Emulator Neuxus - For Rome, Venice Unleashed and Rime
* sand2710 - For the original .meshdata importer. 
* Nasipal and Badbaubau - For helping me figure out how Frostbite works.
