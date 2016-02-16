using UnityEngine;
using System.Collections;
using BC2;
public class TerrainSplinePointData : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Inst inst = transform.GetComponent<BC2Instance>().instance;
        string parentGUID = Util.GetField("Parent", inst).reference;
		Util.Log (parentGUID);
		if (parentGUID != null) {
			GameObject parent = Util.GetGOByString (parentGUID.ToUpper());
			if (parent.gameObject != null) {
				transform.parent = parent.transform;

			} else {
			}
		} else {
		}
	}
	
}
