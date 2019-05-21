using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderDBParser
{
	class Block7
	{
		long itemCount;
		List<Block7Item> items = new List<Block7Item>(); 

		public Block7(BinaryReader p_Reader)
		{
			itemCount = p_Reader.ReadInt32();
			for(int i = 0; i < itemCount; i++)
			{
				items.Add(new Block7Item(p_Reader));
			}
		}

		public Dictionary<string, short[]> getItems()
		{
			Dictionary<string, short[]> items = new Dictionary<string, short[]>();
			foreach(Block7Item item in this.items)
			{
				items.Add(item.reference, item.items);
			}
			return items;
		}
	}

	class Block7Item
	{
		public string reference;
		long shortsCount;
		public short[] items;

		public Block7Item(BinaryReader p_Reader)
		{
			reference = Util.ByteArrayToString(p_Reader.ReadBytes(16));
			shortsCount = p_Reader.ReadInt32();
			items = new short[shortsCount];

			for(int i = 0; i < shortsCount; i++)
			{
				items[i] = p_Reader.ReadInt16();
			}
		}
	}
}
