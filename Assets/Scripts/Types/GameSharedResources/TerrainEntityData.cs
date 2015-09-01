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
	
	// Check if raw file already exists. If it does, just load it into the TerrainAsset.
	// If it doesn't exist, run the .sh or .bat file to convert the terrainheigtfield file into raw and then run the mogrify command to make it represent the actual map.
	// We won't worry about converting it back just yet, but it shouldn't be harder than to re-add the header after flipping and rotating it back to it's original format.
	
	
	// So, right now we just need to create the .sh file that will copy and rename the terrainheightfield file int raw and run the mogrify command¨.
	// Later, we will need to add windows support, because fuck mac. 
	// First of all, figure out how to copy and rename files.
	// Then figure out how we can check for files. Shouldn't be harder than to load the asset and just do the above if it doesn't exist.
	// Let's hope it's very fixed.
	
	// Let's get started. 
	public void sStart() {
		if (name != null) {
			var InstanceCollection = MapContainer.Load ("Assets/Resources/Terrain" + name + ".xml");
			foreach (Inst inst in InstanceCollection.instance) {
				//GenerateHavokItem(inst);
			}
		}
	}

	void Start() {
		if(System.IO.File.Exists("Assets/Resources/_Converted/" + transform.name + ".raw")) {
			UnityEngine.Debug.Log("Found the converted Texture");
		} else {
			//GetConvertInfo(transform.name);
		}
	}

	void GetConvertInfo(string path) {
		AddTempFile("TerrainFile",transform.name);
		var InstanceCollection = MapContainer.Load ("Assets/Resources/" + path + ".xml");
		foreach (Inst inst in InstanceCollection.instance) {
			HandleInstances(inst);
		}
	}
	
	void HandleInstances(Inst inst) {
		if(inst.type == "Terrain.TerrainData") {
			foreach(Field field in inst.field) {
				if(field.name == "SizeXZ") {
					AddTempFile("curRes", field.value);
				}
			}
			foreach(BC2Array array in inst.array) {
				
			}
		}
	}
	void AddTempFile(string type, string value) {
		if(type == "curRes") {
			string curResLocation = "/Users/Powback/Unity/Bad-Company-2-Map-Editor/Tools/Temp/CurRes.txt";
			string newResLocation = "/Users/Powback/Unity/Bad-Company-2-Map-Editor/Tools/Temp/NewRes.txt";
			string terrainLocation = "/Users/Powback/Unity/Bad-Company-2-Map-Editor/Tools/Temp/TerrainLocation.txt";
			float floatres = float.Parse(value);
			string stringres = floatres.ToString("0");
			int res = int.Parse(stringres);
			int newRes = res+1;
			File.WriteAllText(curResLocation, res + "x"+res);
			File.WriteAllText(newResLocation, newRes + "x"+newRes);
			File.WriteAllText(terrainLocation, transform.name);
			StartConvert();
		}
		if(type == "SizeY") {
			
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
		p.StartInfo.WorkingDirectory = "/Users/Powback/Unity/Bad-Company-2-Map-Editor/Tools";
		p.StartInfo.CreateNoWindow = false;

		p.Start ();
	}
}
