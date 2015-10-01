using UnityEngine;
using System.Collections;
using BC2;

public class BC2Instance : MonoBehaviour {
	public int id;
	public Inst instance;
	public MapLoad mapLoad;

	void Start() {
		if(instance.type == "GameSharedResources.TerrainEntityData") {
           transform.gameObject.AddComponent<TerrainEntityData>();
		}
        if(instance.type == "Physics.HavokAsset")
        {
           transform.gameObject.AddComponent<HavokAsset>();
        }
	}
}