using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using BC2;

public class TerrainEntityData : MonoBehaviour {

    public Partition partition;
    public Inst instance;
    string terrainLoc;
    int fullres;

    void Start() {
        CheckTerrain("00", 0);
        GenerateOuterTerrain(terrainLoc);
    }

    void CheckTerrain(string id, int res)
    {
        instance = GetComponent<BC2Instance>().instance;
        string terrainLocation = Util.GetField("TerrainAsset", instance).reference;
        if (terrainLocation == "" || terrainLocation == null)
        {
            Util.Log("TerrainAsset is missing from " + instance.guid);
        } else
        {
           
            string guid = Util.GetGuid(terrainLocation);
            terrainLocation = Util.ClearGUIDString(terrainLocation);
            terrainLoc = terrainLocation;
            partition = Util.LoadPartition(terrainLocation);
            int terrainRes;
            if (res == 0)
            {
                terrainRes = TerrainRes(terrainLocation, guid);
                fullres = terrainRes;
            } else
            {
                terrainRes = res;
            }
            
            int terrainHeight = TerrainHeight(terrainLocation);

            LoadTerrain(terrainLocation, terrainRes, terrainHeight, id);

        }
    }
    void LoadTerrain(string location, int res, int height, string id)
    {
        if (Util.FileExist("Assets/Resources/_Converted/" + location + id + ".raw")) {
            GameObject terrain = GenerateTerrain(location, res, height, id);
            terrain.transform.parent = Util.GetMapload().terrainHolder.transform;
        }
        else
        {
            Util.Log("Couldn't find terrain " + location + " , attempting to convert it.");
            string convertlocation = location + ".heightfield-0" + id +".terrainheightfield";
            ConvertTerrain(location, res, id);
            //GameObject terrain = GenerateTerrain(location, res, height, id);
            //terrain.transform.parent = Util.GetMapload().terrainHolder.transform;
        }
    }

    void ConvertTerrain(string location, int res, string id)
    {
        if (Util.FileExist("Assets/Resources/" + location + ".heightfield-0" + id + ".terrainheightfield"))
        {
            Util.AddTempFile("location", location);
            string resolution = res.ToString();
            Util.AddTempFile("res", resolution);
            Util.AddTempFile("id", id); // used for outer terrain mostly.
            StartConvert();

        }
    }

    GameObject GenerateTerrain(string location, int res, int height, string id)
    {
       
        GameObject terrain = (GameObject)Instantiate(Util.GetMapload().empty, Vector3.zero, Quaternion.identity);
        terrain.name = location + id;
        if (Util.FileExist("Assets/Resources/_Converted/" + location + id + ".raw"))
        {
            Util.Log("Trying to load " + location);
            Util.GenerateTerrain(terrain, "Assets/Resources/_Converted/" + location+ id + ".raw", res, height, fullres);
           

        }
        else
        {
            Util.Log("Couldn't create " + location);
        }
        float terrainPos = ((res * -1) / 2);
        if(res < 512)
        {
            terrain.transform.position = OuterTerrainPos(512, id);
        } else
        {
            terrain.transform.position = new Vector3(terrainPos, 0, terrainPos);
        }
        Util.Log("Shit should be done loading by now");

        return terrain;
    }


    int TerrainHeight(string location)
    {
        Inst heightFieldData = Util.GetType("Terrain.TerrainHeightfieldData", partition);
        int height = Mathf.CeilToInt(float.Parse(Util.GetField("SizeY", heightFieldData).value));
        return height;
    }



    int TerrainRes(string location, string guid)
    {

        Inst terrainInst = Util.GetInst(guid, partition);
        int res = Mathf.CeilToInt(float.Parse(Util.GetField("SizeXZ", terrainInst).value));
        return res;
    }



    Vector3 OuterTerrainPos(int res, string id)
    {
        List<Vector3> pos12 = new List<Vector3>();


        pos12.Add(new Vector3(-1, 0, -1)); // useless
        pos12.Add(new Vector3(-2, 0, -2));
        pos12.Add(new Vector3(-2, 0, -1));
        pos12.Add(new Vector3(-1, 0, -2));
        pos12.Add(new Vector3(-2, 0, 0));
        pos12.Add(new Vector3(-2, 0, 1));
        pos12.Add(new Vector3(-1, 0, 1));
        pos12.Add(new Vector3(0, 0, -2));
        pos12.Add(new Vector3(1, 0, -2));
        pos12.Add(new Vector3(1, 0, -1));
        pos12.Add(new Vector3(0, 0, 1));
        pos12.Add(new Vector3(1, 0, 0));
        pos12.Add(new Vector3(1, 0, 1));
        int i = int.Parse(id);
            return pos12[i] * (fullres / 2);
        
    }
    void GenerateOuterTerrain(string location)
    {

        for (int i = 0; i < 12; i++)
        {
            string prefix = "0";
            if(i + 1 > 9)
            {
                prefix = "";
            }
            int terrainID = i + 1;
            string id = prefix + terrainID.ToString();
            if(Util.FileExist("Assets/Resources/" + location + ".heightfield-0" + id + ".terrainheightfield"))
            {
                int size = Util.GetFilesize("Assets/Resources/" + location + ".heightfield-0" + id + ".terrainheightfield");
                int res = 0;
                if (size == 132485)
                {
                    res = 256;
                }
                else if (size == 33157)
                {
                    res = 124;
                }
                else if (size == 8325)
                {
                    res = 64;
                }
                else
                {
                    Util.Log("Couldn't find the correct res for " + location + id);
                }

                CheckTerrain(id, res);
            }
            


        }
    }


    // TODO: Fix the shitty hardcoded script.
    void StartConvert() {
        Process p = new Process();
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;

        p.StartInfo.FileName = "open";
        p.StartInfo.Arguments = "TerrainConvertMac.command";
        p.StartInfo.WorkingDirectory = "Tools";
        p.StartInfo.CreateNoWindow = false;

        p.Start();
    }


}


