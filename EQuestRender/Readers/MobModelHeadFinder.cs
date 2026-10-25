using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using S3DFileLib;
using S3DFileLib.WldFrags;

namespace EQuestRender.Readers
{
	public class MobModelHeadFinder
	{
		public MobModelReader _modelReader;

		public MobModelHeadFinder(MobModelReader modelReader)
		{
			_modelReader = modelReader;
		}
		
		public MobModelHeadFinderResult DetermineBodyAndHeads(MobMesh[] meshes, WLDFile wld, TextureListContainer textures, GraphicsDevice graphics)
		{
			var heads = new List<MobMesh>();

			AssignBodyAndHead(meshes, out MobMesh body, heads);
			FindAltHeads(heads, wld, textures, graphics);
			
			return new MobModelHeadFinderResult() { Body = body, Heads = heads.ToArray() };
		}
		
		public void FindAltHeads(List<MobMesh> heads, WLDFile wld, TextureListContainer textures, GraphicsDevice graphics)
		{
			if (heads.Count == 0)
				return;

			string headName = heads[0].Name;
			
			for (int i = 1; i < 10; i++)
			{
				string altHeadName = MakeAltHeadName(headName, i);
				
				Frag36 frag = wld.FindFragByName(altHeadName) as Frag36;

				if (frag == null)
					break;

				heads.Add((MobMesh)_modelReader.ReadMesh(frag.FragName, frag, textures, wld, graphics, true));
				// RenderLog.Debug("Found head " + frag.FragName);
			}
		}

		private string MakeAltHeadName(string headName, int index)
		{
			return headName.Substring(0, 5) +
				index.ToString("00") +
				headName.Substring(7, headName.Length - 7);
		}

		private void AssignBodyAndHead(MobMesh[] meshes, out MobMesh mobBody, List<MobMesh> heads)
		{
			if (meshes.Length < 2)
			{
				mobBody = meshes[0];
				return;
			}

			bool isMesh0Head = IsHeadMesh(meshes[0]);
			bool isMesh1Head = IsHeadMesh(meshes[1]);

			if (isMesh0Head && isMesh1Head)
			{
				RenderLog.Debug("Warning: Found two head meshes for model " + meshes[0].Name);
				mobBody = meshes[0];
				return;
			}

			if (!isMesh0Head && !isMesh1Head)
			{
				RenderLog.Debug("Warning: Found no head meshes for model " + meshes[0].Name);
				mobBody = meshes[0];
				return;
			}

			if (isMesh0Head)
			{
				mobBody = meshes[1];
				heads.Add(meshes[0]);
				return;
			}
			else
			{
				mobBody = meshes[0];
				heads.Add(meshes[1]);
				return;
			}
		}

		private MobMesh[] FindAdditionalHeadMeshes(MobMesh headMesh)
		{
			if (headMesh == null || !IsHeadMesh(headMesh))
				return new MobMesh[] { headMesh };

			var meshes = new List<MobMesh>();
			meshes.Add(headMesh);

			return meshes.ToArray();
		}

		private bool IsHeadMesh(MobMesh mesh)
		{
			if (mesh == null || mesh.Name.Length != 19 || mesh.Name[7] != '_')
				return false;
		
			return mesh.Name.Substring(3, 2) == "HE";
		}
	}
}
