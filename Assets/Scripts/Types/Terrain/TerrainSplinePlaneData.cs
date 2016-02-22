using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BC2;

public class TerrainSplinePlaneData : MonoBehaviour {
	
	public List<GameObject> positionPoints = new List<GameObject>();
	public List<GameObject> dataPoints = new List<GameObject> ();


	public void AssignValues() {
		AssignChildren ();
		if (Util.GetField ("PlaneType", transform.GetComponent<BC2Instance> ().instance).value == "Lake") {
			AssignLakeMesh ();
		}
	}

	public void AssignChildren() {
		Inst inst = transform.GetComponent<BC2Instance>().instance;
		BC2Array points = Util.GetArray ("Points", inst);

		foreach (Item item in points.item) {
			if (item.reference != "" && item.reference != "null" && item.reference != null) {
				GameObject point = Util.GetGOByString (item.reference);
				dataPoints.Add (point);
			}
		}

		foreach (GameObject positionPoint in positionPoints) {
			positionPoint.transform.parent = transform;

		}
		foreach (Item item in points.item) {
			if (item.reference != "" && item.reference != "null" && item.reference != null) {
				GameObject point = Util.GetGOByString (item.reference);
				string parent = Util.GetField ("AffectedPoint", point.GetComponent<BC2Instance> ().instance).reference;
				if (parent != "" && parent != "null" && parent != null) {
					GameObject parentGO =  Util.GetGOByString (parent);
					point.transform.parent = parentGO.transform;
					//point.transform.position = parentGO.transform.position;
				}
			}
		}

	}

	public void AssignLakeMesh() {

		Inst plane = transform.GetComponent<BC2Instance> ().instance;
		GameObject go = (GameObject) Instantiate(Util.GetMapload().empty, new Vector3(0,positionPoints[0].transform.position.y, 0), Quaternion.identity);
		GeneratePlane gp = go.gameObject.AddComponent<GeneratePlane> ();
		go.transform.parent = transform;
		go.transform.name = "Water";
		foreach(GameObject pos in positionPoints) {
			gp.points.Add (pos.transform.position);
		}

		transform.localScale = Vector3.one;
		Vector3 startpos = new Vector3 (0, 0, 0);
		startpos.y = gp.points [0].y;
		//transform.position = startpos;
		//transform.rotation = new Quaternion (0, 0, 0, 0);

		gp.Generate ();
		go.GetComponent<MeshRenderer> ().material = Util.GetMapload().waterMaterial;
	}
}


