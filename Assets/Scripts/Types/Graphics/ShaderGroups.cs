using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderDBParser
{
	class ShaderGroups
	{
		private long itemCount;
		ShaderGroup1 group1;
		ShaderGroup2 group2;

		public ShaderGroups(BinaryReader p_Reader)
		{
			group1 = new ShaderGroup1(p_Reader);
			group2 = new ShaderGroup2(p_Reader);

		}


	}
	
	class ShaderGroup1
	{
		long shaderCount;
		List<Shader> shaders = new List<Shader>();
		List<ShaderDecleration> shaderDeclerations = new List<ShaderDecleration>();

		public ShaderGroup1(BinaryReader p_Reader)
		{
			shaderCount = p_Reader.ReadInt32();
			
			for(int i = 0;i < shaderCount; i++)
			{
				shaders.Add(new Shader(p_Reader, 12));
				shaderDeclerations.Add(new ShaderDecleration(p_Reader));
			}
		}
	}
	class ShaderGroup2
	{
		long shaderCount;
		List<Shader> shaders = new List<Shader>();
		List<ShaderDecleration> shaderDeclerations = new List<ShaderDecleration>();

		public ShaderGroup2(BinaryReader p_Reader)
		{
			shaderCount = p_Reader.ReadInt32();

			shaders.Add(new Shader(p_Reader, 12)); // First shader has different meta
			for (int i = 0; i < shaderCount - 1; i++)
			{
				shaders.Add(new Shader(p_Reader, 24));
			}
		}
	}

	class magic4
	{
		char[] magic;
		public magic4(BinaryReader p_Reader)
		{
			magic = p_Reader.ReadChars(4);
		}
	}

	class Shader
	{
		long unknown;
		byte[] groupMeta;
		long shaderSize;
		magic4 dxbc;
		byte[] checksum;
		long unknown2;
		long shaderSize2;
		long chunkCount;

		long RDEFOffset;
		long ISGNOffset;
		long OSGNOffset;
		long SHEXOffset;
		long STATOffset;

		List<ShaderChunk> chunks = new List<ShaderChunk>();

		public Shader(BinaryReader p_Reader, int metadata_size)
		{
			unknown = p_Reader.ReadInt32();
			groupMeta = p_Reader.ReadBytes(metadata_size);
			shaderSize = p_Reader.ReadInt32();
			dxbc = new magic4(p_Reader);
			checksum = p_Reader.ReadBytes(16);
			unknown2 = p_Reader.ReadInt32();
			shaderSize2 = p_Reader.ReadInt32();
			chunkCount = p_Reader.ReadInt32();

			RDEFOffset = p_Reader.ReadInt32();
			ISGNOffset = p_Reader.ReadInt32();
			OSGNOffset = p_Reader.ReadInt32();
			SHEXOffset = p_Reader.ReadInt32();
			STATOffset = p_Reader.ReadInt32();

			for (int i = 0; i < chunkCount; i++)
			{
				chunks.Add(new ShaderChunk(p_Reader));
			}
		}
	}

	class ShaderChunk
	{
		magic4 chunkName;
		long chunkLength;
		byte[] content;

		public ShaderChunk(BinaryReader p_Reader)
		{
			chunkName = new magic4(p_Reader);
			chunkLength = p_Reader.ReadInt32();
			content = p_Reader.ReadBytes((int) chunkLength);
		}
	}

	class ShaderDecleration
	{
		byte[] groupMeta;
		long shaderSize;
		magic4 dxbc;
		byte[] checksum;
		long unknown;
		long shaderSize2;
		long chunkCount;
		long ISGNOffset;
		magic4 chunkName;
		long chunkLength;
		byte[] content;

		long propertyCount;

		ShaderValues values;

		long propertyNameCount;

		ShaderNames names;

		public ShaderDecleration(BinaryReader p_Reader)
		{
			groupMeta = p_Reader.ReadBytes(12);
			shaderSize = p_Reader.ReadInt32();
			dxbc = new magic4(p_Reader);
			checksum = p_Reader.ReadBytes(16);
			unknown = p_Reader.ReadInt32();
			shaderSize2 = p_Reader.ReadInt32();
			chunkCount = p_Reader.ReadInt32();
			ISGNOffset = p_Reader.ReadInt32();
			chunkName = new magic4(p_Reader);
			chunkLength = p_Reader.ReadInt32();
			content = p_Reader.ReadBytes((int) chunkLength);
			propertyCount = p_Reader.ReadInt32();

			values = new ShaderValues(p_Reader, (int) propertyCount);

			propertyNameCount = p_Reader.ReadInt32();

			names = new ShaderNames(p_Reader, (int)propertyCount);

		}

	}

	class ShaderValues
	{
		List<byte[]> values = new List<byte[]>();

		public ShaderValues(BinaryReader p_Reader, int propertyCount)
		{
			for (int i = 0; i < propertyCount; i++)
			{
				values.Add(p_Reader.ReadBytes(28));
			}
		}
	}

	class ShaderNames
	{
		List<string> names = new List<string>();

		public ShaderNames(BinaryReader p_Reader, int propertyCount)
		{
			for (int i = 0; i < propertyCount; i++)
			{
		
				string name = "";
				char ch;
				while ((int)(ch = p_Reader.ReadChar()) != 0)
					name = name + ch;

				names.Add(name);
			}
		}
	}


	
}
