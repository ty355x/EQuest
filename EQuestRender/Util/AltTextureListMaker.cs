using System;
using System.Collections.Generic;

namespace EQuestRender.Util
{
	public class AltTextureListMaker
	{
		public void MakeAltTextureLists(Texture[] textures, TextureList[] textureLists)
		{
			Dictionary<string, Texture> hash = MakeTextureHash(textures);

			foreach (var list in textureLists)
				MakeAltTextureListsForList(list, hash);
		}

		private void MakeAltTextureListsForList(TextureList list, Dictionary<string, Texture> hash)
		{
			if (!ShouldProcessTextureList(list))
				return;

			// Search for alt texture lists.
			for (int matIndex = 0; matIndex < 10; matIndex++)
			{
				// RenderLog.Debug("Making alt texture list " + matIndex);

				var texAltName = MakeAltTextureName(list[0].Name, matIndex);
				
				if (!hash.ContainsKey(texAltName))
				{
					// RenderLog.Debug("Unable to make list: first alt texture not found");
					break;
				}

				Texture[] altTextures = new Texture[list.Length];
				bool foundAllTextures = true;

				for (int texIndex = 0; texIndex < list.Length; texIndex++)
				{
					var altTextureName = MakeAltTextureName(list[texIndex].Name, matIndex);
					var tex = hash.ContainsKey(altTextureName) ? hash[altTextureName] : null;

					if (tex == null)
					{
						if (IsHeadTexture(list[texIndex].Name))
						{
							tex = list[texIndex];
						}
						else
						{
							foundAllTextures = false;
							RenderLog.Debug("Unable to make list: unable to find alt texture " + altTextureName);
							break;
						}
					}

					altTextures[texIndex] = tex;
				}

				if (foundAllTextures)
				{
					// RenderLog.Debug("Found all textures for alt list " + matIndex);
					list.AddAltTextureList(altTextures);
				}
			}
		}

		private bool IsHeadTexture(string textureName)
		{
			return textureName.Length == 9 && textureName.Substring(3, 2) == "HE";
		}

		private bool ShouldProcessTextureList(TextureList list)
		{
			if (list == null || list.Length == 0)
				return false;

			string texName = null;

			for (int i = 0; i < list.Length; i++)
			{
				if (list[i] == null)
					continue;

				if (list[i].Name.Length != 9)
					break;

				if (IsHeadTexture(list[i].Name))
				{
					continue;
				}
				else
				{
					texName = list[i].Name;
					break;
				}
			}

			if (texName == null)
				return false;

			// Get the postfix and make sure it's numeric.
			string texNamePostfix = GetTextureNamePostfix(texName);
			return IsStringNumeric(texNamePostfix);
		}

		private string GetTextureNamePostfix(string texName)
		{
			return texName.Substring(texName.Length - 4, 4);
		}

		private string MakeAltTextureName(string texName, int materialNumber)
		{
			if (texName.Length != 9)
				return texName;

			return texName.Substring(0, 5) +
				materialNumber.ToString("00").Substring(0, 2) +
				texName.Substring(7, 2);
		}

		private int GetMaterialNumber(string textureNamePostfix)
		{
			string texMaterialNumString = textureNamePostfix.Substring(0, 2);
			return int.Parse(texMaterialNumString);
		}

		private Dictionary<string, Texture> MakeTextureHash(Texture[] textures)
		{
			var hash = new Dictionary<string, Texture>();

			foreach (var tex in textures)
			{
				if (!hash.ContainsKey(tex.Name))
					hash.Add(tex.Name, tex);
			}

			return hash;
		}

		private bool IsStringNumeric(string s)
		{
			int result;
			return int.TryParse(s, out result);
		}
	}
}
