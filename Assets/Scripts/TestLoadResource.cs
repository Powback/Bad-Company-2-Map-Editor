using UnityEngine;
using System.Collections;

public class TestLoadResource : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject go = Instantiate (Resources.Load ("objects/Vietnam/Rocks/NAM_Rock_02_Mesh_lod0_data", typeof(GameObject))) as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
