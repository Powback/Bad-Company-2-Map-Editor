using UnityEngine;
using System.Collections;
using BC2;
public class MissionObjectiveEntityData : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Inst inst = transform.GetComponent<BC2Instance> ().instance;
		BC2Array array = Util.GetArray ("ChildObjective", inst);
		foreach (Item item in array.item) {
			if (Util.GetGOByString (item.reference) != null) {
				Util.GetGOByString (item.reference).transform.parent = transform;
			}
		}
	}
	

}
