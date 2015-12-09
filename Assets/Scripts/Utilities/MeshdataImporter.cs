using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;


public class BC2Mesh {
	public string path;
	public List<string> subMeshNames = new List<string> ();
	public List<Mesh> subMesh = new List<Mesh> ();
}


public class MeshDataImporter {
	
	public string path1;
	public string path2;
	public bool float32;
	public static int uvOffset;
	private static BinaryReader stream;

	
	public static BC2Mesh LoadMesh(string loc) {
		BC2Mesh bc2mesh = new BC2Mesh ();
		List<Mesh> subsetMesh = new List<Mesh> ();
		List<string> subsetNames = new List<string> ();

		var s_Data = File.ReadAllBytes (loc);
		using (var s_Reader = new BinaryReader(new MemoryStream(s_Data))) {
			MeshData md = new MeshData();
			
			Debug.Log ("Converting" + loc);
			stream = s_Reader;
			//md.useFloat = float32;
			md.Init ();
			int i = 0;
			
			
			List<Vector3> v3 = new List<Vector3>();
			v3 = md.subset[0].GetV3(0);
			
			int v3int = 0;
			float v3float = 0;
			while(v3int < v3.Count()) {
				v3float += v3[v3int].x;
				v3int ++;
			}
			if(v3float > 100000f || v3float < -100000f) {
				md= new MeshData();
				s_Reader.BaseStream.Seek(0,0);
				md.useFloat = true;
				md.Init();
				Debug.Log("32 bit float");
				
			}
			
			
			while (i < md.subset.Count) {
				
				if(!md.subset[i].name.Contains("ZOnly")) {
					Mesh me = new Mesh ();
					Vector3[] verts  = null;
					verts = md.subset[i].GetV3(0).ToArray();
					int[] tris = md.subset[i].GetIndices().ToArray();

					if(md.subset[i].name.Contains("Sewer")) {
						tris = tris.Reverse().ToArray();
						 //verts = md.subset[i].GetV3Inverted(0).ToArray();

					}


					;
					Vector2[] uv = null;
					if(uvOffset != 0) {
						md.subset[i].uvOffset = uvOffset;
					} else {
						md.subset[i].getV2offfsets();
					}
					
					List<Vector2> tempv2 = new List<Vector2>();
					tempv2 = md.subset[i].getV2();
					if(tempv2.Count != 0) {
						
						uv = tempv2.ToArray();
					}
					me.vertices = verts;
					me.triangles = tris;
					me.uv = uv;
					subsetNames.Add(md.subset[i].name);
					subsetMesh.Add(me);
				}
				i++;
			}
			
			
			
		}
		bc2mesh.path = loc;
		bc2mesh.subMesh = subsetMesh;
		bc2mesh.subMeshNames = subsetNames;
		return bc2mesh;
		
	}
	
	
	public static string String()
	{
		string str = "";
		char ch;
		while ((int)(ch = stream.ReadChar()) != 0)
			str = str + ch;
		return str;
	}
	
	public static int Long() {
		int ret = stream.ReadInt32 ();
		return ret;
	}
	public static uint uLong() {
		uint ret = stream.ReadUInt32 ();
		return ret;
	}
	
	public static int Short() {

		int ret = stream.ReadInt16 ();

		return ret;
	}
	public static int uShort() {
		int ret = stream.ReadUInt16 ();
		return ret;
	}
	
	public static byte Byte() {
		byte ret = stream.ReadByte ();
		return ret;
	}
	public static int FTell () {
		string pos =  stream.BaseStream.Position.ToString();
		int ret = int.Parse (pos);
		return ret;
	}
	
	public static byte[] Bytes(int count) {
		return stream.ReadBytes (count);
	}
	
	public static float Float() {
		return stream.ReadSingle ();
	}
	
	public float U82F(int byt) {
		return 1.0f / 255 * byt;
	}
	
	
	
	
	
	
	[System.Serializable]
	public class Subset	 {
		
		public bool useFloat;
		public int uvOffset = 0;
		public int boneWeightsOffset = 0;
		
		public int size = 0;
		public int offset = -1;
		public string name = "";
		public int primitiveCount = -1;
		public int indexOffset = -1;
		public int vertexOffset = -1;
		public int vertexCount = -1;
		public int vertexStride = -1;
		public int primitiveType = -1;
		public int bonesPerVertex = -1;
		public int boneCount = -1;
		
		public int u0 = -1;
		public long vxOffset;
		public int idxOffset;
		public int verticesSize;
		public List<int> bones = new List<int>();
		public List<Vector3> v3 = new List<Vector3>();
		public List<int> v2 = new List<int>();
		
		
		public void Start() {
			
			//Debug.Log("    File offset: " + offset);
			name = String ();
			
			//Debug.Log("    Name: " + name);
			primitiveCount = Long ();
			vertexCount = Long ();
			indexOffset = Long ();
			vertexOffset = Long ();
			vertexStride = Byte ();
			primitiveType = Byte ();
			bonesPerVertex = Byte ();
			boneCount = Byte (); 
			u0 = Short ();
			size += 22;
			
			int i = 0;
			while (i < boneCount) {
				int boneId = Short ();
				size += 2;
				bones.Add(boneId);
				i++;
			}
			
			
			
		}
		public float h2f(int h) {
			int hres = (int)((h & 0x8000) << 16) | (int)(((h & 0x7fff) << 13) + 0x38000000);
			
			byte[] f = System.BitConverter.GetBytes(hres);
			float ret = System.BitConverter.ToSingle (f, 0);
			//Debug.Log (hres);
			return ret;
		}
		
		public List<Vector3> GetV3(int offs) {
			List<Vector3> ve = new List<Vector3>();
			if (vertexStride < offs) {
				//Debug.Log(vertexStride + " | " + offs);
				return ve;
			} else {
				
				
				int i = 0;
				stream.BaseStream.Seek (vxOffset, 0);
				
				while (i < vertexCount) {
					
					if (useFloat) {
						var ix = Float ();
						var iy = Float ();
						var iz = Float ();
						Bytes (vertexStride - 12);
						ve.Add (new Vector3 (-ix,iy,iz));
					} else {
						
						// i think there's some trouble here...
						int ix =  uShort();
						int iy =  uShort();
						int iz =  uShort();
						Bytes (vertexStride - 6);
						
						
						ve.Add (new Vector3 (h2f(ix),h2f(iy),h2f(iz)));
						//Debug.Log ("New vert");
						
					}
					i++;
				}
				
			}
			v3 = ve;
			return ve;
		}


		public List<Vector3> GetV3Inverted(int offs) {
			List<Vector3> ve = new List<Vector3>();
			if (vertexStride < offs) {
				//Debug.Log(vertexStride + " | " + offs);
				return ve;
			} else {
				
				
				int i = 0;
				stream.BaseStream.Seek (vxOffset, 0);
				
				while (i < vertexCount) {
					
					if (useFloat) {
						var ix = Float ();
						var iy = Float ();
						var iz = Float ();
						Bytes (vertexStride - 12);
						ve.Add (new Vector3 (-ix,iy,iz));
					} else {
						
						// i think there's some trouble here...
						int ix =  uShort();
						int iy =  uShort();
						int iz =  uShort();
						Bytes (vertexStride - 6);
						
						
						ve.Add (new Vector3 (-h2f(ix), -h2f(iy),h2f(iz)));
						//Debug.Log ("New vert");
						
					}
					i++;
				}
				
			}
			v3 = ve;
			return ve;
		}

		//SaveWeight???????????
		//GetWeight??????
		
		
		
		//		public List<Color> GetColors (int ofs) {
		//			List<Color> col = new List<Color>();
		//			stream.BaseStream.Seek(vxOffset,0);
		//			int i = 0;
		//			while( i < vertexCount) {
		//				//byte[] vxdata = stream.BaseStream.Read(stream,gxStride, );
		//				int r = Byte ();
		//				int g = Byte ();
		//				int b = Byte ();
		//				int a = Byte ();
		//				//col.Add(new Color(U82F(r),U82F(g),U82F(b),U82F(a)));
		//			}
		//			return col;
		//		}
		
		// Prob wrong. Most likely wrong.
		public List<Vector2> getV2 () {
			int offs = uvOffset;
			List<Vector2> ve = new List<Vector2> ();
			if (vertexStride < offs) {
				return ve;
			} else {
				stream.BaseStream.Seek(vxOffset + offs,0);
				int i = 0;
				while( i < vertexCount) {
					int iu = Short ();
					int iv = Short ();
					Bytes (vertexStride - 4);
					ve.Add(new Vector2(h2f (iu),(1.0f - h2f(iv) )));
					i++;
				}
			}
			return ve; 
		}
		
		public void getV2offfsets () {
			List<Vector2> ve = new List<Vector2> ();
			bool foundUV = false;
			
			bool stop = false;
			int v2offset = 0;
			if (vertexStride < uvOffset) {
				
			} else {
				while(v2offset < 64 && !foundUV && stop == false) {
					stream.BaseStream.Seek(vxOffset + v2offset,0);
					int i = 0;
					Vector2 uvtest = new Vector2();
					bool invalidChar = false;
					while( i < vertexCount && stop == false) {
						int ret = 0;
						Debug.Log (stream.BaseStream.Length + " | " + FTell ());
						if (stream.BaseStream.Length - (FTell () + 4) > 0) {
							stop = true;
						} else {
							int iu = Short ();
							int iv = Short ();
							Bytes (vertexStride - 4);
							//ve.Add(new Vector2(h2f (iu),(1.0f - h2f(iv) )));
							if(iu == null || iu == null) {
								Debug.Log("End of flile");
							} else {
								uvtest = new Vector2(h2f (iu),(1.0f - h2f(iv) ));
								
								if( !(uvtest.x < 1.0f && uvtest.x > 0.001f)) {
									
									invalidChar = true;
								}
								
								if( !(uvtest.y < 1.0f && uvtest.y > 0.001f)) {
									
									invalidChar = true;
								}
							}
						}
						i++;
					}
					
					
					if( invalidChar == false) {
						Debug.Log("Found offset: " + v2offset + " for: + " + name);
						foundUV = true;
						uvOffset = v2offset;
					} else {
						//Debug.Log("Tried offset: " + v2offset);
						v2offset ++;
					}
				}
				
			}
		}
		
		
		public List<int> GetIndices() {
			List<int> fa = new List<int> ();
			
			stream.BaseStream.Seek (idxOffset, 0);
			int i = 0;
			while (i < primitiveCount) {
				int i1 = uShort ();
				int i2 = uShort ();
				int i3 = uShort();
				fa.Add(i1);
				fa.Add(i2);
				fa.Add(i3);
				i++;
			}
			v2 = fa;
			return fa;
			
		}
		
	}
	
	public class Bone {
		
	}
	
	
	
	
	
	//public Object[] bones = new Object[];
	
	
	
	
	
	
	[System.Serializable]
	public class MeshData {
		public bool useFloat;
		public int size = 0;
		public int meshType = -1;
		public int subsetCount = -1;
		public List<Subset> subset = new List<Subset>();
		public List<byte> subsetExtraByte = new List<byte>();
		public byte[] unknownChunk;
		public int u0;
		
		public int dataOffset;
		
		public void GetUnkownChunk() {
			unknownChunk = Bytes (28);
			size += 28;
		}
		
		public void GetHeader() {
			meshType = Long();
			u0 = Short ();
			subsetCount = Byte ();
			size += 7;
		}
		
		
		public void GetSubsets() {
			int i = 0;
			while(i < subsetCount) {
				//Debug.Log("Subset [" + i + "]");
				Subset sub = new Subset();
				sub.offset = FTell();
				sub.useFloat = useFloat;
				subset.Add(sub);
				sub.Start();
				size += sub.size;
				i++;
			}
		}
		
		//Not sure.....
		public void GetSubsetExtraBytes() {
			int i = 0;
			while (i  < subsetCount) {
				byte extrabyte = Byte ();
				size += 1;
				subsetExtraByte.Add(extrabyte);
				//Debug.Log(extrabyte.ToString());
				i++;
			}
		}
		
		public int GetVerticesSize() {
			int maxv = 0;
			int i = 0;
			while (i < subsetCount) {
				Subset sub = subset[i];
				if(sub.vertexOffset + sub.vertexCount * sub.vertexStride > maxv) {
					maxv = sub.vertexOffset + sub.vertexCount * sub.vertexStride;
				}
				i++;
			}
			return maxv;
		}
		
		
		public int GetIndicesSize() {
			int maxi = 0;
			int i = 0;
			while (i < subsetCount) {
				Subset sub = subset[i];
				if((sub.indexOffset + sub.primitiveCount * 3) * 2 > maxi) {
					maxi = (sub.indexOffset + sub.primitiveCount * 3) * 2;
				}
				i++;
			}
			return maxi;
		}
		
		public void  AdjustVxOffs() {
			int i = 0;
			while (i < subsetCount) {
				Subset sub = subset[i];
				sub.vxOffset = dataOffset + sub.vertexOffset;
				i++;
			}
		}
		public void  AdjustIdxOffs() {
			int verticesSize = GetVerticesSize ();
			int i = 0;
			while (i < subsetCount) {
				Subset sub = subset[i];
				sub.idxOffset = dataOffset + verticesSize + (sub.indexOffset * 2);
				i++;
			}
		}
		
		public void Init() {
			
			
			size = 0;
			meshType = -1;
			u0 = -1;
			subsetCount = -1;
			subset = new List<Subset>();
			//subsetExtraByte = new List<byte>();
			//unknownChunk = byte[];
			
			
			
			GetHeader();
			GetSubsets();
			GetSubsetExtraBytes ();

			GetUnkownChunk();
			dataOffset = FTell ();
			size += GetVerticesSize();
			size += GetIndicesSize();
			
			AdjustVxOffs();
			AdjustIdxOffs();
			
			
			
		}
	}
	
}
