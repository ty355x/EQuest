using System;
using Microsoft.Xna.Framework;
using S3DFileLib;
using S3DFileLib.WldFrags;

namespace EQuestRender.Animations
{
	public class AnimationHelper
	{
		public static bool Debug = false;
		
		public static string ExtractPieceName(string fragName)
		{
			if (fragName == null)
				return null;

			string result = fragName;

			if (fragName.EndsWith("_TRACKDEF"))
				result = result.Substring(0, result.Length - 9);

			if (result.Length > 3 && result[1] >= '0' && result[1] <= '9')
				result = result.Substring(3, result.Length - 3);

			if (result.Length == 3)
				result = "ROOT";
			else if (result.Length > 3)
				result = result.Substring(3, result.Length - 3);
			else
				result = "UNKNOWN";

			return result;
		}
		
		public static void TransformSkeleton(string modelName, SkeletonPiece[] skeleton)
		{
			if (skeleton.Length == 0)
			{
				RenderLog.Debug("Unable to transform skeleton with no pieces");
				return;
			}
			
			int frameCount = 0;
			DetermineFrameCount(skeleton[0], skeleton, ref frameCount);
			TransformSkeletonPieceAndChildren(modelName, skeleton, skeleton[0], null, frameCount);
		}

		private static void TransformSkeletonPieceAndChildren(string modelName, SkeletonPiece[] skeleton, SkeletonPiece piece, SkeletonPiece parent, 
			int frameCount)
		{
			if (piece == null)
				return;

			EnsurePieceHasFrameCount(piece, frameCount);

			// Transform each frame.
			for (int i = 0; i < frameCount; i++)
			{
				SkeletonPieceFrame frame = piece.Frames[i];
				SkeletonPieceFrame parentFrame = parent != null ? parent.Frames[i] : null;
				TransformFrame(modelName, frame, parentFrame);
			}

			// Transform child pieces.
			if (piece.Children != null)
			{
				foreach (var childIndex in piece.Children)
					TransformSkeletonPieceAndChildren(modelName, skeleton, skeleton[childIndex], piece, frameCount);
			}
		}

		private static void TransformFrame(string modelName, SkeletonPieceFrame frame, SkeletonPieceFrame parentFrame)
		{
			// Calculate the shift values for this frame.
			if (frame.ShiftDenom != 0)
			{
				frame.Shift.X = frame.ShiftXNum / (float)frame.ShiftDenom;
				frame.Shift.Y = frame.ShiftYNum / (float)frame.ShiftDenom;
				frame.Shift.Z = frame.ShiftZNum / (float)frame.ShiftDenom;
			}

			// Add the parent rotation and shift.
			if (parentFrame != null)
			{ 
				frame.Shift = Vector3.Transform(frame.Shift, parentFrame.Rotate);
				frame.Shift += parentFrame.Shift;
			}
			// Create the shift matrix.
			Matrix matShift = Matrix.CreateTranslation(new Vector3(frame.Shift.X, frame.Shift.Y, frame.Shift.Z));

			// Calculate the rotation quaternion.
			frame.QW = frame.QWShort / 16384f;
			frame.QX = frame.QXShort / 16384f;
			frame.QY = frame.QYShort / 16384f;
			frame.QZ = frame.QZShort / 16384f;
			frame.Rotate = new Quaternion(frame.QX, frame.QY, frame.QZ, frame.QW);

			// Rotate by the parent rotation quaternion.
			if (parentFrame != null)
				frame.Rotate = Quaternion.Multiply(parentFrame.Rotate, frame.Rotate);

			// Create the rotation matrix.
			Matrix matRotate = Matrix.CreateFromQuaternion(frame.Rotate);

			// Create the transform matrix.
			frame.Transform = matRotate * matShift;
		}

		public static SkeletonPieceFrame MakeSkelPieceFrame(Frag12Frame frag12)
		{
			return new SkeletonPieceFrame
			{
				QWShort = frag12.QW,
				QXShort = frag12.QX,
				QYShort = frag12.QY,
				QZShort = frag12.QZ,
				ShiftXNum = frag12.ShiftX,
				ShiftYNum = frag12.ShiftY,
				ShiftZNum = frag12.ShiftZ,
				ShiftDenom = frag12.ShiftDenom,
			};
		}

		public static Frag12 FindAnimationSkeleton(string fragName, WLDFile wld)
		{
			return wld.FindFragByName(fragName) as Frag12;
		}

		private static void EnsurePieceHasFrameCount(SkeletonPiece piece, int frameCount)
		{
			int framesMissing = frameCount - piece.Frames.Count;

			if (framesMissing > 0)
			{
				SkeletonPieceFrame lastFrame = piece.Frames[piece.Frames.Count - 1];

				for (int i = 0; i < framesMissing; i++)
					piece.Frames.Add(lastFrame.Copy());
			}
		}

		private static void DetermineFrameCount(SkeletonPiece piece, SkeletonPiece[] skeleton, ref int count)
		{
			if (piece == null)
				return;

			if (piece.Frames.Count > count)
				count = piece.Frames.Count;

			foreach (int childIndex in piece.Children)
				DetermineFrameCount(skeleton[childIndex], skeleton, ref count);
		}

		public static void PrintSkeleton(SkeletonPiece piece, SkeletonPiece[] skeleton, int level)
		{
			var sb = new System.Text.StringBuilder();
			sb.Append('-', level + 1);

			sb.Append(string.Format(
				" Piece: {0}, VIndex: {1}, VCount: {2}, Frames: {3}, Children: {4}, Mesh Name: {5}, Mesh Verts: {6}",
				piece.Name, piece.VertIndex, piece.VertCount, piece.Frames.Count, piece.Children.Length, piece.Mesh?.Name,
				piece.Mesh?.Frames[0].Vertices.Length));

			RenderLog.Debug(sb.ToString());

			foreach (int childIndex in piece.Children)
				PrintSkeleton(skeleton[childIndex], skeleton, level + 1);
		}
	}
}
