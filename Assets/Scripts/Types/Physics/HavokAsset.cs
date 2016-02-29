using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Text.RegularExpressions;
using BC2;


public class HavokAsset : MonoBehaviour {
    public Partition partition;
    private List<string> havokItems;

    private List<Matrix4x4> matrix = new List<Matrix4x4>();
    private List<Vector3> positions = new List<Vector3>();
	private List<GameObject> gameObjects = new List<GameObject>();
	public Dictionary<string, BC2Mesh> instantiatedMeshDictionary = new Dictionary<string, BC2Mesh>();
	public Dictionary<string, GameObject> instantiatedDictionary = new Dictionary<string, GameObject>();

	private GameObject emptyGO;

	public void Start() {
		emptyGO = Util.GetMapload ().empty.gameObject;

        Inst instance = transform.gameObject.GetComponent<BC2Instance>().instance;
		Field field = Util.GetField ("Name", instance);   
        partition = Util.LoadPartition(field.value);
        foreach (Inst inst in partition.instance)
        {
            GenerateHavokItem(inst);
        }

    }

	public void UpdatePosRot() { // incomplete, as the positions aren't stored in the main level file. 
		foreach (GameObject go in gameObjects) {
			int id = go.GetComponent<HavokItem> ().ID;
			havokItems [id] = Util.GetMatrixString (go.transform);
		}

	}


	public void GenerateHavokItem(Inst inst) {
		string coordinatestring = "";
		if (inst.array != null) {
			foreach (BC2Array array in inst.array) {
				if (array.name == "Transforms") {
					coordinatestring = array.value;
					GeneratePosRot (coordinatestring);
				}
				if (array.name == "Assets") {
					int i = 0;
					foreach(Item item in array.item) {
						Vector3 pos = positions [i];
						Quaternion rot = MatrixHelper.QuatFromMatrix(matrix[i]);
						string name = CleanName (item.reference);
						string modelName = name;
						string actualModelName = name + "_lod0_data.meshdata";
						string actualModelName2 = name + "_mesh_lod0_data.meshdata";

						if (Util.FileExist ("Resources/"+actualModelName)) {
							modelName = actualModelName;
						} else if (Util.FileExist ("Resources/"+actualModelName2)) {
							modelName = actualModelName2;
						} else {
							modelName = "Havok_unknown";
						}


						if (modelName != "Havok_unknown") {
				
//							if (instantiatedDictionary.ContainsKey (name)) {
//								instantiatedDictionary.TryGetValue (name, out go);
//								Util.Log ("Saving time on caching yo");
//							} else {
								GameObject go = loadMesh ("Resources/" + modelName);
//								instantiatedDictionary.Add (name, go.gameObject);
//							}
//
							go.transform.position = pos;
							go.transform.rotation = rot;
							go.transform.localScale = Vector3.one;
							Vector3 scale = go.transform.localScale;
							scale.x *= -1;
							go.transform.localScale = scale;

							go.name = name;


							go.transform.parent = transform;
							HavokItem hi = go.AddComponent<HavokItem> ();
							hi.ID = i;
							hi.pos = pos;
							hi.rot = rot;
							gameObjects.Add (go);
						}
						i++;

					}
				}
			}
		}
	}


	public GameObject loadMesh(string meshpath) {
		GameObject go = Instantiate (emptyGO, Vector3.zero, Quaternion.identity) as GameObject;


		if (Util.FileExist (meshpath)) {
			BC2Mesh bc2mesh = null;

			List<Mesh> subsetMesh = new List<Mesh> ();
			List<string> subsetNames = new List<string> ();
			int subsetInt = 0;

			if (!(instantiatedMeshDictionary.TryGetValue (meshpath, out bc2mesh))) {
				bc2mesh = MeshDataImporter.LoadMesh(meshpath);
				instantiatedMeshDictionary.Add (meshpath, bc2mesh);
			}

			subsetMesh = bc2mesh.subMesh;
			subsetNames = bc2mesh.subMeshNames;
			foreach(Mesh sub in subsetMesh) {

				GameObject subGO = (GameObject) Instantiate(emptyGO, Vector3.zero,Quaternion.identity);
				MeshRenderer mr = subGO.AddComponent<MeshRenderer>();
				MeshFilter mf = subGO.AddComponent<MeshFilter>();
				mr.material = new Material(Util.GetMapload().materialwhite);
				mr.material.name = subsetNames[subsetInt];
				mf.mesh = sub;
				mf.mesh.RecalculateNormals();
				subGO.name = subsetNames[subsetInt];
				subGO.transform.parent = go.transform;

				subsetInt++;
			}
			if (bc2mesh.inverted) {
				Vector3 localScale = go.transform.localScale;
				localScale.x *= -1;
				go.transform.localScale = localScale;
			}
		}
		return go;
	}


	public void GenerateHavokItem2(Inst inst) {
//		if (inst.array != null) {
//			foreach (BC2Array array in inst.array) {
//				if(array.name == "Assets") {
//					int i = 0;
//					foreach (Item item in array.item) {
//						string name = CleanName(item.reference);
//						string modelname = name;
//						string actualmodelname = name + "_lod0_data";
//						string actualmodelname2 = name + "_mesh_lod0_data";
//						//Debug.Log(actualmodelname);
//						if(Util.FileExist(actualmodelname)) {
//							modelname = actualmodelname;
//						} else if(Util.FileExist(actualmodelname2)) {
//							modelname = actualmodelname2;
//						} else {
//							modelname = "Unknown";
//						}
//						if(modelname != "Unknown") {
//
//							GameObject go = Util.LoadMesh(modelname) as GameObject;
//							Vector3 pos = positions[i];
//							GameObject instGO = Instantiate(go, pos, Quaternion.identity) as GameObject;
//                            Quaternion rot = MatrixHelper.QuatFromMatrix(matrix[i]);
//                            instGO.transform.rotation = rot;
//                            instGO.name = name;
//							instGO.transform.parent = transform;
//							i++;
//						} else {
//							Debug.Log("Could not load file " + name);
//						}
//					}
//				}
//			}
//		}
	}


	String CleanName(string name) {
		if (name == null) {
			name = "unknown";
		}

		string pattern = "/[0-9a-z]+-[0-9a-z]+-[0-9a-z]+-[0-9a-z]+-[0-9a-z]+";
		name = Regex.Replace (name, pattern, "");
		string pattern2 = "_havok";
		string pattern3 = "_asset";
		name = Regex.Replace (name, pattern3, "");

		name = Regex.Replace (name, pattern2, "");
		return name;
	}



	
	void GeneratePosRot(string coordinates) {
		int i = -1;



		string[] coords = coordinates.Split ('/');
		//int numcoords = coords.Length;
		//Debug.Log(numcoords);
		while(i < coords.Length - 1) {
            Matrix4x4 m = new Matrix4x4();


			float rz = 	float.Parse (coords [i + 1]) * -1;
			float ry =	float.Parse (coords [i + 2]) * -1;
			float rx =	float.Parse (coords [i + 3]) * -1;

			float uz =	float.Parse (coords [i + 5]);
			float uy =	float.Parse (coords [i + 6]);
			float ux =	float.Parse (coords [i + 7]);

			float fz =	float.Parse (coords [i + 9]);
			float fy =	float.Parse (coords [i + 10]);
			float fx =	float.Parse (coords [i + 11]);

			float z =	float.Parse (coords [(i + 13)]);
			float y = 	float.Parse (coords [(i + 14)]);
			float x = 	float.Parse (coords [(i + 15)]);

            m.SetColumn(0, new Vector4(rx, ry, rz, 0));
            m.SetColumn(1, new Vector4(ux, uy, uz, 0));
            m.SetColumn(2, new Vector4(fx, fy, fz, 0));
            m.SetColumn(3, new Vector4(x, y, z, 0));


			Vector3 pos = new Vector3(x,y,z);


            matrix.Add(m);
			positions.Add(pos);
			i+=16;

		}
			

	}
}
