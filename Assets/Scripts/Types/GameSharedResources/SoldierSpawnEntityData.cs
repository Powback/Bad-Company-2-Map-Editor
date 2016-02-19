using UnityEngine;
using System.Collections;
using BC2;
public class SoldierSpawnEntityData : MonoBehaviour {

	// Use this for initialization
	void Start () {
		AssignChildren ();
	}
	
	void AssignChildren() {
		
		Inst inst = this.gameObject.GetComponent<BC2Instance>().instance;
		BC2Array array = Util.GetArray ("AlternativeSpawnPoints", inst);
		foreach (Item item in array.item) {
			string spawnPoint = item.reference;
			Util.GetGOByString (spawnPoint).gameObject.transform.parent = transform;

		}
	}
}
