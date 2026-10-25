using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using S3DFileLib;
using S3DFileLib.WldFrags;

namespace EQuestRender.Readers
{
	public class TextureReader
	{
		private S3DFile _s3d = null;
		private WLDFile _wld = null;

		public TextureReader(S3DFile s3d, WLDFile wld)
		{
			_s3d = s3d;
			_wld = wld;
		}

		public TextureImage ReadImage(string name, GraphicsDevice graphics)
		{
			return ReadTextureImage(name, 1, graphics);
		}

		public TextureListContainer ReadTextures(GraphicsDevice graphics)
		{
			List<Texture> textures = new List<Texture>();
			List<TextureList> textureSets = new List<TextureList>();

			// Read textures.
			foreach (var frag in _wld.Frags)
			{
				if (frag.FragType == 0x30)
				{
					// RenderLog.Debug("Reading texture " + frag.FragName);
					Texture tex = ReadTexture((Frag30)frag, graphics);
					
					if (tex != null) 
						textures.Add(tex);
				}
			}

			// Read texture sets.
			foreach (var frag in _wld.Frags)
			{
				if (frag.FragType == 0x31)
				{
					TextureList ts = ReadTextureSet((Frag31)frag, textures);
					
					if (ts != null) 
						textureSets.Add(ts);
				}
			}

			return new TextureListContainer(textures.ToArray(), textureSets.ToArray());
		}

		private Texture ReadTexture(Frag30 frag30, GraphicsDevice graphics)
		{
			if (frag30.Ref == 0)
				return null;

			Frag05 frag05 = (Frag05)_wld.FindFragByRef(frag30.Ref);
			Frag04 frag04 = (Frag04)_wld.FindFragByRef(frag05.Ref);
			TextureImage[] textureImages = new TextureImage[frag04.Refs.Length];

			for (int i = 0; i < frag04.Refs.Length; i++)
			{
				Frag03 frag03 = (Frag03)_wld.FindFragByRef(frag04.Refs[i]);
				textureImages[i] = ReadTextureImage(frag03.Name, frag30.Params1, graphics);
			}

			return new Texture(frag30.FragIndex, frag30.FragName, textureImages);
		}

		private TextureList ReadTextureSet(Frag31 frag31, List<Texture> textures)
		{
			Texture[] tsEntries = new Texture[frag31.Refs.Length];

			for (int i = 0; i < frag31.Refs.Length; i++)
			{
				Frag texFrag = _wld.FindFragByRef(frag31.Refs[i]);
				Texture tex = FindTexture(texFrag.FragIndex, textures);
				tsEntries[i] = tex;
			}

			return new TextureList(frag31.FragName, frag31.FragIndex, tsEntries);
		}

		private TextureImage ReadTextureImage(string imageName, uint params1, GraphicsDevice graphics)
		{
			if (params1 == 0)
				return null;

			uint maskMasked = 2;
			uint maskTransparent2 = 8;
			uint maskLastPalEntryNotMasked = 16;
			
			bool isMasked = (params1 & maskMasked) == maskMasked;
			bool isTransparent = (params1 & maskTransparent2) == maskTransparent2;
			bool isLastPalEntryMasked = (params1 & maskLastPalEntryNotMasked) != maskLastPalEntryNotMasked;

			using (var bmpStream = _s3d.GetFileStream(imageName))
			{
				if (bmpStream == null) 
					return null;
				
				using (var texBmp = (Bitmap)Image.FromStream(bmpStream))
				{
					if (isTransparent)
					{
						ColorPalette pal = texBmp.Palette;

						for (int i = 0; i < pal.Entries.Length; i++)
						{
							pal.Entries[i] = Color.FromArgb(32, pal.Entries[i].R, pal.Entries[i].G, pal.Entries[i].B);
						}

						texBmp.Palette = pal;
					}

					if (isMasked)
					{
						ColorPalette pal = texBmp.Palette;
						pal.Entries[0] = Color.FromArgb(0, 0, 0, 0);
						
						if (isLastPalEntryMasked)
						{
							pal.Entries[pal.Entries.Length - 1] = Color.FromArgb(0, 0, 0, 0);
						}

						texBmp.Palette = pal;
					}

					using (var texPngStream = new MemoryStream())
					{
						texBmp.Save(texPngStream, ImageFormat.Png);
						
						Texture2D image = Texture2D.FromStream(graphics, texPngStream);
						
						TextureProperty property = isTransparent ? TextureProperty.Transparent :
							isMasked ? TextureProperty.Masked : TextureProperty.Normal;
						
						return new TextureImage(image, property);
					}
				}
			}
		}

		private Texture FindTexture(int id, List<Texture> textures)
		{
			foreach (var tex in textures)
			{
				if (tex.FragId == id)
					return tex;
			}

			return null;
		}
	}
}
