using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Text.RegularExpressions;
using BC2;
using System.Reflection;

public class MapLoad : MonoBehaviour 
{
	
	//TODO
	//Fix havokasset
	[Header("Main config")]
	[Tooltip("Main map XML file")]
	public string mapName;

	[Tooltip("Saved as Resources/Levels/mapname.xml")]
	public string saveAs = "TestMap";

	[Header("Load Config")]
	public bool showHelpers = false;
	public bool loadAllPartitions = false;

	[Header("Prefabs")]

	public GameObject terrainHolder;
	public GameObject placeholder;
	public GameObject empty;
	public GameObject sphere;

	[Header("Materials")]
	public Material materialwhite;
	public Material waterMaterial;
	public Material materialInvisibleWall;
	public Material helperMaterial;
	public Material glassMaterial;

	[Header("Content")]
	public List<GameObject> instantiatedGameObjects = new List<GameObject>();
	public Dictionary<string, GameObject> instantiatedDictionary = new Dictionary<string, GameObject>();
	public Dictionary<string, BC2Mesh> instantiatedMeshDictionary = new Dictionary<string, BC2Mesh>();

	//public List<InstGameObject> InstGameObjects = new List<InstGameObject>();


	//public Dictionary<string, GameObject> goDictionary = new Dictionary<string, GameObject> ();

	Dictionary<string, Inst> unconvertedDictionary = new Dictionary<string, Inst>();

	public List<Inst> unconvertedInst = new List<Inst>();
	Dictionary<string, GameObject> convertedDictionary = new Dictionary<string, GameObject>();

	int convertedCount = 0;

	[NonSerialized]
	public Partition partition;
	int i;


    void Start()
    {
		Util.mapLoad = this;
        partition = Util.LoadPartition("levels/" + mapName);
//		StartCoroutine ("StartImport");
//		StartCoroutine (StartConvert());
		foreach (Inst inst in partition.instance) {
			//StartCoroutine(GenerateItem(inst));
			GenerateItem(inst);
		}
		Debug.Log ("Done adding items?");
		DoCleanup ();
	}


	void DoCleanup() {
//		GameObject terrain_SplinePlaneData = transform.GetComponent<MapItems> ().SelectParent ("Terrain.TerrainSplinePlaneData").gameObject;
//		int i = 0;
//		while(i < terrain_SplinePlaneData.transform.childCount) {
//			GameObject tspd = terrain_SplinePlaneData.transform.GetChild (i).gameObject;
//			if (tspd != null) {
//				if (tspd.GetComponent<BC2Instance> () != null) {
//					Util.Log (tspd.name);
//					tspd.GetComponent<TerrainSplinePlaneData> ().AssignChildren ();
//				} else {
//					Debug.Log ("Excecution order error");
//				}
//			}
//			i++;
//		}
	}

//	IEnumerator GenerateObject(Inst inst, bool haveMesh) {
//		Vector3 pos = Util.GetPosition (inst);
//		Quaternion rot = Util.GetRotation (inst);
//		GameObject go = null;
//		if (haveMesh) {
//			string meshPath = Util.GetField("ReferencedObject", inst).reference;
//			go = (GameObject) Instantiate(convertedDictionary[meshPath], pos,rot);
//		} else {
//			go = (GameObject) Instantiate (placeholder,pos,rot);
//		}
//
//		go.name = Util.ClearGUID (inst);
//		
//		//go.AddComponent<MeshRenderer>().material.color = new Color (UnityEngine.Random.Range (0.1f, 1.0f), UnityEngine.Random.Range (0.1f, 1.0f), UnityEngine.Random.Range (0.1f, 1.0f));
//		GameObject parent = transform.GetComponent<MapItems>().SelectParent(inst.type);
//		go.transform.parent = parent.transform;
//
//		BC2Instance instance = go.AddComponent<BC2Instance>();
//		instance.instance = inst;
//		instance.id = i;
//		instance.mapLoad = this;
//		instantiatedGameObjects.Add(go.gameObject);
//		instantiatedDictionary.Add (inst.guid.ToUpper(), go.gameObject);
//		i++;
//		yield return null;
//
//	}


	public void Save() {
		if (saveAs != "" && saveAs != null) {
			foreach (GameObject go in instantiatedGameObjects) {
				go.GetComponent<BC2Instance> ().SetPosRot ();
			}
			MapContainer.Save (partition, "Resources/Levels/" + saveAs + ".xml");
		} else {
			Util.Log ("No save name assigned");
		}
	}


	public string GetMeshPath(Inst inst) {
		List<Partition> partitions = new List<Partition>();
		string mesh = "";
		if(inst.type == "Entity.ReferenceObjectData" && (Util.GetField("ReferencedObject", inst).reference != null || Util.GetField("ReferencedObject", inst).reference != "null"))
		{
			name = Util.GetField("ReferencedObject", inst).reference;
			
			string cleanName = Util.ClearGUIDString(name);
			string refGuid = Util.GetGuid(name);
			
			
			Partition refPartition = Util.LoadPartition(cleanName);
		
			partitions.Add(refPartition);
			
			if(refPartition != null && name != "null") {
				Inst bluePrint = Util.GetInst(refGuid, refPartition);
				string refObject = "";
				if (bluePrint != null)
				{
					refObject = Util.GetField("Object", bluePrint).reference;
				} 
				
				
				if (Util.GetInst(refObject, refPartition) != null)
				{
					Inst staticModelEntityData = Util.GetInst(refObject, refPartition);
					if (staticModelEntityData != null)
					{
						if (Util.GetField("Mesh", staticModelEntityData) != null)
						{
							string refMesh = Util.GetField("Mesh", staticModelEntityData).reference;
							string refMeshClean = Util.ClearGUIDString(refMesh);
							string refMeshGuid = Util.GetGuid(refMesh);
							
							Partition meshPartition = Util.LoadPartition(refMeshClean);
							if(meshPartition != null)
							{
								Inst rigidMeshAsset = Util.GetInst(refMeshGuid, meshPartition);
								if(rigidMeshAsset != null)
								{
									string refMeshMesh = Util.GetField("Name", rigidMeshAsset).value;
									
									mesh = refMeshMesh + "_lod0_data";
									
								}
								
							}
							
						}
						
					}
				}
				
			}
			
		}
		return mesh;

	}

	// Horrible!

	//public IEnumerator GenerateItem(Inst inst) {
	void GenerateItem(Inst inst) {
		Vector3 pos = Util.GetPosition (inst);
		Quaternion rot = Util.GetRotation (inst);
        string name = "Unknown";
        string mesh = inst.type + " | " + inst.guid;
        List<Partition> partitions = new List<Partition>();
       
		//This part is just trying to get the actual model name. It goes through different partitions and blueprints in order to get an accurate model name.
		//Normally, it's fine to just do name + _lod0_data, but sometimes we have objects that reference non-existant objects, such as container_large_blue.
		//While container_large exists, _blue is just referencing an other instance, and thus an other material for said container.
		//It's mostly not an issue.
		if(inst.type == "Entity.ReferenceObjectData" && Util.GetField("ReferencedObject", inst) != null && (Util.GetField("ReferencedObject", inst).reference != null && Util.GetField("ReferencedObject", inst).reference != "null"))
        {
            name = Util.GetField("ReferencedObject", inst).reference;

            string cleanName = Util.ClearGUIDString(name);
            string refGuid = Util.GetGuid(name);
            

            Partition refPartition = Util.LoadPartition(cleanName);
            partitions.Add(refPartition);

            if(refPartition != null && name != "null") {
                Inst bluePrint = Util.GetInst(refGuid, refPartition);
                string refObject = "";
                if (bluePrint != null)
                {
                    refObject = Util.GetField("Object", bluePrint).reference;
                } 


                if (Util.GetInst(refObject, refPartition) != null)
                {
                    Inst staticModelEntityData = Util.GetInst(refObject, refPartition);
                    if (staticModelEntityData != null)
                    {
                        if (Util.GetField("Mesh", staticModelEntityData) != null)
                        {
                            string refMesh = Util.GetField("Mesh", staticModelEntityData).reference;
                            string refMeshClean = Util.ClearGUIDString(refMesh);
                            string refMeshGuid = Util.GetGuid(refMesh);

                            Partition meshPartition = Util.LoadPartition(refMeshClean);
                            if(meshPartition != null)
                            {
                                Inst rigidMeshAsset = Util.GetInst(refMeshGuid, meshPartition);
                                if(rigidMeshAsset != null)
                                {
                                    string refMeshMesh = Util.GetField("Name", rigidMeshAsset).value;

                                    mesh = refMeshMesh + "_lod0_data";

                                }
                               
                            }
                           
                        }

                    }
                }
               
            }
          
        }

		string meshpath = "Resources/" + mesh + ".meshdata";
		GameObject go = null;
		List<Mesh> subsetMesh = new List<Mesh> ();
		List<string> subsetNames = new List<string> ();

		if (Util.FileExist (meshpath)) {
			BC2Mesh bc2mesh = null;
			go = Instantiate (empty, Vector3.zero, Quaternion.identity) as GameObject;

			if (!(instantiatedMeshDictionary.TryGetValue (mesh, out bc2mesh))) {
				bc2mesh = MeshDataImporter.LoadMesh(meshpath);


				foreach(string s in subsetNames) {
					Debug.Log(s);
				}
				instantiatedMeshDictionary.Add (mesh, bc2mesh);
			}
			int subsetInt = 0;
			subsetMesh = bc2mesh.subMesh;
			subsetNames = bc2mesh.subMeshNames;
			foreach(Mesh sub in subsetMesh) {

				GameObject subGO = (GameObject) Instantiate(empty,Vector3.zero,Quaternion.identity);
				MeshRenderer mr = subGO.AddComponent<MeshRenderer>();
				MeshFilter mf = subGO.AddComponent<MeshFilter>();
				MeshCollider mc = subGO.AddComponent<MeshCollider> ();

				if (Regex.IsMatch (subsetNames [subsetInt].ToLower (), "glass")) {
					mr.material = glassMaterial;
				} else {
					mr.material = new Material (materialwhite);
				}
				mr.material.name = subsetNames[subsetInt];
				mf.mesh = sub;
				mf.mesh.RecalculateNormals();
				subGO.name = subsetNames[subsetInt];
				subGO.transform.parent = go.transform;
				mc.sharedMesh = mf.mesh;
				subsetInt++;
			}
			if (bc2mesh.inverted) {
				Vector3 localScale = go.transform.localScale;
				localScale.x *= -1;
				go.transform.localScale = localScale;
			}
		} else {
			go = Instantiate (placeholder.gameObject, pos, rot) as GameObject;
		}

		go.name = Util.ClearGUID (inst);

		//go.AddComponent<MeshRenderer>().material.color = new Color (UnityEngine.Random.Range (0.1f, 1.0f), UnityEngine.Random.Range (0.1f, 1.0f), UnityEngine.Random.Range (0.1f, 1.0f));
		GameObject parent = transform.GetComponent<MapItems>().SelectParent(inst.type);
		go.transform.parent = parent.transform;
        
		
		Matrix4x4 matrix = Util.GenerateMatrix4x4 (inst);	
		Quaternion newQuat = MatrixHelper.QuatFromMatrix(matrix);
		//Quaternion newQuat = go.transform.localRotation;
		//newQuat.x *= -1;
		//newQuat.z *= -1f;
		go.transform.localRotation = newQuat;
		go.transform.position = pos;
		if (inst.type == "Entity.ReferenceObjectData") {
			Vector3 scale = go.transform.localScale;
			scale.x *= -1; // the meshimporter is inverted. This is a workaround.
			go.transform.localScale = scale;

			if(Regex.IsMatch(go.name.ToLower() ,"invisiblewall")) {
				go.GetComponentInChildren<MeshRenderer>().material = new Material(materialInvisibleWall);
			}
		}
		BC2Instance instance = go.AddComponent<BC2Instance>();
        instance.instance = inst;
        instance.id = i;
		instance.mapLoad = this;
		instance.partitions = partitions;
		instantiatedGameObjects.Add(go.gameObject);
		instantiatedDictionary.Add (inst.guid.ToUpper(), go.gameObject);
		i++;
	//	yield return null;
	}

}
