using UnityEngine;
using System.Collections;
using BC2;

public class BC2Instance : MonoBehaviour {
	public int id;
	public Inst instance;
	public MapLoad mapLoad;
	public Vector3 lastPos;
	
//	public void FixedUpdate() {
//		if(transform.position != lastPos) {
//			foreach (Complex complex in instance.complex)
//			{
//				if(complex.name == "Transform") {
//
//					string position = complex.value;
//					GameObject selected = transform.gameObject;
//					Matrix4x4 m = Util.GenerateMatrix4x4String(position);
//					if(m.GetColumn(3).x != selected.transform.position.z || m.GetColumn(3).z != selected.transform.position.x || m.GetColumn(3).y != selected.transform.position.y) { 
//						Vector3 pos = selected.transform.position;
//						string coordiantes = complex.value;
//						string[] coords = coordiantes.Split ('/');
//						int numcoords = coords.Length;
//						if(numcoords > 3) { 
//						float rx = (float.Parse (coords [0]));
//						float ry = (float.Parse (coords [1]));
//						float rz = (float.Parse (coords [2]));
//						
//						float ux = (float.Parse (coords [4]));
//						float uy = (float.Parse (coords [5]));
//						float uz = (float.Parse (coords [6]));
//						
//						float fx = (float.Parse (coords [8]));
//						float fy = (float.Parse (coords [9]));
//						float fz = (float.Parse (coords [10]));
//						
//						float px = (float.Parse (coords [12]));
//						float py = (float.Parse (coords [13]));
//						float pz = (float.Parse (coords [14]));
//						
//						string zero = "*zero*";
//						
//						string newComplex = rx + "/"+ry+"/"+rx+"/"+zero+"/"+ux+"/"+uy+"/"+uz+"/"+zero+"/"+fx+"/"+fy+"/"+fz+"/"+zero+"/"+pos.z+"/"+pos.y+"/"+pos.z+"/"+zero;
//						
//						complex.value = newComplex;
//						Debug.Log("Updated Complex!");
//						lastPos = transform.position;
//						} else {
//							Debug.Log("Shit's the same");
//						}	
//					}
//				}
//			}
//		}
//	}

	void Start() {
		lastPos = transform.position;
		if(instance.type == "GameSharedResources.TerrainEntityData") {
           transform.gameObject.AddComponent<TerrainEntityData>();
		}
        if(instance.type == "Physics.HavokAsset")
        {
           transform.gameObject.AddComponent<HavokAsset>();
        }
		if (instance.type == "Terrain.TerrainSplinePlaneData") {
			transform.gameObject.AddComponent<TerrainSplinePlaneData>();
		}
		if (instance.type == "Terrain.TerrainSplineData") {
			transform.gameObject.AddComponent<TerrainSplineData> ();
		}
	}
}