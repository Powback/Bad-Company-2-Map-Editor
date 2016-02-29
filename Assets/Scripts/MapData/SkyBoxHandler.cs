using UnityEngine;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

public class SkyBoxHandler : MonoBehaviour {
	public Material skyboxVertexlit;
	string mapName;

	void Start () {
		mapName = transform.GetComponent<MapLoad>().mapName;
		string patternCQ = "CQ";
		string patternR= "R";
		string patternSR = "SR";
		string patternSDM = "SDM";

		mapName = Regex.Replace (mapName, patternCQ, "");
		mapName = Regex.Replace (mapName, patternR, "");
		mapName = Regex.Replace (mapName, patternSR, "");
		mapName = Regex.Replace (mapName, patternSDM, "");

		Texture skyBoxTexture = (Texture) Util.LoadiTexture ("Terrains/NAM_MP_007/Textures/NAM_MP_007_Sky.itexture");
		if (skyBoxTexture == null) {
			string mapCompact = Regex.Replace(mapName, "_", "");
			string mapCompact2 = Regex.Replace(mapCompact, "0", "");
			string mapCompact3 = Regex.Replace(mapCompact, "00", "0");
			Texture skyBoxTextureR = (Texture) Util.LoadiTexture ("Terrains/" + mapName + "R/Textures/" + mapName + "_Sky.itexture");
			Texture skyBoxTexture_01 = (Texture) Util.LoadiTexture ("Terrains/" + mapName + "/Textures/" + mapCompact + "_Sky_01.itexture");
			Texture skyBoxTexture1 = (Texture) Util.LoadiTexture ("Terrains/" + mapName + "/Textures/" + mapCompact2 + "_Sky_01.itexture");
			Texture skyBoxTexture01 = (Texture) Util.LoadiTexture ("Terrains/" + mapName + "/Textures/" + mapCompact3 + "_Sky_01.itexture");
			Texture skyBoxTexture01c = (Texture) Util.LoadiTexture ("Terrains/" + mapName + "/Textures/" + mapCompact3 + "_Sky_01_c.itexture");

			if(skyBoxTextureR != null) {
				skyBoxTexture = skyBoxTextureR;
			} else  if(skyBoxTexture01 != null) {
				skyBoxTexture = skyBoxTexture01;
			} else if(skyBoxTexture1 != null) {
				skyBoxTexture = skyBoxTexture1;
			} else if(skyBoxTexture_01 != null) {
				skyBoxTexture = skyBoxTexture_01;
			} else if(skyBoxTexture01c != null) {
				skyBoxTexture = skyBoxTexture01c;
			}
		}

		RenderSettings.skybox = skyboxVertexlit;
		RenderSettings.skybox.mainTexture = skyBoxTexture;
//		Debug.Log (mapName);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
