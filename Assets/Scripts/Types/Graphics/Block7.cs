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
	}

	class Block7Item
	{
		byte[] reference;
		long shortsCount;
		List<short> items = new List<short>();

		public Block7Item(BinaryReader p_Reader)
		{
			reference = p_Reader.ReadBytes(16);
			shortsCount = p_Reader.ReadInt32();

			for(int i = 0; i < shortsCount; i++)
			{
				items.Add(p_Reader.ReadInt16());
			}
		}
	}
}
