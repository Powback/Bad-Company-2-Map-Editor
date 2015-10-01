using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using BC2;

public class Util  {

	public static string ClearGUID(Inst inst) {
		string name = "Unknown";
		foreach(Field field in inst.field) {
			if(IsObject(inst) && field.reference != null && field.reference != "null") {
				string pattern = "/[0-9a-z]+-[0-9a-z]+-[0-9a-z]+-[0-9a-z]+-[0-9a-z]+";
				string pattern2 = "_entity";
				string pattern3 = "_asset";
				name = field.reference;
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
    public static Inst SelectByGUID(string GUID, Partition partition)
    {
        Inst retInst = new Inst();
        foreach(Inst inst in partition.instance)
        {
            if(inst.guid == GUID)
            {
                retInst = inst;
            }
        }
        return retInst;
    }

    public static Partition LoadPartition(string path)
    {
        string subPath = "Assets/Resources/";
        string extension = ".xml";
        Partition partition = new Partition();
        if(FileExist(subPath + path + extension))
        {
            var InstanceCollection = MapContainer.Load(subPath + path + extension);
            if(InstanceCollection != null)
            {
                partition = InstanceCollection;
            }
        }
        return partition;
    }

    public static bool FileExist(string path)
    {
       if( System.IO.File.Exists(path) == true)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public static string ClearGUIDString(string name)
    {
        string pattern = "/[0-9a-z]+-[0-9a-z]+-[0-9a-z]+-[0-9a-z]+-[0-9a-z]+";
        string pattern2 = "_entity";
        string pattern3 = "_asset";
        name = Regex.Replace(name, pattern, "");
        name = Regex.Replace(name, pattern2, "");
        name = Regex.Replace(name, pattern3, "");
        return name;
    }

    public static bool IsObject(Inst inst) {
		if(CalculatePosition(inst) == Vector3.zero) {
			return false;
		} else {
			return true;
		}
	}

	public static Vector3 CalculatePosition(Inst inst) {
		Vector3 pos = Vector3.zero;
		string bc2pos = null;
		
		if (inst.complex != null) {
			foreach(Complex complex in inst.complex) {
				if(complex.name == "Transform") {
					bc2pos = complex.value;
				}
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
				matrix.SetColumn(2, new Vector4(fx,ry,fz,0));
				matrix.SetColumn(3, new Vector4(px,py,pz,0));
			}
		} 
		return matrix;
	}
}
