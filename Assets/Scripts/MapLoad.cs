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
	
	public string mapName;
	public GameObject placeholder;
    public GameObject empty;
    public GameObject terrainHolder;
	public Material materialwhite;
    public Component waterScript;
    public Material waterMaterial;
	[NonSerialized]
	public Partition partition;
	int i;
	public string saveAs = "TestMap";
    public bool save;
    public bool saved;
	//public List<InstGameObject> InstGameObjects = new List<InstGameObject>();

	public int MaxThreads = 1;
	public int threads = 0;

	//public Dictionary<string, GameObject> goDictionary = new Dictionary<string, GameObject> ();
	public List<GameObject> instantiatedGameObjects = new List<GameObject>();
	public Dictionary<string, GameObject> instantiatedDictionary = new Dictionary<string, GameObject>();
	public Dictionary<string, BC2Mesh> instantiatedMeshDictionary = new Dictionary<string, BC2Mesh>();
	Dictionary<string, Inst> unconvertedDictionary = new Dictionary<string, Inst>();

	public List<Inst> unconvertedInst = new List<Inst>();
	Dictionary<string, GameObject> convertedDictionary = new Dictionary<string, GameObject>();

	int convertedCount = 0;

    void Start()
    {
		Util.mapLoad = this;
        partition = Util.LoadPartition("levels/" + mapName);
//		StartCoroutine ("StartImport");
//		StartCoroutine (StartConvert());
		foreach (Inst inst in partition.instance) {
			StartCoroutine(GenerateItem(inst));
		}

	}




	IEnumerator GenerateObject(Inst inst, bool haveMesh) {
		Vector3 pos = Util.GetPosition (inst);
		Quaternion rot = Util.GetRotation (inst);
		GameObject go = null;
		if (haveMesh) {
			string meshPath = Util.GetField("ReferencedObject", inst).reference;
			go = (GameObject) Instantiate(convertedDictionary[meshPath], pos,rot);
		} else {
			go = (GameObject) Instantiate (placeholder,pos,rot);
		}

		go.name = Util.ClearGUID (inst);
		
		//go.AddComponent<MeshRenderer>().material.color = new Color (UnityEngine.Random.Range (0.1f, 1.0f), UnityEngine.Random.Range (0.1f, 1.0f), UnityEngine.Random.Range (0.1f, 1.0f));
		GameObject parent = transform.GetComponent<MapItems>().SelectParent(inst.type);
		go.transform.parent = parent.transform;

		BC2Instance instance = go.AddComponent<BC2Instance>();
		instance.instance = inst;
		instance.id = i;
		instance.mapLoad = this;
		InstGameObject instGameObject = new InstGameObject ();
		instantiatedGameObjects.Add(go.gameObject);
		instantiatedDictionary.Add (inst.guid, go.gameObject);
		i++;
		yield return null;

	}

	void FixedUpdate()
    {
        if(save && saved == false)
        {
            saved = true;
            foreach (GameObject go in instantiatedGameObjects) {
                go.GetComponent<BC2Instance>().SetPosRot();
            }
            MapContainer.Save(partition, "Resources/Levels/"+ saveAs + ".xml");
        }
	}




	public string GetMeshPath(Inst inst) {
		List<Partition> partitions = new List<Partition>();
		Partition partition = new Partition();
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
	public IEnumerator GenerateItem(Inst inst) {
		Vector3 pos = Util.GetPosition (inst);
		Quaternion rot = Util.GetRotation (inst);
        string name = "Unknown";
        string mesh = inst.type + " | " + inst.guid;
        List<Partition> partitions = new List<Partition>();
        Partition partition = new Partition();
		Mesh meshfile = null;
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
				mr.material = new Material(materialwhite);
				mr.material.name = subsetNames[subsetInt];
				mf.mesh = sub;
				mf.mesh.RecalculateNormals();
				subGO.name = subsetNames[subsetInt];
				subGO.transform.parent = go.transform;
				subsetInt++;
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
			scale.x *= -1;
			go.transform.localScale = scale;
		}
		BC2Instance instance = go.AddComponent<BC2Instance>();
        instance.instance = inst;
        instance.id = i;
		instance.mapLoad = this;
	
		//InstGameObjects.Add (instGameObject);
		instantiatedDictionary.Add (inst.guid, go.gameObject);
		i++;
		yield return null;
	}

}
