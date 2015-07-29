using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Text.RegularExpressions;

public class MapLoad : MonoBehaviour {

	//Load all mesh (Grouped objects are missing)
	//convert all itexture to dds (some are missing. Fix UV map and find correct texture?)
	//load texture based on meshdata-data?
	//Allow back-parsing
		//Edit items based on guid
	//Terrain loading







	//
	public string mapName;
	public GameObject placeholder;
	public Material material_white;

	// Use this for initialization
	void Start () {
		var InstanceCollection = MapContainer.Load ("Assets/maps/" + mapName + ".xml");
		foreach (Inst inst in InstanceCollection.instance) {
			GenerateItem(inst, transform.GetComponent<MapItems>().ItemType(inst.type));
		}
		//Debug.Log (InstanceCollection.instance.Count);
	}

	Vector3 CalculatePosition(Inst inst, int id) {
		Vector3 pos = Vector3.zero;
		if (transform.GetComponent<MapItems> ().IsObject (id)) {
			if (inst.complex != null) {
				if (inst.complex.value != null) {
					//Debug.Log("pos val for " + inst.guid + " | " + inst.complex.value);
					string coordiantes = inst.complex.value;
					string[] coords = coordiantes.Split ('/');
					int numcoords = coords.Length;
					float z = float.Parse (coords [(numcoords - 4)]);
					float y = float.Parse (coords [(numcoords - 3)]);
					float x = float.Parse (coords [(numcoords - 2)]);
					pos = new Vector3 (x, y, z);
				}
			}
		}
		return pos;
	}

	Vector3 CalculateRotation(Inst inst, int id, string type) {
		Vector3 rot = Vector3.zero;
		if (transform.GetComponent<MapItems>().IsObject(id) && inst.complex.value != null) {
			string coordiantes = inst.complex.value;
			string[] coords = coordiantes.Split ('/');
			int numcoords = coords.Length;
			if(numcoords > 3) { 
				float rz = (float.Parse (coords [(0)]));
				float ry = (float.Parse (coords [1]));
				float rx = (float.Parse (coords [2]));

				float uz = (float.Parse (coords [4]));
				float uy = (float.Parse (coords [5]));
				float ux = (float.Parse (coords [6]));
				
				float fz = (float.Parse (coords [8]));
				float fy = (float.Parse (coords [9]));
				float fx = (float.Parse (coords [10]));
				if(type == "r") {
					rot = new Vector3(rx,ry,rz);
				} else if(type == "u") {
					rot = new Vector3(ux,uz,uy);
				} else if(type == "f") {
					rot = new Vector3(fx,fy,fz);
				}
			}
		}
		return rot;
	}

	String CleanName(Inst inst, int id) {
		string name = "Unknown";
		foreach(Field field in inst.field) {
			if(transform.GetComponent<MapItems>().IsObject(id) && field.refference != null && field.refference != "null") {
				string pattern = "/[0-9a-z]+-[0-9a-z]+-[0-9a-z]+-[0-9a-z]+-[0-9a-z]+";
				string pattern2 = "_entity";
				string pattern3 = "_asset";
				name = field.refference;
				name = Regex.Replace(name,pattern,"");
				name = Regex.Replace(name,pattern2,"");
				name = Regex.Replace(name,pattern3,"");
				//Debug.Log(name);
			}

		}
		if (name == "Unknown" || name == "null" || name == null) {
			
			if (inst.type != null) {
				name = inst.type + " | " + inst.guid;
			}
		}
		return name;
	}
	
		



	void GenerateItem(Inst inst, int id) {
		if (transform.GetComponent<MapItems> ().IsObject (id)) {
			//Debug.Log(inst.guid + " | " + id);
			Vector3 pos = CalculatePosition (inst, id);
			GameObject model;
			string actualmodelname = CleanName(inst, id) + "_lod0_data";
			string actualmodelname2 = CleanName(inst, id) + "_mesh_lod0_data";
			//Debug.Log(actualmodelname);

			if(Resources.Load(actualmodelname) != null) {
				model = Resources.Load(actualmodelname) as GameObject;
            } else if(Resources.Load(actualmodelname2) != null) {
				model = Resources.Load(actualmodelname2) as GameObject;
			} else {
				model = placeholder.gameObject;
			}
			GameObject go = GameObject.Instantiate(model, pos, Quaternion.identity) as GameObject;
			//go.AddComponent<ChangeUV>();
			AddValues(inst, id, go);
			if(id != 63 || id != 64 || id != 65 || id != 66) {
				go.name = CleanName (inst, id);
			} else {
				go.name = "Terrain.TerrainSpline";
			}
			//Debug.Log("Setting parent id to go" + id);
			GameObject parent = transform.GetComponent<MapItems>().SelectParents(id);
			//Debug.Log(parent.name);
			go.transform.parent = parent.transform;
			//Debug.Log("ID is set");

			//transform.GetComponent<MapItems>().SelectParent();
		} else {

		}
		//go.transform.parent = transform.GetComponent<MapItems>().SelectParent(id).gameObject.transform;
	}

	void AddValues(Inst inst, int id, GameObject instgo) {
		//AreaGEometryEntity
		if (id == 1) {
			Debug.Log("Called AGED");
			AreaGeometryEntityData AGED = instgo.AddComponent<AreaGeometryEntityData>();
			foreach(Field field in inst.field) {
				if(field.value != null && field.value != "null") {
					if(field.name == "Components" ) {
						AGED.components = field.value;
					}
					//transform?
					if(field.name == "Enumeration" ) {
						AGED.enumeration = field.value;
					}
					if(field.name == "Name") {
						AGED.name = field.refference;
					}
					if(field.name == "Height") {
						AGED.height = field.value;
					}
					if(field.name == "Weight") {
						AGED.weight = field.value;
					}
					if(field.name == "Next") {
						AGED.next = field.refference;
					}
					if(field.name == "Previous") {
						AGED.previous = field.value;
					}
				}
			}
		}
		if (id == 4) {
			Vector3 right = CalculateRotation(inst, id, "r");
			Vector3 up = CalculateRotation(inst, id, "u");
			Vector3 forward = CalculateRotation(inst, id, "f");
			RotationSlave rotslave = instgo.AddComponent<RotationSlave>();
			rotslave.forward = forward;
			rotslave.up = up;
			rotslave.right = right;
		}
		if (id == 60) {
			Debug.Log("Called Havok");
			HavokAsset havok = instgo.AddComponent<HavokAsset>();
			foreach(Field field in inst.field) {
				if(field.value != null && field.value != "null") {
					if(field.name == "Name") {
						havok.name = field.value;
					}
					if(field.name == "MemoryReportGroup") {
						havok.memoryReportGroup = field.value;
					}
					if(field.name == "SourceFile") {
						havok.sourceFile = field.value;
					}
					if(field.name == "RootNode") {
						havok.rootNode = field.value;
					}
					if(field.name == "ParticleAllotment") {
						havok.particleAllotment = field.value;
					}
					if(field.name == "IsStatic") {
						havok.isStatic = field.value;
					}
					if(field.name == "IsDynamic") {
						havok.isDynamic = field.value;
					}
					if(field.name == "IsGroupable") {
						havok.isGroupable = field.value;
					}
					if(field.name == "Mass") {
						havok.mass = field.value;
					}
					if(field.name == "HkxDataFilePath") {
						havok.hkxDataFilePath = field.value;
					}
					if(field.name == "HkxImgFilePath") {
						havok.hkxImgFilePath = field.value;
					}
					if(field.name == "CollisionNode") {
						havok.collisionNode = field.value;
					}
					if(field.name == "DetailNode") {
						havok.detailNode = field.value;
					}
				}
			}
			
			foreach (BC2Array array in inst.array) {
				if(array.value != null && array.value != "null") {
					if(array.name == "RigidBodyDatas") {
						havok.rigidBodyDatas = array.value;
					}
					if(array.name == "MaterialMappings") {
						havok.materialMappings = array.value;
					}
					if(array.name == "MaterialSets") {
						havok.materialSets = array.value;
					}
					if(array.name == "ReferencedMaterials") {
						havok.referencedMaterials = array.value;
					}
					if(array.name == "PhysicsMaterialSets") {
						havok.physicsMaterialSets = array.value;
					}
					if(array.name == "PartInfoDatas") {
						havok.partInfoDatas = array.value;
					}
					if(array.name == "FaceMaterials") {
						havok.faceMaterials = array.value;
					}
					
				}
			}
		}
	}
}

