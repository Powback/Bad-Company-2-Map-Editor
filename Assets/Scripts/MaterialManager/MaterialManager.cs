using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using ShaderDBParser;
namespace Assets.Scripts.MaterialManager
{
	public class MaterialManager
	{

		Dictionary<string, Material> materials = new Dictionary<string, Material>();
		Dictionary<string, Dictionary<string, List<string>>> shaders = new Dictionary<string, Dictionary<string, List<string>>>();
		ShaderDB shaderdb = new ShaderDB();

		public MaterialManager(string levelName)
		{
			shaderdb.LoadShaderDatabase("levels/" + levelName + "/Shaders/Dx11_Single.dx11shaderdatabase");

		}

		List<string> GetTextures(string meshPath, string shaderPath)
		{
			string shaderDataPath = meshPath + "/" + shaderPath;
			List<string> textureNames = new List<string>();
			shaderdb.ResourceDictionary.TryGetValue(shaderDataPath.ToLower(), out textureNames);
			return textureNames;
		}


		public void RegisterShader(string meshPath, string shaderPath, string shaderName)
		{
			
			Dictionary<string, List<string>> shader;

			//Check if the shader already exists, if so, use that instead.
			if(!shaders.TryGetValue(meshPath, out shader))
			{
				shader = new Dictionary<string, List<string>>();
				shaders.Add(meshPath, shader);
			}
			if(!shader.ContainsKey(shaderName))
			{
				List<string> textureNames = GetTextures(meshPath, shaderPath);
				shader.Add(shaderName, textureNames);
			}
		}




		public Material GetMaterial(string meshPath, string shaderPath, string subsetName)
		{
			Material mat;
			MapLoad mapLoad = Util.GetMapload();

			string materialPath = meshPath + "/" + shaderPath;

			// We have already processed this material.
			if(materials.TryGetValue(materialPath, out mat))
			{
				return mat;
			}
			Dictionary<string, List<string>> shader;
			// We don't have that shader registered.
			if (!shaders.TryGetValue(meshPath, out shader))
			{
				return mapLoad.materialwhite;
			}

			// Loop through the shader names of the shader, figure out if any of them fits inside the subset name.
			foreach (string shaderName in shader.Keys)
			{
				if (subsetName.Contains(shaderName))
				{
					//The current shader fits within the subset name.
					List<string> textures = new List<string>();
					if(!shader.TryGetValue(shaderName, out textures)) 
					{
						Debug.Log(shaderName);
					}

					mat = new Material(mapLoad.materialwhite);
					mat.name = shaderName;
					if(textures == null)
					{
						return mapLoad.materialwhite;
					}
					//Loop through all the textures registered to that shader, and apply them to a new material.
					foreach (string textureName in textures)
					{
						//Figure out what type of texture this is and try to load it.
						string textureType = Util.GetTextureType(textureName);
						Texture2D texture = Util.LoadiTexture(textureName + ".itexture");

						if (textureType != "" && texture != null)
						{
							mat.SetTexture(textureType, texture);
						}
					}
					//Cache this material for later recovery.
					this.materials.Add(materialPath, mat);
					return mat;
				}
			}
			return mapLoad.materialwhite;
		}
	}
}
