using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderDBParser
{
	class Block4
	{
		Block4Header header;
		List<Block4Item> items = new List<Block4Item>();
		public Block4(BinaryReader p_Reader)
		{
			header = new Block4Header(p_Reader);
			for(int i = 0; i < header.itemCount; i++)
			{
				items.Add(new Block4Item(p_Reader));
			}
		}
	}

	class Block4Item
	{
		byte[] reference;
		long A;
		long B;
		long C;
		long D;
		long shaderIndex1;
		long shaderIndex2;
		long E;
		long paramGroupIndex;

		public Block4Item(BinaryReader p_Reader)
		{
			reference = p_Reader.ReadBytes(16);
			A = p_Reader.ReadInt32();
			B = p_Reader.ReadInt32();
			C = p_Reader.ReadInt32();
			D = p_Reader.ReadInt32();
			shaderIndex1 = p_Reader.ReadInt32();
			shaderIndex2 = p_Reader.ReadInt32();
			E = p_Reader.ReadInt32();
			paramGroupIndex = p_Reader.ReadInt32();
		}
	}

	class Block4Header
	{
		long A;
		long B;
		long C;
		public long itemCount;

		public Block4Header(BinaryReader p_Reader)
		{
			A = p_Reader.ReadInt32();
			B = p_Reader.ReadInt32();
			C = p_Reader.ReadInt32();
			itemCount = p_Reader.ReadInt32();
		}
	}
}
