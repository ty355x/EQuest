using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using S3DFileLib;
using S3DFileLib.WldFrags;
using EQuestRender.Animations;

namespace EQuestRender.Readers
{
	public class MobModelReader : BaseModelReader
	{
		private S3DFile _s3d;
		private WLDFile _wld;
		private MobModelHeadFinder _headFinder;

		public MobModelReader(S3DFile s3d, WLDFile wld)
		{
			_s3d = s3d;
			_wld = wld;
			_headFinder = new MobModelHeadFinder(this);
		}

		public List<MobModel> ReadMobModels(TextureListContainer textures, GraphicsDevice graphics)
		{
			var mobModels = new List<MobModel>();

			foreach (var frag in _wld.Frags)
			{
				// The starting point for a mob model is a Frag14 (Static/Animated Model Reference).
				if (frag.FragType != 0x14)
					continue;

				// Get the frag10 pointed to by this frag.
				Frag10 frag10 = GetFrag10FromFrag((Frag14)frag);

				if (frag10 == null)
					continue;

				// Get the list of Frag36s (meshes) for this frag.
				Frag36[] frag36List = GetFrag36List(frag10);

				// No Frag36s found.
				if (frag36List == null || frag36List.Length == 0)
					continue;

				// Read the mob model and add it to the mob model list.
				mobModels.Add(ReadMobModel(frag10, frag36List, textures, graphics));
			}

			// Add the base animation to the model.
			foreach (var model in mobModels)
			{
				model.Animations.Add(new MobModelAnimation("BASE", model.Skeleton));
			}

			// PrintAnimationList(mobModels);

			return mobModels;
		}

		private MobModel ReadMobModel(Frag10 frag10, Frag36[] frag36List, TextureListContainer textures, GraphicsDevice graphics)
		{
			var meshes = new MobMesh[frag36List.Length];

			// Read all the meshes for the model.
			for (int meshIndex = 0; meshIndex < frag36List.Length; meshIndex++)
			{
				var frag36 = frag36List[meshIndex];
				var mesh = ReadMesh(frag36.FragName, frag36, textures, _wld, graphics, true);
				meshes[meshIndex] = (MobMesh)mesh;
			}

			// Build the model's skeleton.
			var skeleton = BuildSkeleton(frag10, _wld);

			//AnimationHelper.Debug = frag10.FragName == "FIS_HS_DEF";
			AnimationHelper.TransformSkeleton(frag10.FragName.Substring(0, 3), skeleton);

			var result = _headFinder.DetermineBodyAndHeads(meshes, _wld, textures, graphics);

			// Create the model.
			return new MobModel(frag10.FragName, result.Body, result.Heads, skeleton, graphics);
		}

		private SkeletonPiece[] BuildSkeleton(Frag10 frag10, WLDFile wld)
		{
			var pieces = new SkeletonPiece[frag10.Entries.Length];

			for (int i = 0; i < frag10.Entries.Length; i++)
			{
				var entry = frag10.Entries[i];
				var frag13 = (Frag13)wld.FindFragByRef(entry.FragRef1);
				var frag12 = (Frag12)wld.FindFragByRef(frag13.Ref);
				pieces[i] = ReadSkeletonPiece(frag12, entry.Children);
			}

			// PrintSkeleton(pieces[0], pieces, 0);
			return pieces;
		}

		private SkeletonPiece ReadSkeletonPiece(Frag12 frag12, int[] children)
		{
			var piece = new SkeletonPiece()
			{
				Name = frag12.FragName,
				Children = children
			};

			foreach (var frag12Frame in frag12.Frames)
				piece.Frames.Add(AnimationHelper.MakeSkelPieceFrame(frag12Frame));

			return piece;
		}

		private Frag10 GetFrag10FromFrag(Frag14 frag14)
		{
			// Frag14 should point to a single Frag11 (Skeleton Track Ref).
			if (frag14.Ref.Length != 1)
			{
				RenderLog.Debug("Incorrect number of Frag11 refs found for Frag14 " + frag14.FragName);
				return null;
			}

			// Get the Frag11 pointed to by the Frag14.
			var frag11 = _wld.FindFragByRef(frag14.Ref[0]) as Frag11;

			// No Frag11 found.
			if (frag11 == null)
				return null;

			// Frag11 points to a frag10 (Skeleton Track Set).
			var frag10 = _wld.FindFragByRef(frag11.Ref) as Frag10;

			return frag10;
		}

		private Frag36[] GetFrag36List(Frag10 frag10)
		{
			if (frag10.Frag3Refs == null)
				return null;

			var frag36List = new Frag36[frag10.Frag3Refs.Length];

			for (int i = 0; i < frag10.Frag3Refs.Length; i++)
			{
				var frag2D = (Frag2D)_wld.FindFragByRef(frag10.Frag3Refs[i]);
				frag36List[i] = (Frag36)_wld.FindFragByRef(frag2D.Ref);
			}

			return frag36List;
		}

		private void PrintSkeleton(SkeletonPiece piece, SkeletonPiece[] allPieces, int level)
		{
			string s = "";

			for (int i = 0; i < level; i++)
				s += "-";

			RenderLog.Debug(s + " " + piece.Name);

			foreach (int childIndex in piece.Children)
				PrintSkeleton(allPieces[childIndex], allPieces, level + 1);
		}

		private void PrintAnimationList(List<MobModel> mobModels)
		{
			foreach (var model in mobModels)
			{
				var sb = new System.Text.StringBuilder();
				sb.Append("Model " + model.Name + " has " + model.AnimationCount + " animations: ");

				foreach (var anim in model.Animations)
					sb.Append(anim.Name + " ");

				RenderLog.Debug(sb.ToString());
			}
		}
	}
}
