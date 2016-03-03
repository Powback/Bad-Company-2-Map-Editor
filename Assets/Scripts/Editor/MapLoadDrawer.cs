using UnityEngine;
using System.Collections;
using UnityEditor;
using BC2;

[CustomEditor(typeof(MapLoad))]
public class MapLoadDrawer : Editor {
	string lastMapName;
	Texture2D texture = null;
	Partition partition = null;

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		MapLoad mapLoad = (MapLoad)target;


		if(GUILayout.Button("Save as " + mapLoad.saveAs))
		{
			mapLoad.Save();
		}
		string path = "Levels/"+mapLoad.mapName;
		string minimapPath;
		if (lastMapName != mapLoad.mapName) {
			if(Util.FileExist("Resources/"+path+".xml")) {
				partition = Util.LoadPartition (path);
				foreach (Field field in Util.GetComplex ("LevelDescription", partition.instance [0]).field) {
					if (field.name == "MinimapTexture") {
						minimapPath = Util.ClearGUIDString (field.reference);
						minimapPath += ".itexture";
						texture = Util.LoadiTexture (minimapPath);

					}
				}

			}
				lastMapName = mapLoad.mapName;
		}
		if (texture != null && !Application.isPlaying) {
			GUILayout.Label (texture, GUILayout.Width(EditorGUIUtility.currentViewWidth - 40), GUILayout.Height(EditorGUIUtility.currentViewWidth - 40));
		} 
	}

}

