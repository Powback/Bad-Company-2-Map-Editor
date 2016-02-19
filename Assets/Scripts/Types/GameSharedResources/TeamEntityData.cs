using UnityEngine;
using System.Collections;
using BC2;
public class TeamEntityData : MonoBehaviour {

	// Use this for initialization
	void Start () {
		AssignChildren ();
	}
	
	void AssignChildren() {
		BC2Array squads = Util.GetArray ("Squads", transform.GetComponent<BC2Instance>().instance);
		foreach (Item item in squads.item) {
			if (item.reference != "" && item.reference != "null" && item.reference != null) {
				GameObject squad = Util.GetGOByString (item.reference);
				squad.transform.parent = transform;
			}
		}
	}
}
