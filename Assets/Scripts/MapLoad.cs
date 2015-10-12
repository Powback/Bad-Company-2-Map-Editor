using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Text.RegularExpressions;
using BC2;
using System.Reflection;

public class MapLoad : MonoBehaviour 
{

	public string mapName;
	public GameObject placeholder;
    public GameObject empty;
	public Material material_white;
    public Component waterScript;
    public Material waterMaterial;
	public Partition partition;
	int i;
	public string SaveAs = "TestMap";
    public bool Save;
    public bool saved;
	public List<InstGameObject> InstGameObjects;

    void Start()
    {
        partition = Util.LoadPartition("levels/" + mapName);
        foreach(Inst inst in partition.instance)
        {
            GenerateItem(inst);
        }
    }

    void FixedUpdate()
    {
        if(Save && saved == false)
        {
            saved = true;
            MapContainer.Save(partition, "Assets/"+ SaveAs + ".xml");
        }
    }


    void GenerateItem(Inst inst) {

		Vector3 pos = Util.CalculatePosition (inst);
		GameObject model;
		string actualmodelname = Util.ClearGUID(inst) + "_lod0_data";
		string actualmodelname2 = Util.ClearGUID(inst) + "_mesh_lod0_data";

		if(Resources.Load(actualmodelname) != null) {
			model = Resources.Load(actualmodelname) as GameObject;
        } else if(Resources.Load(actualmodelname2) != null) {
			model = Resources.Load(actualmodelname2) as GameObject;
		} else {
            if (inst.type == "Terrain.TerrainSplineData")
            {
                model = empty.gameObject;
            }
            else
            {
                model = placeholder.gameObject;
            }
		}
		GameObject go = GameObject.Instantiate(model, pos, Quaternion.identity) as GameObject;
		
		go.name = Util.ClearGUID (inst);
		GameObject parent = transform.GetComponent<MapItems>().SelectParent(inst.type);
		go.transform.parent = parent.transform;
		
		Matrix4x4 matrix = Util.GenerateMatrix4x4 (inst);
		Quaternion newQuat = MatrixHelper.QuatFromMatrix(matrix);
		//Quaternion newQuat = go.transform.localRotation;
		//newQuat.x *= -1;
		//newQuat.z *= -1f;
		go.transform.rotation = newQuat;
        BC2Instance instance = go.AddComponent<BC2Instance>();
        instance.instance = inst;
		instance.id = i;
		instance.mapLoad = this;
		InstGameObject instGameObject = new InstGameObject ();
		instGameObject.GUID = inst.guid;
		instGameObject.GO = go;
		InstGameObjects.Add (instGameObject);
		i++;
	}
}
