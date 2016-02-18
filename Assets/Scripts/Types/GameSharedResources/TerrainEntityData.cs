using UnityEngine;
using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BC2;


public class TerrainEntityData : MonoBehaviour {

    public Partition partition;
    public Inst instance;
    string terrainLoc;
    int fullres;

    void Start() {
        CheckTerrain("00", 0, 0);
        GenerateOuterTerrain(terrainLoc);
    }

	void CheckTerrain(string id, int res, int terrainCount)
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

			LoadTerrain(terrainLocation, terrainRes, terrainHeight, id, terrainCount);

        }
    }


	int TerrainHeight(string location)
	{
		Inst heightFieldData = Util.GetTypes("Terrain.TerrainHeightfieldData", partition)[0];
		int height = Mathf.CeilToInt(float.Parse(Util.GetField("SizeY", heightFieldData).value));
		return height;
	}



	int TerrainRes(string location, string guid)
	{

		Inst terrainInst = Util.GetInst(guid, partition);
		int res = Mathf.CeilToInt(float.Parse(Util.GetField("SizeXZ", terrainInst).value));
		return res;
	}


	void LoadTerrain(string location, int res, int height, string id, int terrainCount)
    {
		if (Util.FileExist("Resources/"+location + ".heightfield-0" + id +".terrainheightfield")) {
			GameObject terrain = GenerateTerrain("Resources/"+location + ".heightfield-0" + id +".terrainheightfield", res, height, id, terrainCount);
            terrain.transform.parent = Util.GetMapload().terrainHolder.transform;
        }
    }
		

	GameObject GenerateTerrain(string location, int res, int height, string id, int terrainCount)
    {
       
        GameObject terrain = (GameObject)Instantiate(Util.GetMapload().empty, Vector3.zero, Quaternion.identity);
        terrain.name = location + id;
        
		GenerateTerrainMesh(terrain, location, res, height, fullres);
        
        float terrainPos = ((res * -1) / 2);
        if(res < 512)
        {
			terrain.transform.position = OuterTerrainPos(512, id, terrainCount);
        } else
        {
            terrain.transform.position = new Vector3(terrainPos, 0, terrainPos);
        }
        Util.Log("Shit should be done loading by now");

        return terrain;
    }




	Vector3 OuterTerrainPos(int res, string id, int terrainCount)
    {
		if (terrainCount == 12) {
			List<Vector3> pos12 = new List<Vector3> ();


			pos12.Add (new Vector3 (-1, 0, -1)); // useless
			pos12.Add (new Vector3 (-2, 0, -2));
			pos12.Add (new Vector3 (-2, 0, -1));
			pos12.Add (new Vector3 (-1, 0, -2));
			pos12.Add (new Vector3 (-2, 0, 0));
			pos12.Add (new Vector3 (-2, 0, 1));
			pos12.Add (new Vector3 (-1, 0, 1));
			pos12.Add (new Vector3 (0, 0, -2));
			pos12.Add (new Vector3 (1, 0, -2));
			pos12.Add (new Vector3 (1, 0, -1));
			pos12.Add (new Vector3 (0, 0, 1));
			pos12.Add (new Vector3 (1, 0, 0));
			pos12.Add (new Vector3 (1, 0, 1));
			int i = int.Parse (id);
			return pos12 [i] * (fullres / 2);
		} else {
			return new Vector3 (1024, 0, 1024);
		}
    }
    void GenerateOuterTerrain(string location)
    {
		int numOuterTerrains = 0;
		if (Util.FileExist ("Resources/" + location + ".heightfield-048.terrainheightfield")) {
			numOuterTerrains = 48;
		} else if (Util.FileExist ("Resources/" + location + ".heightfield-036.terrainheightfield")) {
			numOuterTerrains = 36;
		} else if (Util.FileExist ("Resources/" + location + ".heightfield-024.terrainheightfield")) {
			numOuterTerrains = 24;
		} else if (Util.FileExist ("Resources/" + location + ".heightfield-012.terrainheightfield")) {
			numOuterTerrains = 12;
		} else {
			numOuterTerrains = 0;
		}
		 
		for (int i = 0; i < numOuterTerrains; i++)
        {
			string prefix = "0"; // make terrain0-9 into terrain001-09;
            if(i + 1 > 9)
            {
                prefix = "";
            }
            int terrainID = i + 1;
            string id = prefix + terrainID.ToString();
            if(Util.FileExist("Resources/" + location + ".heightfield-0" + id + ".terrainheightfield"))
            {
                int size = Util.GetFilesize("Resources/" + location + ".heightfield-0" + id + ".terrainheightfield");
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

				CheckTerrain(id, res, numOuterTerrains);
            }
            


        }
    }
	public static int ToBigEndian(byte[] buf, int i) {
		return(buf [i] << 24) | buf [i + 1] << 16 | buf [i + 2] << 8 | buf [i + 3];
	}

	public static void GenerateTerrainMesh(GameObject terrainGO, string path, int sizeorg, int height, int fullsize)
	{
		int size = sizeorg + 1;

		byte[] orgBuffer = new byte[(size * size) * 2];
		byte[] buffer = new byte[(size * size) * 2];
		using (BinaryReader reader = new BinaryReader (File.Open (path, FileMode.Open, FileAccess.Read))) {
			
			int headerLength = Util.GetTerrainHeaderLength(path);



			reader.ReadBytes (headerLength);

			int lineoffset = 0;
			int terrainOffset = 0;

			while (lineoffset < sizeorg) { // Resize the image to (power of 2) + 1
				int fileOffset = 0;

				while (fileOffset < sizeorg * 2) { // read one line
					orgBuffer [terrainOffset] = reader.ReadByte ();
					terrainOffset++;
					fileOffset++;
				}
				orgBuffer [terrainOffset + 1] = buffer [terrainOffset - 1];
				orgBuffer [terrainOffset + 2] = buffer [terrainOffset];
				terrainOffset += 2;
				lineoffset++;
			}

			int lastLine = 0;

			while (lastLine < (size * 2)) {
				int offset = (((size * size) * 2) - (size * 2));
				orgBuffer [offset + lastLine] = orgBuffer [(offset - (size * 2)) + lastLine];
				lastLine++;
			}


			for (int y = 0; y < (size * 2) ; y++) {
				for (int x = 0; x < (size  *2 ); x++) {
					
					int newOffset = (x * size) + y;
					int orgOffset = (y * size) + x;

					buffer [newOffset] = orgBuffer [orgOffset]; 
					buffer [newOffset+1] = orgBuffer [orgOffset +1]; 
					x++;
				}
				y++;
			}
				



		

			reader.Close();
		}


		Terrain terrain = terrainGO.AddComponent<Terrain>();
		terrainGO.AddComponent<TerrainCollider>();
		TerrainData terrainData = new TerrainData();
		terrainData.heightmapResolution = size;
		if(size < 512)
		{
			int otSize = (fullsize) / 2;
			terrainData.size = new Vector3(otSize, height, otSize);
		} else
		{
			terrainData.size = new Vector3(size - 1, height, size - 1);
		}
		
		terrain.terrainData = terrainData;
		
		int heightmapWidth = terrain.terrainData.heightmapWidth;
		int heightmapHeight = terrain.terrainData.heightmapHeight;
		
		float[,] heights = new float[heightmapHeight, heightmapWidth];
		float num3 = 1.525879E-05f;
		for (int i = 0; i < heightmapHeight; i++)
		{
			for (int j = 0; j < heightmapWidth; j++)
			{
				int num6 = Mathf.Clamp(j, 0, size - 1) + (Mathf.Clamp(i, 0, size - 1) * size);
//				byte num7 = buffer[num6 * 2];
//				buffer[num6 * 2] = buffer[(num6 * 2) + 1];
//				buffer[(num6 * 2) + 1] = num7;

				float num9 = System.BitConverter.ToUInt16(buffer, num6 * 2) * num3;
				heights[i, j] = num9;
				
			}
		}
		terrain.terrainData.SetHeights(0, 0, heights);
		terrain.heightmapPixelError = 1;
		
	}
}


