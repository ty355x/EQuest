using Microsoft.Xna.Framework;

namespace EQuestRender
{
	public class SkeletonPieceFrame
	{
		public short QWShort;
		public short QXShort;
		public short QYShort;
		public short QZShort;
		public short ShiftXNum;
		public short ShiftYNum;
		public short ShiftZNum;
		public short ShiftDenom;

		public float QW;
		public float QX;
		public float QY;
		public float QZ;

		public Vector3 Shift = Vector3.Zero;
		public Quaternion Rotate = Quaternion.Identity;

		public Matrix Transform;

		public SkeletonPieceFrame Copy()
		{
			return new SkeletonPieceFrame
			{
				QWShort = this.QWShort,
				QXShort = this.QXShort,
				QYShort = this.QYShort,
				QZShort = this.QZShort,
				ShiftXNum = this.ShiftXNum,
				ShiftYNum = this.ShiftYNum,
				ShiftZNum = this.ShiftZNum,
				ShiftDenom = this.ShiftDenom,
				QW = this.QW,
				QX = this.QX,
				QY = this.QY,
				QZ = this.QZ,
				Shift = this.Shift,
				Rotate = this.Rotate,
				Transform = this.Transform,
			};
		}
	}
}
