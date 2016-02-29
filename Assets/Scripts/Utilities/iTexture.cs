using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
namespace BC2.Util.Texture {
	public class iTexture {

		public static BinaryReader stream;

		private static int u1;
		private static int u2;
		private static int u4;
		private static int u5;
		private static int format;
		private static int mipmaps;
		private static int size;
		private static int sizex;
		private static int sizey;
		private static byte[] data;


		private static int mipmapsize = 0;
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

		public static uint ULong() {
			uint ret = stream.ReadUInt32 ();
			return ret;
		}
		public static int Short() {
			int ret = stream.ReadInt16 ();
			return ret;
		}
		public static uint UShort() {
			uint ret = stream.ReadUInt16();
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



		public static Texture2D LoadTexture(string path) {
			var s_Data = File.ReadAllBytes(path);
			using (var s_Reader = new BinaryReader (new MemoryStream (s_Data))) {
				stream = s_Reader;
				u1 = Long ();
				u2 = Long ();
				format = Long ();
				u4 = Long ();
				sizex = Long ();
				sizey = Long ();
				u5 = Long ();
				mipmaps = Long ();

				int i = 0;
				while(i < mipmaps) {
					int size = Long ();
					mipmapsize += size;
					i++;
				}
				int endPos = FTell ();
				int i2 = 0;
				Bytes (92 - endPos);
				byte[] mipmapdata = Bytes (mipmapsize);
				data = mipmapdata;


			}

			TextureFormat texFormat = TextureFormat.Alpha8;
			if (format == 0 || format == 18) {
				texFormat = TextureFormat.DXT1;
			} else if (format == 1) {
				texFormat = TextureFormat.DXT1; // should be dxt3
			} else if (format == 2 || format == 19 || format == 20) {
				texFormat = TextureFormat.DXT5;
			} else if (format == 9) {
				texFormat = TextureFormat.RGBA32;
			} else if (format == 10) {
				texFormat = TextureFormat.RGBA32;
			}
			Texture2D texture = null;
			bool useMipmaps = false;
			if(mipmaps > 1) { 
				useMipmaps = true;
			}

			texture = new Texture2D (sizex, sizey, texFormat, useMipmaps);


			texture.LoadRawTextureData (data);
			texture.Apply ();    
			return texture;
		}


	}

}