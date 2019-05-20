using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderDBParser
{
	class Block5
	{
		Block5Group1 group1;
		Block5Group2 group2;

		public Block5(BinaryReader p_Reader)
		{
			group1 = new Block5Group1(p_Reader);
			group2 = new Block5Group2(p_Reader);
		}
	}

	class Block5Group1
	{
		long itemCount;
		List<Block5Group1Item> items = new List<Block5Group1Item>();
		public Block5Group1(BinaryReader p_Reader)
		{
			itemCount = p_Reader.ReadInt32();
			for(int i = 0; i < itemCount; i++)
			{
				items.Add(new Block5Group1Item(p_Reader));
			}
		}
	}

	class Block5Group1Item
	{
		byte[] reference;
		byte[] reference2;
		byte[] reference3;
		byte[] unknown;
		long fieldCount;
		List<long> fields = new List<long>();
		short unknown2;

		public Block5Group1Item(BinaryReader p_Reader)
		{
			reference = p_Reader.ReadBytes(16);
			reference2 = p_Reader.ReadBytes(16);
			reference3 = p_Reader.ReadBytes(16);
			unknown = p_Reader.ReadBytes(10);
			fieldCount = p_Reader.ReadInt32();

			for(int i = 0; i < fieldCount; i++)
			{
				fields.Add(p_Reader.ReadInt32());
			}
			unknown2 = p_Reader.ReadInt16();
		}
	}

	class Block5Group2
	{
		long itemCount;
		List<Block5Group2Item> items = new List<Block5Group2Item>();
		public Block5Group2(BinaryReader p_Reader)
		{
			itemCount = p_Reader.ReadInt32();
			for(int i = 0; i < itemCount; i++)
			{
				items.Add(new Block5Group2Item(p_Reader));
			}
		}
	}

	class Block5Group2Item
	{
		byte[] reference;
		byte[] unknown;
		public Block5Group2Item(BinaryReader p_Reader)
		{
			reference = p_Reader.ReadBytes(16);
			unknown = p_Reader.ReadBytes(73);
		}
	}
}
