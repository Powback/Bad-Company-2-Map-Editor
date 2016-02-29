using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BC2;


[SelectionBase]
public class BC2Instance : MonoBehaviour {
	public int id;
	public Inst instance;
	public List<Partition> partitions;
	public MapLoad mapLoad;
    string matrixstring;
	string name;

    void Start() {
		
		if(Util.GetField ("Name", instance) != null) {
			name = Util.GetField ("Name", instance).value;
			if (name != null) {
				transform.name = name;
			}
		}
		if (mapLoad.loadAllPartitions) {
			foreach (Field field in instance.field) {
				if (field.reference != null && field.reference != "null" && field.name != "ReferencedObject" && field.name != "Source" && field.name != "Target") {
					string refName = Util.ClearGUIDString (field.reference);
					if (Util.ClearGUIDString (field.reference) != null) {
						Partition refPartition = Util.LoadPartition (refName);
						refPartition.name = refName;
						partitions.Add (refPartition);

					}
				}
			}
		}

		string type = instance.type;
		if(type == "GameSharedResources.TerrainEntityData") {
           transform.gameObject.AddComponent<TerrainEntityData>();
		}
        if(type == "Physics.HavokAsset")
        {
           transform.gameObject.AddComponent<HavokAsset>();
		}
		if (type == "Terrain.TerrainSplineData") {
			transform.gameObject.AddComponent<TerrainSplineData> ();
		}
		if (type == "Terrain.TerrainSplinePointData") {
			transform.gameObject.AddComponent<TerrainSplinePointData> ();
		}	
		if (type == "Terrain.TerrainSplinePlaneData") {
			transform.gameObject.AddComponent<TerrainSplinePlaneData>();
		}
		if (type == "GameSharedResources.SoldierSpawnEntityData") {
			transform.gameObject.AddComponent<SoldierSpawnEntityData> ();
		}
		if (type == "GameSharedResources.TeamEntityData") {
			transform.gameObject.AddComponent<TeamEntityData> ();
		}



		if (type == "GameSharedResources.MissionObjectiveEntityData") {
			transform.gameObject.AddComponent<MissionObjectiveEntityData> ();
		}

		if (type == "GameSharedResources.AreaTriggerEntityData") {
			transform.gameObject.AddComponent<AreaTriggerEntityData> ();
		}
	}

    public void SetPosRot() {
		if (Util.GetComplex("Transform", instance) != null && Util.GetComplex("Transform", instance).value != "") {
			
			matrixstring = Util.GetMatrixString (transform);
            Util.GetComplex("Transform", instance).value = matrixstring;
        
		}

		if (instance.type == "Physcis.HavokAsset") {

		}
    }


}