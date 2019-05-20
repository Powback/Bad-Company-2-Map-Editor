using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderDBParser
{
	class Block2
	{
		private long itemCount;
		Block2Group group1;
		Block2Group group2;

		public Block2(BinaryReader p_Reader)
		{
			group1 = new Block2Group(p_Reader);
			group2 = new Block2Group(p_Reader);
			
		}

	}
	class Block2Group
	{
		private long itemCount;
		private List<Block2Item> items = new List<Block2Item>();

		public Block2Group(BinaryReader p_Reader)
		{
			itemCount = p_Reader.ReadInt32();
			for (int i = 0; i < itemCount; i++)
			{
				items.Add(new Block2Item(p_Reader));
			}
		}
	}
	class Block2Item
	{
		private short count;
		private short A;
		private List<long> items = new List<long>();

		public Block2Item(BinaryReader p_Reader)
		{
			count = p_Reader.ReadInt16();
			A = p_Reader.ReadInt16();
			for(int i = 0; i < count; i++)
			{
				items.Add(p_Reader.ReadInt32());
			}
		}
	}
}
