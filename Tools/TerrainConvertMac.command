#!/bin/sh
FILE=$(<"/Users/Powback/Unity/Bad-Company-2-Map-Editor/Tools/Temp/TerrainLocation.txt")
CURRES=$(<"/Users/Powback/Unity/Bad-Company-2-Map-Editor/Tools/Temp/CurRes.txt")
NEWRES=$(<"/Users/Powback/Unity/Bad-Company-2-Map-Editor/Tools/Temp/NewRes.txt")
mkdir -p "/Users/Powback/Unity/Bad-Company-2-Map-Editor/Assets/Resources/_Converted/$FILE"
cp "/Users/Powback/Unity/Bad-Company-2-Map-Editor/Assets/Resources/$FILE.Heightfield-000.terrainheightfield" "/Users/Powback/Unity/Bad-Company-2-Map-Editor/Assets/Resources/_Converted/$FILE.raw"
mogrify -resize $NEWRES -flip -rotate 90 -depth 16 -size $CURRES+49 gray:"/Users/Powback/Unity/Bad-Company-2-Map-Editor/Assets/Resources/_Converted/$FILE.raw"


