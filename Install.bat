set "curpath=%cd%"
python "Extractor\Python Extractor\archive.py" "C:\Program Files (x86)\Origin Games\Battlefield Bad Company 2"
python "Extractor\Python Extractor\dbx.py" "%curpath%/Resources FbRB/"
move "Resources FbRB" "Resources"