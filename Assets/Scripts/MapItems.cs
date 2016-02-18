using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MapItems : MonoBehaviour {
	
	
	public GameObject emptyGO;
	public GameObject dataholder;
	public GameObject helperholder;

	public List<GameObject> types;
	public List<GameObject> helperTypes;

	public GameObject SelectParent(string typename) {
		GameObject parent = null;
		bool foundParent = false;
		foreach (GameObject type in types)
		{
			if(type.name == typename) {
				parent = type.transform.gameObject;
				foundParent = true;
			}	
		}
		if(!foundParent) {
			
			parent = GameObject.Instantiate(emptyGO.gameObject, Vector3.zero,Quaternion.identity) as GameObject;			
			parent.name = typename;
			parent.transform.parent = dataholder.transform;
			types.Add(parent.transform.gameObject);
		}
		return parent.gameObject;
		
	}
	
	public GameObject SelectHelperParent(string typename) {
		GameObject parent = null;
		bool foundParent = false;
		foreach (GameObject type in helperTypes)
		{
			if(type.name == typename) {
				parent = type.transform.gameObject;
				foundParent = true;
			}	
		}
		if(!foundParent) {

			parent = GameObject.Instantiate(emptyGO.gameObject, Vector3.zero,Quaternion.identity) as GameObject;			
			parent.name = typename;
			parent.transform.parent = dataholder.transform;
			types.Add(parent.transform.gameObject);
		}
		return parent.gameObject;

	}
	

	

}

