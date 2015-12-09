using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using BC2;

public class Util {

	public static MapLoad mapLoad;
    public static Inst GetType(string type, Partition partition)
    {
        Inst ret = null;
        if (partition.instance != null)
        {
            foreach (Inst inst in partition.instance)
            {
                if (inst.type == type)
                {
                    ret = inst;
                }
            }
        }
        return ret;
    }

	public static List<Inst> GetTypes(string type, Partition partition)
	{
		List<Inst> ret = new List<Inst> ();

		if (partition.instance != null)
		{
			foreach (Inst inst in partition.instance)
			{
				if (inst.type == type)
				{
					ret.Add(inst);
				}
			}
		}
		return ret;
	}

	public static Inst GetInst(string GUID, Partition partition) {
		Inst ret = null;
		GUID = GUID.ToUpper();
		if(partition.instance != null)
        {
            foreach (Inst inst in partition.instance)
            {
                if (inst.guid.ToUpper() == GUID)
                {
                    ret = inst;
                }
            }
        } else
        {
            Util.Log("shit went wrong patition does not exist: " + GUID);
        }
       
        return ret;
    }

    public static Field GetField(string name, Inst inst) {
        Field ret = null;
        if (inst != null) {
            foreach (Field field in inst.field) {
                if (field.name == name) {
                    ret = field;
                }
            }
        }
        return ret;
    }


	
	
	public static Complex GetComplex(string name, Inst inst) {
		Complex ret = null;
        foreach (Complex complex in inst.complex) {
            if (complex.name == name) {
                ret = complex;
            }
        }
        return ret;
    }

    public static BC2Array GetArray(string name, Inst inst) {
        BC2Array ret = null;
        if (inst == null) {
            Debug.Log("Didn't find shit like inst");
        }
        if (inst.array != null)
        {
            foreach (BC2Array array in inst.array)
            {
                if (array.name == name)
                {
                    ret = array;
                }
            }
        }
        return ret;
    }

    public static string ClearGUID(Inst inst) {
        string name = "Unknown";
        foreach (Field field in inst.field) {
            if (field.name == "ReferencedObject") {

                string pattern = "/[a-zA-Z0-9]+-[a-zA-Z0-9]+-[a-zA-Z0-9]+[a-zA-Z0-9]+-[a-zA-Z0-9]+-[a-zA-Z0-9]+";
                name = field.reference;
                name = Regex.Replace(name, pattern, "");
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

    public static GameObject GetGOByString(string GUID) {
        MapLoad ml = GetMapload();
        GameObject returnGO = null;
		ml.instantiatedDictionary.TryGetValue(GUID, out returnGO);
        return returnGO;
    }

    public static MapLoad GetMapload() {
		return mapLoad;
    }

    public static Partition LoadPartition(string path)
    {
        string subPath = "Resources/";
        string extension = ".xml";
        Partition partition = new Partition();
        if (FileExist(subPath + path + extension))
        {
            var InstanceCollection = MapContainer.Load(subPath + path + extension);
            if (InstanceCollection != null)
            {
                partition = InstanceCollection;
            }
        }
        return partition;
    }

    public static bool FileExist(string path)
    {
		if( System.IO.File.Exists(path) == true || System.IO.File.Exists("Resources/" + path) == true || System.IO.File.Exists(path + ".obj") == true || System.IO.File.Exists("Resources/" +path + ".obj") == true) {
            return true;
		} else {
            return false;
        }
    }

    public static string ClearGUIDString(string name)
    {
        string pattern = "/[a-zA-Z0-9]+-[a-zA-Z0-9]+-[a-zA-Z0-9]+[a-zA-Z0-9]+-[a-zA-Z0-9]+-[a-zA-Z0-9]+";
        name = Regex.Replace(name, pattern, "");
        return name;
    }

    public static bool IsObject(Inst inst) {
		if(GetPosition(inst) == Vector3.zero) {
			return false;
		} else {
			return true;
		}
	}

    public static int GetFilesize(string path)
    {
        FileInfo fi = new FileInfo(path);
        string size = fi.Length.ToString();
        int ret = int.Parse(size);
        return ret;
    }

    public static string GetGuid(string name)
    {
        string pattern = "[a-zA-Z0-9]+-[a-zA-Z0-9]+-[a-zA-Z0-9]+[a-zA-Z0-9]+-[a-zA-Z0-9]+-[a-zA-Z0-9]+";
        return Regex.Match(name, pattern).Value;
    }
	public static Quaternion GetRotation(Inst inst) {
		Matrix4x4 matrix = Util.GenerateMatrix4x4 (inst);
		Quaternion newQuat = MatrixHelper.QuatFromMatrix(matrix);
		return newQuat;
	}


	public static Vector3 GetPosition(Inst inst) {
		Vector3 pos = Vector3.zero;
		string bc2pos = null;
		
		if (Util.GetComplex("Transform", inst) != null || Util.GetComplex("Position", inst) != null) {
            //Util.Log(inst.guid);
            if(Util.GetComplex("Transform", inst) != null && Util.GetComplex("Transform", inst).value != null)
            {
                bc2pos = Util.GetComplex("Transform", inst).value;

            } else if(Util.GetComplex("Position", inst) != null && Util.GetComplex("Position", inst).value != null) { 
                bc2pos = Util.GetComplex("Position", inst).value;
			}

			if (bc2pos != null) {
				//Debug.Log("pos val for " + inst.guid + " | " + inst.complex.value);
				string coordiantes = bc2pos;
				string[] coords = coordiantes.Split ('/');
				int numcoords = coords.Length;
				float z = float.Parse (coords [(numcoords - 4)]);
				float y = float.Parse (coords [(numcoords - 3)]);
				float x = float.Parse (coords [(numcoords - 2)]);
				pos = new Vector3 (x, y, z);
			}
			
		} else {
			pos = Vector3.zero;
		}
		return pos;
	}

	public static Vector3 GetPositionFromString(string bc2pos) {
		Vector3 pos = Vector3.zero;
		if (bc2pos != null) {
			//Debug.Log("pos val for " + inst.guid + " | " + inst.complex.value);
			string coordiantes = bc2pos;
			string[] coords = coordiantes.Split ('/');
			int numcoords = coords.Length;
			float z = float.Parse (coords [(numcoords - 4)]);
			float y = float.Parse (coords [(numcoords - 3)]);
			float x = float.Parse (coords [(numcoords - 2)]);
			pos = new Vector3 (x, y, z);
		}
		return pos;
	}


	public static Matrix4x4 GenerateMatrix4x4(Inst inst) {
		Matrix4x4 matrix = new Matrix4x4();
		string bc2rot = null;
		foreach(Complex complex in inst.complex) {
			if(complex.name == "Transform") {
				bc2rot = complex.value;
			}
		}
		if (IsObject(inst) && bc2rot != null) {
			string coordiantes = bc2rot;
			string[] coords = coordiantes.Split ('/');
			int numcoords = coords.Length;
			if(numcoords > 3) { 
				float rz = (float.Parse (coords [0]) * -1);
				float ry = (float.Parse (coords [1]) * -1);
				float rx = (float.Parse (coords [2]) * -1);
				
				float uz = (float.Parse (coords [4]));
				float uy = (float.Parse (coords [5]));
				float ux = (float.Parse (coords [6]));
				
				float fz = (float.Parse (coords [8]));
				float fy = (float.Parse (coords [9]));
				float fx = (float.Parse (coords [10]));
				
				float px = (float.Parse (coords [12]));
				float py = (float.Parse (coords [13]));
				float pz = (float.Parse (coords [14]));
				
				matrix.SetColumn(0, new Vector4(rx,ry,rz,0));
				matrix.SetColumn(1, new Vector4(ux,uy,uz,0));
				matrix.SetColumn(2, new Vector4(fx,fy,fz,0));
				matrix.SetColumn(3, new Vector4(px,py,pz,0));
			}
		} 
		return matrix;
	}

	public static Matrix4x4 GenerateMatrix4x4String(string bc2rot) {
		Matrix4x4 matrix = new Matrix4x4();
		if (bc2rot != null) {
			string coordiantes = bc2rot;
			string[] coords = coordiantes.Split ('/');
			int numcoords = coords.Length;
			if(numcoords > 3) { 
				float rz = (float.Parse (coords [0]) * -1);
				float ry = (float.Parse (coords [1]) * -1);
				float rx = (float.Parse (coords [2]) * -1);
				
				float uz = (float.Parse (coords [4]));
				float uy = (float.Parse (coords [5]));
				float ux = (float.Parse (coords [6]));
				
				float fz = (float.Parse (coords [8]));
				float fy = (float.Parse (coords [9]));
				float fx = (float.Parse (coords [10]));
				
				float px = (float.Parse (coords [12]));
				float py = (float.Parse (coords [13]));
				float pz = (float.Parse (coords [14]));
				
				matrix.SetColumn(0, new Vector4(rx,ry,rz,0));
				matrix.SetColumn(1, new Vector4(ux,uy,uz,0));
				matrix.SetColumn(2, new Vector4(fx,fy,fz,0));
				matrix.SetColumn(3, new Vector4(px,py,pz,0));
			}
		} 
		return matrix;
	}

    public static void Log(string log)
    {
        UnityEngine.Debug.Log(log);
    }

    public static void AddTempFile(string type, string text)
    {
        if (text != null)
        {

            string curResLocation   = "Tools/Temp/CurRes.txt";
            string newResLocation   = "Tools/Temp/NewRes.txt";
            string terrainLocation  = "Tools/Temp/TerrainLocation.txt";
            string idLocation       = "Tools/Temp/id.txt";

            if (type == "res")
            {
                int res = int.Parse(text);
                int newRes = res + 1;

                File.WriteAllText(curResLocation, res + "x" + res);
                File.WriteAllText(newResLocation, newRes + "x" + newRes);
            } else if( type == "location")
            {
                File.WriteAllText(terrainLocation, text);
            } else if( type == "id")
            {
                File.WriteAllText(idLocation, text);
            }
        }
        else
        {
            UnityEngine.Debug.Log("Temp File add failed. No values passed");
        }
    }

}
