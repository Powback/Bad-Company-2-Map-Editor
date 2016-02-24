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
			Vector3 pos = transform.position;
            Quaternion rot = transform.rotation;


            Matrix4x4 matrix = new Matrix4x4(); 
			matrix.SetTRS(pos.normalized,Normalize(rot),Vector3.one);
            Matrix4x4 m = matrix;
			Quaternion kek;

			Vector3 f = transform.forward;
			Vector3 u = transform.up;
			Vector3 r = transform.right * -1;

			string m02 = ( m.m02 ).ToString();
			string m10 = ( m.m10 * -1).ToString();
			string m00 = ( m.m00 * -1).ToString();
			string m21 = ( m.m21 * -1).ToString();
			string m11 = ( m.m11 ).ToString();
			string m01 = ( m.m01 ).ToString();
			string m22 = ( m.m22 ).ToString();
			string m12 = ( m.m12 ).ToString();
			string m20 = ( m.m20  * -1).ToString ();

			if( m.m02.ToString() == "1") { 	m02 = "1.0"; } else if(m.m02.ToString() == "-1") { m02 = "-1.0";}
			if( m.m10.ToString() == "1") { 	m10 = "1.0"; } else if(m.m10.ToString() == "-1") { m10 = "-1.0";}  
			if( m.m00.ToString() == "1") { 	m00 = "1.0"; } else if(m.m00.ToString() == "-1") { m00 = "-1.0";}  
			if( m.m21.ToString() == "1") { 	m21 = "1.0"; } else if(m.m21.ToString() == "-1") { m21 = "-1.0";}  
			if( m.m11.ToString() == "1") { 	m11 = "1.0"; } else if(m.m11.ToString() == "-1") { m11 = "-1.0";}  
			if( m.m01.ToString() == "1") { 	m01 = "1.0"; } else if(m.m01.ToString() == "-1") { m01 = "-1.0";}  
			if( m.m22.ToString() == "1") { 	m22 = "1.0"; } else if(m.m22.ToString() == "-1") { m22 = "-1.0";}  
			if( m.m12.ToString() == "1") { 	m12 = "1.0"; } else if(m.m12.ToString() == "-1") { m12 = "-1.0";}  
			if( m.m20.ToString() == "1") { 	m20 = "1.0"; } else if(m.m20.ToString() == "-1") { m20 = "-1.0";}  	


			matrixstring =
				m02+ "/" + m10 + "/" + m00 + "/*zero*/" +
				m21 + "/" + m11+ "/" + m01+ "/*zero*/" +
				m22+ "/" + m12+ "/" + m20+ "/*zero*/" +
				pos.z + "/" + pos.y + "/" + pos.x + "/*zero*";

            Util.GetComplex("Transform", instance).value = matrixstring;
        
		}
    }

	public Quaternion Normalize(Quaternion q){
		double nqx = double.Parse (q.x.ToString ());
		double nqy = double.Parse (q.y.ToString ());
		double nqz = double.Parse (q.z.ToString ());
		double nqw = double.Parse (q.w.ToString ());

		double norm2 = nqx*nqx + nqy*nqy + nqz*nqz + nqw*nqw;
		if (norm2 > Double.MaxValue) 
		{
			// Handle overflow in computation of norm2
			double rmax = 1.0/Max(Math.Abs(nqx),
				Math.Abs(nqy), 
				Math.Abs(nqz),
				Math.Abs(nqw)); 

			nqx *= rmax;
			nqy *= rmax; 
			nqz *= rmax;
			nqw *= rmax;
			norm2 = nqx*nqx + nqy*nqy + nqz*nqz + nqw*nqw;
		} 
		double normInverse = 1.0 / Math.Sqrt(norm2);
		nqx *= normInverse; 
		nqy *= normInverse; 
		nqz *= normInverse;
		nqw *= normInverse; 

		Quaternion quat = new Quaternion (float.Parse (nqx.ToString()), float.Parse (nqy.ToString()), float.Parse (nqz.ToString()), float.Parse (nqw.ToString()));
		return quat;
	}

	static private double Max(double a, double b, double c, double d)
	{ 
		if (b > a) 
			a = b;
		if (c > a) 
			a = c;
		if (d > a)
			a = d;
		return a; 
	}
}