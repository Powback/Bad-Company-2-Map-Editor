using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Text.RegularExpressions;

public class MapLoad : MonoBehaviour {

	//Load all mesh (Grouped objects are missing) - fixed. All objects should be loaded ( with the exception of flags/mcoms)
	//convert all itexture to dds (some are missing. Fix UV map and find correct texture?)
	//load texture based on meshdata-data?
	//Allow back-parsing
		//Edit items based on guid
	//Terrain loading
	  // If terrain raw file is not found, run the terminal script at the terrainheightfield file and load item.
	  // if terrain raw file is found, create a new terrain and apply the raw heightmap.

	  // Make HavokAsset use this function instead of the one it's using.






	//
	public string mapName;
	public GameObject placeholder;
	public Material material_white;

	// Use this for initialization
	void Start () {
		var InstanceCollection = MapContainer.Load ("Assets/Resources/Levels/" + mapName + ".xml");
		foreach (Inst inst in InstanceCollection.instance) {
			GenerateItem(inst);
		}
		//Debug.Log (InstanceCollection.instance.Count);
	}



	// Kind of redundant, but we're getting the position twice here. Look into it in the future.
	// We are cleaning the name twice in order to find the actual name instead of name_mesh and name_lod. This is useful because name_mesh is not.
	// Then we check if the acutal model exists. If it doesn't, we replace it with an empty game objects. 
	// If it doesn't exist, we most likely failed to convert all objects. I will blame this on the occulsion models that can't be exported for some reason.
	// Hopefully this will be solved before you see this. If not, I'm sorry. Just delete the models that won't export. It shouldn't matter.
	// Eventually we go ahead and spawn either the placeholder or the model. We then run the addvalues function which will add values and components to the correct instances.
	// This is the core to the way me make shit work. Sorry if anything is misspelled here. I'm writing this in Visual Code. It doesn't realize that this is a comment and therefore tries to suggest functions to use. Don't use Visual Code. 
	// Fuck Monodevelopt too for crashing all the fucking TimeUntilAbandoned.
	// You see this shit? TimeUntilAbandoned. I was going to write TimeUntilAbandoned. Fuck Visual Code;
	void GenerateItem(Inst inst) {
		//if (IsObject (inst)) {
			//Debug.Log(inst.guid + " | " + id);
			Vector3 pos = Util.CalculatePosition (inst);
			GameObject model;
			string actualmodelname = Util.ClearGUID(inst) + "_lod0_data";
			string actualmodelname2 = Util.ClearGUID(inst) + "_mesh_lod0_data";
			//Debug.Log(actualmodelname);

			if(Resources.Load(actualmodelname) != null) {
				model = Resources.Load(actualmodelname) as GameObject;
            } else if(Resources.Load(actualmodelname2) != null) {
				model = Resources.Load(actualmodelname2) as GameObject;
			} else {
				model = placeholder.gameObject;
			}
			GameObject go = GameObject.Instantiate(model, pos, Quaternion.identity) as GameObject;
			AddValues(inst, go);
			go.name = Util.ClearGUID (inst);
			GameObject parent = transform.GetComponent<MapItems>().SelectParent(inst.type);
			go.transform.parent = parent.transform;
			//transform.GetComponent<MapItems>().AddComponentData(inst.type, go);

	}
	
   // Rework this
	void AddValues(Inst inst, GameObject instGO) {
		//AreaGEometryEntity
		string instType = inst.type;
		if (instType == "Entity.AreaGeometryEntityData") {
			Debug.Log("Called Entity.AreaGeometryEntityData");
			AreaGeometryEntityData AGED = instGO.AddComponent<AreaGeometryEntityData>();
			AGED.GUID = inst.guid;
			foreach(Field field in inst.field) {
				if(field.value != null && field.value != "null") {
					if(field.name == "Components" ) {
						AGED.components = field.value;
					}
					if(field.name == "Enumeration" ) {
						AGED.enumeration = int.Parse(field.value);
					}
					if(field.name == "Name") {
						AGED.name = field.value;
					}
					if(field.name == "Height") {
						AGED.height = float.Parse(field.value);
					}
					if(field.name == "Weight") {
						AGED.weight = float.Parse(field.value);
					}
				} else if(field.reference != null) {
					if(field.name == "Next") {
						AGED.next = (field.reference);
					}
					if(field.name == "Previous") {
						AGED.previous = (field.reference);
					}
					if(field.name == "Name") {
						AGED.name = field.reference;
					}
				}
			}
			foreach(Complex complex in inst.complex) {
				if(complex.name == "Transform" && complex.value != null) {
					AGED.bc2transform = complex.value;
				}
			}
		}
		if (instType == "Entity.ReferenceObjectData") {
			Debug.Log("Called Entity.ReferenceObjectData");
			//Vector3 right = CalculateRotation(inst, "r");
			//Vector3 up = CalculateRotation(inst, "u");
			//Vector3 forward = CalculateRotation(inst, "f");
			//RotationSlave rotslave = instGO.AddComponent<RotationSlave>();
			//rotslave.forward = forward;
			//rotslave.up = up;
			//rotslave.right = right;
			Matrix4x4 matrix = Util.GenerateMatrix4x4(inst);
			MatrixHelper.SetTransformFromMatrix(instGO.transform,ref matrix);
		}
		/*if (id == 54) {
			Debug.Log("Called VehicleSpawnEntityData");
			VehicleSpawnEntityData VSED = instgo.AddComponent<VehicleSpawnEntityData>();
			VSED.GUID = inst.guid;
			foreach(BC2Array bc2array in inst.array) {
				if(bc2array.value != null || bc2array.value != "null") {
					if(bc2array.name == "Components") {
						if(bc2array.item != null) {
//							VSED.Components.Add(array.item);
						}
					}
					if(bc2array.name == "Vehicles") {
						foreach(Complex complex in bc2array.complex) {
							VSED.Vehicles.Add(complex);
							foreach(Complex subcomplex in complex.complex) {
								VSED.Vehicles[VSED.Vehicles.Count].complex.Add(subcomplex);
								foreach(Field field in subcomplex.field) {
//									subfield.add(field);
								}
							}
						}
					}
				}

				foreach (Field field in inst.field) {
					if(field.value != null || field.value != "null") {
						if(field.name == "Enumeration") {
							VSED.Enumeration = int.Parse(field.value);
						}
						if(field.name == "Name") {
							VSED.Name = field.value;
						}
						if(field.name == "Enabled") {
							VSED.Enabled = bool.Parse(field.value);
						}
						if(field.name == "Team") {
							VSED.Team = field.value;
						}
						if(field.name == "Amount") {
							VSED.Amount = int.Parse(field.value);
						}
						if(field.name == "SpawnDelay") {
							VSED.SpawnDelay = float.Parse(field.value);
						}
						if(field.name == "InitializedSpawnDelay") {
							VSED.InitializedSpawnDelay = float.Parse(field.value);
						}
						if(field.name == "AllowMulipleSpawn") {
							VSED.AllowMulipleSpawn = bool.Parse(field.value);
						}
						if(field.name == "Immortal") {
							VSED.Immortal = bool.Parse(field.value);
						}
						if(field.name == "FakeImmortal") {
							VSED.FakeImmortal = bool.Parse(field.value);
						}
						if(field.name == "Vehicle") {
							VSED.Vehicle = field.reference;
						}
						if(field.name == "TakeControlEntryIndex") {
							VSED.TakeControlEntryIndex = int.Parse(field.value);
						}
						if(field.name == "RotationYaw") {
							VSED.RotationYaw = float.Parse(field.value);
						}
						if(field.name == "RotationPitch") {
							VSED.RotationPitch = float.Parse(field.value);
						}
						if(field.name == "RotationRoll") {
							VSED.RotationRoll = float.Parse(field.value);
						}
						if(field.name == "Throttle") {
							VSED.Throttle = float.Parse(field.value);
						}
						if(field.name == "Brake") {
							VSED.Brake = float.Parse(field.value);
						}
						if(field.name == "RespawnRange") {
							VSED.RespawnRange = float.Parse(field.value);
						}
						if(field.name == "SpawnAreaRadius") {
							VSED.SpawnAreaRadius = float.Parse(field.value);
						}
						if(field.name == "TimeUntilAbandoned") {
							VSED.TimeUntilAbandoned = float.Parse(field.value);
						}
						if(field.name == "TimeUntilAbandonedIsDestroyed") {
							VSED.TimeUntilAbandonedIsDestroyed = float.Parse(field.value);
						}
						if(field.name == "BotBailWhenHealthBelow") {
							VSED.BotBailWhenHealthBelow = float.Parse(field.value);
						}
						if(field.name == "BotBailOutDelay") {
							VSED.BotBailOutDelay = float.Parse(field.value);
						}
						if(field.name == "KeepAliveRadius") {
							VSED.KeepAliveRadius = float.Parse(field.value);
						}
						if(field.name == "WreckDuration") {
							VSED.WreckDuration = float.Parse(field.value);
						}
						if(field.name == "MaxVehicles") {
							VSED.MaxVehicles = int.Parse(field.value);
						}
						if(field.name == "MaxVehiclesPerMap") {
							VSED.MaxVehiclesPerMap = int.Parse(field.value);
						}
						if(field.name == "MeshShaderSetNumber") {
							VSED.MeshShaderSetNumber = int.Parse(field.value);
						}
						if(field.name == "AutoSpawn") {
							VSED.AutoSpawn = bool.Parse(field.value);
						}
						if(field.name == "ApplyDamageToAbandondedVehicles") {
							VSED.ApplyDamageToAbandondedVehicles = bool.Parse(field.value);
						}
						if(field.name == "SnapToSurface") {
							VSED.SnapToSurface = bool.Parse(field.value);
						}
						if(field.name == "SpawnVehicleFromBeginning") {
							VSED.SpawnVehicleFromBeginning = bool.Parse(field.value);
						}
						if(field.name == "DestroyVehiclesOnDisable") {
							VSED.DestroyVehiclesOnDisable = bool.Parse(field.value);
						}
						if(field.name == "SetTeamOnSpawn") {
							VSED.SetTeamOnSpawn = bool.Parse(field.value);
						}
						if(field.name == "EffectedByImpulse") {
							VSED.EffectedByImpulse = bool.Parse(field.value);
						}
						if(field.name == "LodDistance") {
							VSED.LodDistance = float.Parse(field.value);
						}
						if(field.name == "EnterRestriction") {
							VSED.EnterRestriction = field.value;
						}
						if(field.name == "OnlySendEventForHumanPlayers") {
							VSED.OnlySendEventForHumanPlayers = bool.Parse(field.value);
						}
						if(field.name == "SendWeaponEvents") {
							VSED.SendWeaponEvents = bool.Parse(field.value);
						}
					}
				}
			}
		}*/
		if(instType == "GameSharedResources.TerrainEntityData") {
			TerrainEntityData ted = instGO.AddComponent<TerrainEntityData>();
			foreach(Field field in inst.field) {
				if(field.value != null) {
					if(field.name == "Enumeration") {
						ted.Components = field.value;
					}
					if(field.name == "Name") {
						ted.Components = field.value;
					}
					if(field.name == "Enabled") {
						ted.Components = field.value;
					}
					if(field.name == "Material") {
						ted.Components = field.value;
					}
					if(field.name == "Material") {
						ted.Components = field.value;
					}
				} else if(field.reference != null) {
					if(field.name == "TerrainAsset") {
						ted.TerrainAsset = field.reference;
					}
				}
			}
		}
		if (instType == "Physics.HavokAsset") {
			Debug.Log("Called Physics.HavokAsset");
			HavokAsset havok = instGO.AddComponent<HavokAsset>();
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

