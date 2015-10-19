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
	private List<Vector3> vecRight = new List<Vector3>();
    private List<Vector3> vecUp = new List<Vector3>();
    private List<Vector3> vecForward = new List<Vector3>();
    private List<Matrix4x4> matrix = new List<Matrix4x4>();
    private List<Vector3> positions = new List<Vector3>();

	public void Start() {
        Inst instance = transform.gameObject.GetComponent<BC2Instance>().instance;
		Field field = Util.GetField ("Name", instance);   
        partition = Util.LoadPartition(field.value);
        foreach (Inst inst in partition.instance)
        {
            GeneratePosRot(inst);
            GenerateHavokItem(inst);
        }

    }


	public void GenerateHavokItem(Inst inst) {
		if (inst.array != null) {
			foreach (BC2Array array in inst.array) {
				if(array.name == "Assets") {
					int i = 0;
					foreach (Item item in array.item) {
						string name = CleanName(item.reference);
						string modelname = name;
						string actualmodelname = name + "_lod0_data";
						string actualmodelname2 = name + "_mesh_lod0_data";
						//Debug.Log(actualmodelname);
						if(Resources.Load(actualmodelname) != null) {
							modelname = actualmodelname;
						} else if(Resources.Load(actualmodelname2) != null) {
							modelname = actualmodelname2;
						} else {
							modelname = "Unknown";
						}
						if(modelname != "Unknown") {
							GameObject go = Resources.Load(modelname) as GameObject;
							Vector3 pos = positions[i];
							GameObject instGO = Instantiate(go, pos, Quaternion.identity) as GameObject;
                            Quaternion rot = MatrixHelper.QuatFromMatrix(matrix[i]);
                            instGO.transform.rotation = rot;
                            instGO.name = name;
							instGO.transform.parent = transform;
							i++;
						} else {
							Debug.Log("Could not load file " + name);
						}
					}
				}
			}
		}
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



	
	void GeneratePosRot(Inst inst) {
		int i = -1;
		string coordinates = null;
		if (inst.array != null) {
			foreach (BC2Array array in inst.array) { 
				if (array.name == "Transforms") {
					coordinates = array.value;
					//Debug.Log(coordinates);
				}
			}

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
}
