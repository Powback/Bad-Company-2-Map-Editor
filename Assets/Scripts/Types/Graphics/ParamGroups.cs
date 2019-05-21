using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderDBParser
{
	class ParamGroups
	{
		private long groupCount;
		private List<ParamGroup> paramGroups = new List<ParamGroup>();


		public ParamGroups(BinaryReader p_Reader)
		{
			this.groupCount = p_Reader.ReadInt32();
			for (int i = 0; i < this.groupCount; i++)
			{
				ParamGroup l_Group = new ParamGroup(p_Reader);
				paramGroups.Add(l_Group);
			}
		}

		public List<ParamGroup> GetParamGroups()
		{
			return this.paramGroups;
		}
	}

	class ParamGroup
	{
		private long meta_start_pos = 0;
		private long groupSize;
		private long B;
		private long C;
		private long D;
		private long E;
		private long F;
		private byte G;
		private byte H;
		private byte I;
		private byte stringCount;
		private byte K;
		private byte L;
		private short M;

		List<TextureParameter> textureParameters = new List<TextureParameter>();
		List<PropertyParameter> propertyParameters = new List<PropertyParameter>();
		List<ParamGroupMeta> paramGroupMetas = new List<ParamGroupMeta>();


		public ParamGroup(BinaryReader p_Reader)
		{
			this.meta_start_pos = p_Reader.BaseStream.Position;
			this.groupSize = p_Reader.ReadInt32();
			this.B = p_Reader.ReadInt32();
			this.C = p_Reader.ReadInt32();
			this.D = p_Reader.ReadInt32();
			this.E = p_Reader.ReadInt32();
			this.F = p_Reader.ReadInt32();
			this.G = p_Reader.ReadByte();
			this.H = p_Reader.ReadByte();
			this.I = p_Reader.ReadByte();
			this.stringCount = p_Reader.ReadByte();
			this.K = p_Reader.ReadByte();
			this.L = p_Reader.ReadByte();
			this.M = p_Reader.ReadInt16();

			for (int i = 0; i < this.stringCount; i++)
			{
				this.textureParameters.Add(new TextureParameter(p_Reader));
			}

			for (int i = 0; i < this.K; i++)
			{
				this.propertyParameters.Add(new PropertyParameter(p_Reader, 6, 17));
			}

			for (int i = 0; i < this.L; i++)
			{
				this.propertyParameters.Add(new PropertyParameter(p_Reader, 4, 3));
			}

			long meta_remaining = (meta_start_pos + groupSize) - p_Reader.BaseStream.Position;
			long meta_end = meta_start_pos + groupSize;
			if (meta_remaining > 0)
			{
				this.paramGroupMetas.Add(new ParamGroupMeta(p_Reader, meta_end));
			}


		}
		public List<TextureParameter> getTextureParameters()
		{
			return this.textureParameters;
		}
	}

	class ParamGroupMeta
	{
		private long A;
		private long B;
		private byte[] meta_unknown;

		public ParamGroupMeta(BinaryReader p_Reader, long meta_end)
		{
			this.A = p_Reader.ReadInt32();
			this.B = p_Reader.ReadInt32();
			long meta_current = p_Reader.BaseStream.Position;
			int meta_remaining = (int)meta_end - (int)meta_current;

			if ((meta_end - meta_current) > 0)
			{
				meta_unknown = p_Reader.ReadBytes(meta_remaining);
			}
		}
	}

	public class TextureParameter
	{
		private long index;
		public string TextureName;

		public TextureParameter(BinaryReader p_Reader)
		{
			this.index = p_Reader.ReadInt32();
			this.TextureName = new string(p_Reader.ReadChars(136)).Replace("\0","");
		}

	}

	class PropertyParameter
	{
		private char[] propertyName;
		private byte[] GUID;
		private byte enabled;
		private byte[] misc;

		public PropertyParameter(BinaryReader p_Reader, int p_GuidSize, int p_MiscSize)
		{
			this.propertyName = p_Reader.ReadChars(32);
			this.GUID = p_Reader.ReadBytes(p_GuidSize);
			this.enabled = p_Reader.ReadByte();
			this.misc = p_Reader.ReadBytes(p_MiscSize);
		}
	}
}
