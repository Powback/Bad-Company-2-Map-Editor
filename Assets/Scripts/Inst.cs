using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BC2;

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
		if (type == "Terrain.TerrainSplinePlaneData") {
			transform.gameObject.AddComponent<TerrainSplinePlaneData>();
		}
		if (type == "Terrain.TerrainSplineData") {
			transform.gameObject.AddComponent<TerrainSplineData> ();
		}
		if (type == "Terrain.TerrainSplinePointData") {
			transform.gameObject.AddComponent<TerrainSplinePointData> ();
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
			Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;


            Matrix4x4 matrix = new Matrix4x4(); 
            matrix.SetTRS(pos,rot,Vector3.one);
            Matrix4x4 m = matrix;
			matrixstring =
				m.m02 + "/" + (m.m10 * -1) + "/" + (m.m00 * -1) + "/*zero*/" +
				m.m21 + "/" + m.m11 + "/" + m.m01 + "/*zero*/" +
				m.m22 + "/" + m.m12 + "/" + (m.m20 * -1) + "/*zero*/" +
				pos.z + "/" + pos.y + "/" + pos.x + "/*zero*";


            Util.GetComplex("Transform", instance).value = matrixstring;
        }
    }
}