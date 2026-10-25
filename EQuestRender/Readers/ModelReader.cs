using System;
using System.Collections.Generic;
using System.Diagnostics;
using S3DFileLib;
using S3DFileLib.WldFrags;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EQuestRender.Readers
{
	public class ModelReader : BaseModelReader
	{
		public ModelReader()
		{
		}

		public void ReadModels(S3DFile s3d, WLDFile wld, GraphicsDevice graphics, out List<Mesh> models, 
			out TextureListContainer textures)
		{
			TextureReader texReader = new TextureReader(s3d, wld);
			textures = texReader.ReadTextures(graphics);

			models = new List<Mesh>();

			foreach (var frag in wld.Frags)
			{
				if (frag.FragType == 0x36)
					models.Add(ReadMesh(frag.FragName, (Frag36)frag, textures, wld, graphics, false));
			}
		}

		public void ReadZoneObjects(S3DFile s3dObjects, WLDFile wldObjects, WLDFile wldObjectPositions,
			out List<ZoneObject> zoneObjects, out TextureListContainer textures, GraphicsDevice graphics)
		{
			// Load the object textures.
			TextureReader texReader = new TextureReader(s3dObjects, wldObjects);
			textures = texReader.ReadTextures(graphics);

			// Load the models.
			List<Mesh> models = new List<Mesh>();

			foreach (var frag in wldObjects.Frags)
			{
				if (frag.FragType == 0x14)
				{
					var frag14 = (Frag14) frag;
					var frag2D = wldObjects.FindFragByRef(frag14.Ref[0]) as Frag2D;

					if (frag2D == null)
						continue;

					var frag36 = (Frag36) wldObjects.FindFragByRef(frag2D.Ref);
					models.Add(ReadMesh(frag14.FragName, frag36, textures, wldObjects, graphics, false));
				}
			}

			// Load scene model positions.
			zoneObjects = new List<ZoneObject>();

			foreach (var frag in wldObjectPositions.Frags)
			{
				if (frag.FragType == 0x15)
				{
					var frag15 = (Frag15)frag;
					string refName = wldObjectPositions.GetString(-(frag15.Ref0));
					var model = FindModelByName(models, refName);

					if (model != null)
						zoneObjects.Add(ReadZoneObject(model, frag15));
				}
			}
		}

		private ZoneObject ReadZoneObject(Mesh model, Frag15 frag15)
		{
			return new ZoneObject(
				model, 
				new Vector3(frag15.X, frag15.Y, frag15.Z),
				new Vector3(frag15.RotateX * 360/512, frag15.RotateY * 360/512, frag15.RotateZ * 360/512),
				new Vector3(frag15.ScaleX, frag15.ScaleX, frag15.ScaleX));
		}

		private Mesh FindModelByName(List<Mesh> models, string name)
		{
			foreach (var m in models)
			{
				if (m.Name == name)
					return m;
			}

			return null;
		}
	}
}
