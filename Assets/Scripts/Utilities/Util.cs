using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using BC2;
using BC2.Util.Texture;

public class Util
{

	public static MapLoad mapLoad;
	private static Dictionary<string, string> ExistingFiles = new Dictionary<string, string>();
	private static CultureInfo cultureInfo = new CultureInfo("en-US", false);

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

	public static string GetMatrixString(Transform transform)
	{
		Vector3 pos = transform.position;
		Quaternion rot = transform.rotation;

		Matrix4x4 matrix = new Matrix4x4();
		matrix.SetTRS(pos.normalized, Normalize(rot), Vector3.one);
		Matrix4x4 m = matrix;

		string posX = pos.x.ToString(cultureInfo),
			   posY = pos.y.ToString(cultureInfo),
			   posZ = pos.z.ToString(cultureInfo);

		string m02 = (m.m02).ToString(cultureInfo);
		string m10 = (m.m10 * -1).ToString(cultureInfo);
		string m00 = (m.m00 * -1).ToString(cultureInfo);
		string m21 = (m.m21).ToString(cultureInfo);
		string m11 = (m.m11).ToString(cultureInfo);
		string m01 = (m.m01).ToString(cultureInfo);
		string m22 = (m.m22).ToString(cultureInfo);
		string m12 = (m.m12).ToString(cultureInfo);
		string m20 = (m.m20 * -1).ToString(cultureInfo);

		if (m.m02.ToString() == "1") { m02 = "1.0"; } else if (m.m02.ToString() == "-1") { m02 = "-1.0"; }
		if (m.m10.ToString() == "1") { m10 = "1.0"; } else if (m.m10.ToString() == "-1") { m10 = "-1.0"; }
		if (m.m00.ToString() == "1") { m00 = "1.0"; } else if (m.m00.ToString() == "-1") { m00 = "-1.0"; }
		if (m.m21.ToString() == "1") { m21 = "1.0"; } else if (m.m21.ToString() == "-1") { m21 = "-1.0"; }
		if (m.m11.ToString() == "1") { m11 = "1.0"; } else if (m.m11.ToString() == "-1") { m11 = "-1.0"; }
		if (m.m01.ToString() == "1") { m01 = "1.0"; } else if (m.m01.ToString() == "-1") { m01 = "-1.0"; }
		if (m.m22.ToString() == "1") { m22 = "1.0"; } else if (m.m22.ToString() == "-1") { m22 = "-1.0"; }
		if (m.m12.ToString() == "1") { m12 = "1.0"; } else if (m.m12.ToString() == "-1") { m12 = "-1.0"; }
		if (m.m20.ToString() == "1") { m20 = "1.0"; } else if (m.m20.ToString() == "-1") { m20 = "-1.0"; }

		string matrixstring =
			m20 + "/" + m10 + "/" + m00 + "/*zero*/" +
			m21 + "/" + m11 + "/" + m01 + "/*zero*/" +
			m22 + "/" + m12 + "/" + m02 + "/*zero*/" +
			posZ + "/" + posY + "/" + posX + "/*zero*";

		return matrixstring;
	}


	static Quaternion Normalize(Quaternion q)
	{
		double nqx = double.Parse(q.x.ToString());
		double nqy = double.Parse(q.y.ToString());
		double nqz = double.Parse(q.z.ToString());
		double nqw = double.Parse(q.w.ToString());

		double norm2 = nqx * nqx + nqy * nqy + nqz * nqz + nqw * nqw;
		if (norm2 > Double.MaxValue)
		{
			// Handle overflow in computation of norm2
			double rmax = 1.0 / Max(Math.Abs(nqx),
				Math.Abs(nqy),
				Math.Abs(nqz),
				Math.Abs(nqw));

			nqx *= rmax;
			nqy *= rmax;
			nqz *= rmax;
			nqw *= rmax;
			norm2 = nqx * nqx + nqy * nqy + nqz * nqz + nqw * nqw;
		}
		double normInverse = 1.0 / Math.Sqrt(norm2);
		nqx *= normInverse;
		nqy *= normInverse;
		nqz *= normInverse;
		nqw *= normInverse;

		Quaternion quat = new Quaternion(float.Parse(nqx.ToString()), float.Parse(nqy.ToString()), float.Parse(nqz.ToString()), float.Parse(nqw.ToString()));
		return quat;
	}

	static double Max(double a, double b, double c, double d)
	{
		if (b > a)
			a = b;
		if (c > a)
			a = c;
		if (d > a)
			a = d;
		return a;
	}
	public static List<Inst> GetTypes(string type, Partition partition)
	{
		List<Inst> ret = new List<Inst>();

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

	public static Inst GetInst(string GUID, Partition partition)
	{
		Inst ret = null;
		GUID = GUID.ToUpper();
		if (partition.instance != null && GUID != null)
		{
			foreach (Inst inst in partition.instance)
			{
				if (inst.guid.ToUpper() == GUID.ToUpper())
				{
					ret = inst;
				}
			}
		}
		else
		{
			Util.Log("shit went wrong patition does not exist: " + GUID + " | " + partition.guid);
		}

		return ret;
	}

	public static Field GetField(string name, Inst inst)
	{
		Field ret = null;
		if (inst != null)
		{
			foreach (Field field in inst.field)
			{
				if (field.name == name)
				{
					ret = field;
				}
			}
		}
		return ret;
	}




	public static Complex GetComplex(string name, Inst inst)
	{
		Complex ret = null;
		foreach (Complex complex in inst.complex)
		{
			if (complex.name == name)
			{
				ret = complex;
			}
		}
		return ret;
	}

	public static BC2Array GetArray(string name, Inst inst)
	{
		BC2Array ret = null;
		if (inst == null)
		{
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
	public static string GetTextureType(string textureName)
	{
		string type = "";
		textureName = textureName.ToLower();

		if (textureName.Contains("detail"))
		{
			type = "_DetailMask";
		}
		if (textureName.EndsWith("_d") || textureName.EndsWith("_c"))
		{
			type += "_MainTex";
		}
		else if (textureName.EndsWith("_s"))
		{
			type += "_MetallicGlossMap";
		}
		else if (textureName.EndsWith("_n"))
		{
			type += "_BumpMap";
		}
		else if (textureName.EndsWith("_m"))
		{
			type += "_MetallicGlossMap";
		}
		return type;
	}

	public static string ClearGUID(Inst inst)
	{
		string name = "Unknown";
		foreach (Field field in inst.field)
		{
			if (field.name == "ReferencedObject")
			{

				string pattern = "/[a-zA-Z0-9]+-[a-zA-Z0-9]+-[a-zA-Z0-9]+[a-zA-Z0-9]+-[a-zA-Z0-9]+-[a-zA-Z0-9]+";
				name = field.reference;
				name = Regex.Replace(name, pattern, "");
				//Debug.Log(name);

			}
		}
		if (name == "Unknown" || name == "null" || name == null)
		{

			if (inst.type != null)
			{
				name = inst.type + " | " + inst.guid;
			}
		}
		return name;
	}

	public static GameObject GetGOByString(string GUID)
	{
		MapLoad ml = GetMapload();
		GameObject returnGO = null;
		ml.instantiatedDictionary.TryGetValue(GUID.ToUpper(), out returnGO);
		return returnGO;
	}

	public static MapLoad GetMapload()
	{
		return mapLoad;
	}
	public static int GetTerrainHeaderLength(string path)
	{ // unfinished

		return 49;


	}
	public static byte[] ReadFile(string path)
	{
		string subPath = "Resources/";
		if (FileExist(subPath + path))
		{
			return File.ReadAllBytes(subPath + path);
		}
		return null;
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
				partition.name = path;
			}
		}
		return partition;
	}

	public static bool FileExist(string path)
	{
		string result = null;
		ExistingFiles.TryGetValue(path, out result);
		if (result != null)
		{
			return true;
		}
		else if (System.IO.File.Exists(path) == true || System.IO.File.Exists("Resources/" + path) == true)
		{
			ExistingFiles.Add(path, path);
			return true;
		}
		else
		{
			return false;
		}
	}

	public static string ClearGUIDString(string name)
	{
		if (name != null)
		{
			string pattern = "/[a-zA-Z0-9]+-[a-zA-Z0-9]+-[a-zA-Z0-9]+[a-zA-Z0-9]+-[a-zA-Z0-9]+-[a-zA-Z0-9]+";
			name = Regex.Replace(name, pattern, "");
			return name;
		}
		else
		{
			return null;
		}
	}

	public static bool IsObject(Inst inst)
	{
		if (GetPosition(inst) == Vector3.zero)
		{
			return false;
		}
		else
		{
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
	public static Quaternion GetRotation(Inst inst)
	{
		Matrix4x4 matrix = Util.GenerateMatrix4x4(inst);
		Quaternion newQuat = MatrixHelper.QuatFromMatrix(matrix);
		return newQuat;
	}

	public static Quaternion GetRotationFromString(string rot)
	{
		Matrix4x4 matrix = Util.GenerateMatrix4x4String(rot);
		Quaternion newQuat = MatrixHelper.QuatFromMatrix(matrix);
		return newQuat;
	}


	public static Vector3 GetPosition(Inst inst)
	{
		Vector3 pos = Vector3.zero;
		string bc2pos = null;

		if (Util.GetComplex("Transform", inst) != null || Util.GetComplex("Position", inst) != null)
		{
			//Util.Log(inst.guid);
			if (Util.GetComplex("Transform", inst) != null && Util.GetComplex("Transform", inst).value != null)
			{
				bc2pos = Util.GetComplex("Transform", inst).value;

			}
			else if (Util.GetComplex("Position", inst) != null && Util.GetComplex("Position", inst).value != null)
			{
				bc2pos = Util.GetComplex("Position", inst).value;
			}

			if (bc2pos != null)
			{
				//Debug.Log("pos val for " + inst.guid + " | " + inst.complex.value);
				string coordiantes = bc2pos;
				string[] coords = coordiantes.Split('/');
				int numcoords = coords.Length;
				float z = float.Parse(coords[(numcoords - 4)], cultureInfo);
				float y = float.Parse(coords[(numcoords - 3)], cultureInfo);
				float x = float.Parse(coords[(numcoords - 2)], cultureInfo);
				pos = new Vector3(x, y, z);
			}

		}
		else
		{
			pos = Vector3.zero;
		}
		return pos;
	}

	public static Vector3 GetPositionFromString(string bc2pos)
	{
		Vector3 pos = Vector3.zero;
		if (bc2pos != null)
		{
			//Debug.Log("pos val for " + inst.guid + " | " + inst.complex.value);
			string coordiantes = bc2pos;
			string[] coords = coordiantes.Split('/');
			int numcoords = coords.Length;
			float z = float.Parse(coords[(numcoords - 4)], cultureInfo);
			float y = float.Parse(coords[(numcoords - 3)], cultureInfo);
			float x = float.Parse(coords[(numcoords - 2)], cultureInfo);
			pos = new Vector3(x, y, z);
		}
		return pos;
	}


	public static Matrix4x4 GenerateMatrix4x4(Inst inst)
	{
		Matrix4x4 matrix = new Matrix4x4();
		string bc2rot = null;
		foreach (Complex complex in inst.complex)
		{
			if (complex.name == "Transform")
			{
				bc2rot = complex.value;
			}
		}
		if (IsObject(inst) && bc2rot != null)
		{
			string coordiantes = bc2rot;
			string[] coords = coordiantes.Split('/');
			int numcoords = coords.Length;
			if (numcoords > 3)
			{
				float rz = (float.Parse(coords[0], cultureInfo) * -1);
				float ry = (float.Parse(coords[1], cultureInfo) * -1);
				float rx = (float.Parse(coords[2], cultureInfo) * -1);

				float uz = (float.Parse(coords[4], cultureInfo));
				float uy = (float.Parse(coords[5], cultureInfo));
				float ux = (float.Parse(coords[6], cultureInfo));

				float fz = (float.Parse(coords[8], cultureInfo));
				float fy = (float.Parse(coords[9], cultureInfo));
				float fx = (float.Parse(coords[10], cultureInfo));

				float px = (float.Parse(coords[12], cultureInfo));
				float py = (float.Parse(coords[13], cultureInfo));
				float pz = (float.Parse(coords[14], cultureInfo));

				matrix.SetColumn(0, new Vector4(rx, ry, rz, 0));
				matrix.SetColumn(1, new Vector4(ux, uy, uz, 0));
				matrix.SetColumn(2, new Vector4(fx, fy, fz, 0));
				matrix.SetColumn(3, new Vector4(px, py, pz, 0));
			}
		}
		return matrix;
	}

	public static Matrix4x4 GenerateMatrix4x4String(string bc2rot)
	{
		Matrix4x4 matrix = new Matrix4x4();
		if (bc2rot != null)
		{
			string coordiantes = bc2rot;
			string[] coords = coordiantes.Split('/');
			int numcoords = coords.Length;
			if (numcoords > 3)
			{
				float rz = (float.Parse(coords[0], cultureInfo) * -1);
				float ry = (float.Parse(coords[1], cultureInfo) * -1);
				float rx = (float.Parse(coords[2], cultureInfo) * -1);

				float uz = (float.Parse(coords[4], cultureInfo));
				float uy = (float.Parse(coords[5], cultureInfo));
				float ux = (float.Parse(coords[6], cultureInfo));

				float fz = (float.Parse(coords[8], cultureInfo));
				float fy = (float.Parse(coords[9], cultureInfo));
				float fx = (float.Parse(coords[10], cultureInfo));

				float px = (float.Parse(coords[12], cultureInfo));
				float py = (float.Parse(coords[13], cultureInfo));
				float pz = (float.Parse(coords[14], cultureInfo));

				matrix.SetColumn(0, new Vector4(rx, ry, rz, 0));
				matrix.SetColumn(1, new Vector4(ux, uy, uz, 0));
				matrix.SetColumn(2, new Vector4(fx, fy, fz, 0));
				matrix.SetColumn(3, new Vector4(px, py, pz, 0));
			}
		}
		return matrix;
	}


	public static Texture2D LoadiTexture(string path)
	{
		if (!FileExist("Resources/" + path))
		{
			return null;
		}
		Texture2D ret = iTexture.LoadTexture("Resources/" + path);
		return ret;
	}


	public static void Log(string log)
	{
		UnityEngine.Debug.Log(log);
	}

	public static string ByteArrayToString(byte[] ba)
	{
		StringBuilder hex = new StringBuilder(ba.Length * 2);
		foreach (byte b in ba)
			hex.AppendFormat("{0:x2}", b);
		return hex.ToString();
	}

}
