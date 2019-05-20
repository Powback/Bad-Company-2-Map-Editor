using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ShaderDBParser
{
	class ShaderDBHeader
	{
		private char[] header;

		public ShaderDBHeader(BinaryReader p_Reader)
		{
			this.header = p_Reader.ReadChars(5);
		}
	}

	public class ShaderDB
	{
		public Dictionary<string, string> ResourceDictionary = new Dictionary<string, string>();

		private ShaderDBHeader header;
		private ParamGroups paramGroups;
		private Block2 block2;
		private ShaderGroups shaderGroups;
		private Block4 block4;
		private Block5 block5;
		private Block6 block6;
		private Block7 block7;

		public void LoadShaderDatabase(string loc)
		{
			byte[] s_Data = Util.ReadFile(loc);
			using (var s_Reader = new BinaryReader(new MemoryStream(s_Data)))
			{
				this.header = new ShaderDBHeader(s_Reader);
				this.paramGroups = new ParamGroups(s_Reader);
				this.block2 = new Block2(s_Reader);
				this.shaderGroups = new ShaderGroups(s_Reader);
				this.block4 = new Block4(s_Reader);
				this.block5 = new Block5(s_Reader);
				this.block6 = new Block6(s_Reader);
				this.block7 = new Block7(s_Reader);


			}
		}
	}
}