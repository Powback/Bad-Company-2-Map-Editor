using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

public class TerrainEntityData : MonoBehaviour {
	public string Components;
	public int Enumeration;
	public string Name;
	public string Transform;
	public bool Enabled;
	public string PhysicsData;
	public string Material;
	public bool isObsolete;
	public string ReplacementObject;
	public string TerrainAsset;
	public string OutsideTerrainMaterial;
	
	int m_height = 0;
	int m_res = 0;
	int m_pos = 0;
	bool terrainLoaded;
	bool loadTerrain;
	bool preConverted;
	bool convertStarted;
	
	// Check if raw file already exists. If it does, just load it into the TerrainAsset.
	// If it doesn't exist, run the .sh or .bat file to convert the terrainheigtfield file into raw and then run the mogrify command to make it represent the actual map.
	// We won't worry about converting it back just yet, but it shouldn't be harder than to re-add the header after flipping and rotating it back to it's original format.
	
	
	// So, right now we just need to create the .sh file that will copy and rename the terrainheightfield file int raw and run the mogrify command¨.
	// Later, we will need to add windows support, because fuck mac. 
	// First of all, figure out how to copy and rename files.
	// Then figure out how we can check for files. Shouldn't be harder than to load the asset and just do the above if it doesn't exist.
	// Let's hope it's very fixed.
	
	// Let's get started. 
	void FixedUpdate() {

		if (terrainLoaded == false && loadTerrain) {
			if(System.IO.File.Exists("Assets/Resources/_Converted/" + transform.name + ".raw")) {
				UnityEngine.Debug.Log("Heightmap found, starting import with res:" + m_res + ", height: " + m_height);
				StartTerrainImport ();
				loadTerrain = false;
			}
		}
	}

	void Start() {
		if(System.IO.File.Exists("Assets/Resources/_Converted/" + transform.name + ".raw")) {
			UnityEngine.Debug.Log("Found the converted Texture. Loading it.");
			preConverted = true;
			GetConvertInfo(transform.name);
		} else {
			UnityEngine.Debug.Log("Trying to convert " + transform.name);
			GetConvertInfo(transform.name);
		}
	}

	void GetConvertInfo(string path) {
		//AddTempFile("TerrainFile",transform.name);
		var InstanceCollection = MapContainer.Load ("Assets/Resources/" + path + ".xml");
		foreach (Inst inst in InstanceCollection.instance) {
			UnityEngine.Debug.Log("Found terrain XML");
			HandleInstances(inst);
		}
	}
	
	void HandleInstances(Inst inst) {
		int curRes = 0;
		int height = 0;
		if(inst.type == "Terrain.TerrainData") {
			foreach(Field field in inst.field) {
				if(field.name == "SizeXZ") {
					float resFloat = float.Parse(field.value);
					int res = Mathf.RoundToInt(resFloat);
					m_res = res;
					CheckIfReady();
				}
			}
		}
		if (inst.type == "Terrain.TerrainHeightfieldData") {
			foreach (Field field in inst.field) {
				if(field.name == "SizeY") {
					float heightFloat = float.Parse(field.value);
					int iHeight = Mathf.RoundToInt(heightFloat);
					m_height = iHeight;
					CheckIfReady();
				}
			}
		}
	}

	void CheckIfReady() {
		if (m_height != 0 && m_res != null) {
			if(!preConverted) {
				AddTempFile(m_res);				
			}
			loadTerrain = true;
		}
	}
	void AddTempFile(int res) {
		if (res != 0 && convertStarted == false) {
			convertStarted = true;
			string curResLocation = "Tools/Temp/CurRes.txt";
			string newResLocation = "Tools/Temp/NewRes.txt";
			string terrainLocation = "Tools/Temp/TerrainLocation.txt";

			UnityEngine.Debug.Log("Resolution is " + res);

			int newRes = res + 1;
			
			File.WriteAllText (curResLocation, res + "x" + res);
			File.WriteAllText (newResLocation, newRes + "x" + newRes);
			File.WriteAllText (terrainLocation, transform.name);
			UnityEngine.Debug.Log("Starting convert");
			StartConvert ();
			terrainLoaded = true;
			
		} else {
			UnityEngine.Debug.Log ("Temp File add failed. No values passed");
		}
	}
	// TODO: Fix the shitty hardcoded script.
	void StartConvert() {
		Process p = new Process ();
		p.StartInfo.UseShellExecute = false;
		p.StartInfo.RedirectStandardInput = true;
		p.StartInfo.RedirectStandardOutput = true;

		p.StartInfo.FileName = "open";
		p.StartInfo.Arguments = "TerrainConvertMac.command";
		p.StartInfo.WorkingDirectory = "Tools";
		p.StartInfo.CreateNoWindow = false;

		p.Start ();
	}
	
	void StartTerrainImport() {
		terrainLoaded = true;
		GameObject terrain = GameObject.Find("Terrain");
		TerrainData tData = terrain.GetComponent<Terrain> ().terrainData;
		tData.heightmapResolution = m_res+1;
		tData.size = new Vector3 (m_res, m_height, m_res);
		float terrainPos = ((m_res * -1) / 2);
		terrain.transform.position = new Vector3(terrainPos, 0, terrainPos);
		UnityEngine.Debug.Log ("Loaded terrain with height: " + m_height + ", and res: " + m_res);
		ReadRaw(terrain, "Assets/Resources/_Converted/" + transform.name + ".raw", m_res + 1);

	}

	private void ReadRaw(GameObject terrainGO, string path, int size)
	{
		byte[] buffer;
		using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
		{
			buffer = reader.ReadBytes((size * size) * 2);
			reader.Close();
		}
		Terrain terrain = terrainGO.GetComponent<Terrain>();
		int heightmapWidth = terrain.terrainData.heightmapWidth;
		int heightmapHeight = terrain.terrainData.heightmapHeight;

		float[,] heights = new float[heightmapHeight, heightmapWidth];
		float num3 = 1.525879E-05f;
		for (int i = 0; i < heightmapHeight; i++)
		{
			for (int j = 0; j < heightmapWidth; j++)
			{
				int num6 = Mathf.Clamp(j, 0, size - 1) + (Mathf.Clamp(i, 0, size - 1) * size);
				byte num7 = buffer[num6 * 2];
				buffer[num6 * 2] = buffer[(num6 * 2) + 1];
				buffer[(num6 * 2) + 1] = num7;
				float num9 = System.BitConverter.ToUInt16(buffer, num6 * 2) * num3;
				heights[i, j] = num9;
			}
		}
		terrain.terrainData.SetHeights(0, 0, heights);
	}
}
