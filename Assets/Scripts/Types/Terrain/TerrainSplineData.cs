using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BC2;

public class TerrainSplineData : MonoBehaviour {

	public List<GameObject> points = new List<GameObject>();
	public List<GameObject> planes = new List<GameObject>();
    public MapLoad ml;


	public void Start() {
		transform.localScale = Vector3.one;
		//transform.rotation = Quaternion.identity;
		ml = Util.GetMapload ();
		BC2Array planeArray = Util.GetArray ("Planes", transform.GetComponent<BC2Instance> ().instance);
		BC2Array pointArray = Util.GetArray ("Points", transform.GetComponent<BC2Instance> ().instance);

		foreach (Item item in pointArray.item) {
			if (item.reference != "" && item.reference != "null" && item.reference != null) {
				points.Add (Util.GetGOByString (item.reference));
			}
		}

		foreach (Item item in planeArray.item) {
			if (item.reference != "" && item.reference != "null" && item.reference != null) {
				GameObject plane = Util.GetGOByString (item.reference);
				planes.Add (plane);
				plane.transform.parent = transform;
				plane.GetComponent<TerrainSplinePlaneData> ().positionPoints = points;
				plane.GetComponent<TerrainSplinePlaneData> ().AssignValues ();
			}
		}
			

		DrawLines ();
	
	
	}

	void DrawLines() {
		LineRenderer LR = gameObject.AddComponent<LineRenderer>();
		LR.SetVertexCount(points.Count);
		for(int i = 0; i < points.Count; i++)
		{
			LR.SetPosition(i, points[i].transform.position);
		}

	}
}
