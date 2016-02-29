using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BC2;
using System.Text.RegularExpressions;

public class MeshDebug : MonoBehaviour {
	string path;
	string texture;
	int uvOffset;
	bool bigFloat;
	bool inverted;

	void UpdateMesh() {

		for( int i = 0; i < transform.childCount; i++) {
			Destroy(transform.GetChild(i).gameObject);
		}

		List<Mesh> subsetMesh = new List<Mesh> ();
		List<string> subsetNames = new List<string> ();

		if (Util.FileExist (path)) {
			BC2Mesh bc2mesh = null;
			GameObject go = Instantiate (Util.GetMapload().empty, Vector3.zero, Quaternion.identity) as GameObject;


			bc2mesh = MeshDataImporter.LoadMeshRaw(path, uvOffset,bigFloat, inverted);


			int subsetInt = 0;
			subsetMesh = bc2mesh.subMesh;
			subsetNames = bc2mesh.subMeshNames;
			foreach(Mesh sub in subsetMesh) {

				GameObject subGO = (GameObject) Instantiate(Util.GetMapload().empty,Vector3.zero,Quaternion.identity);
				MeshRenderer mr = subGO.AddComponent<MeshRenderer>();
				MeshFilter mf = subGO.AddComponent<MeshFilter>();
				MeshCollider mc = subGO.AddComponent<MeshCollider> ();

				mr.material = new Material (Util.GetMapload().materialwhite);

				if(texture != "") {
					mr.material.mainTexture = Util.LoadiTexture(texture);
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
		}
	}
}
