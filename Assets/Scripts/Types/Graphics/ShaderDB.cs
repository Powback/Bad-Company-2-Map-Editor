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
		public Dictionary<string, List<string>> ResourceDictionary = new Dictionary<string, List<string>>();

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

			LoadDictionary();
		}

		public void LoadDictionary()
		{
			Dictionary<string, short[]> block7items = this.block7.getItems();
			List<Block4Item> block4items = this.block4.getItems();
			List<ParamGroup> paramGroups = this.paramGroups.GetParamGroups();
			ShaderGroup1 shaderGroup1 = this.shaderGroups.GetParamGroup1();
			ShaderGroup2 shaderGroup2 = this.shaderGroups.GetParamGroup2();
			foreach (Block6GroupItem block6groupitem in this.block6.GetItems())
			{
				Console.WriteLine(block6groupitem.name);

				List<string> textureNames = new List<string>();
				short[] shorts;
				block7items.TryGetValue(block6groupitem.reference, out shorts);
				if (shorts == null)
				{
					continue;
				}
				foreach (short index in shorts)
				{
					Block4Item block4Item = block4items[index];
					ShaderDecleration sd1 = shaderGroup1.shaderDeclerations[(int)block4Item.shaderIndex1];
					Shader shader1 = shaderGroup1.shaders[(int)block4Item.shaderIndex1];

					ShaderDecleration sd2 = shaderGroup1.shaderDeclerations[(int)block4Item.shaderIndex1];
					Shader shader2 = shaderGroup1.shaders[(int)block4Item.shaderIndex1];

					ParamGroup paramGroup = paramGroups[(int)block4Item.paramGroupIndex];
					List<TextureParameter> textureParameter = paramGroup.getTextureParameters();
					//this.ResourceDictionary.Add(block6groupitem.name, textureParameter);
					foreach (TextureParameter tp in textureParameter)
					{
						if (!textureNames.Contains(tp.TextureName))
						{
							Console.WriteLine(index + "----" + tp.TextureName);
							textureNames.Add(tp.TextureName);
						}
					}
				}
				this.ResourceDictionary.Add(block6groupitem.name.ToLower(), textureNames);

			}
		}
	}
}