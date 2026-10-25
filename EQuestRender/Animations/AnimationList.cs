using System;
using System.Collections.Generic;
using S3DFileLib;
using S3DFileLib.WldFrags;

namespace EQuestRender.Animations
{
	/// <summary>
	/// Reads and stores a list of (non-base) animations from a WLD file.
	/// </summary>
	public class AnimationList
	{
		private Dictionary<string, AnimationDef> _modelDefs = new Dictionary<string, AnimationDef>();
		private WLDFile _wld;

		public AnimationList(WLDFile wld)
		{
			_wld = wld;
		}

		public void LoadAnimations()
		{
			_modelDefs.Clear();
			
			foreach (var parentFrag in _wld.Frags)
			{
				if (parentFrag.FragType == 0x13)
				{
					Frag12 frag = _wld.FindFragByRef(((Frag13)parentFrag).Ref) as Frag12;

					if (frag != null)
						AddEntry(frag);
				}
			}

			// PrintAnimations();
		}

		public AnimationDef FindAnimationDef(string name)
		{
			return _modelDefs.ContainsKey(name) ? _modelDefs[name] : null;
		}

		private void AddEntry(Frag12 frag)
		{
			// Make sure this frag is not a base animation.
			if (!IsNonBaseAnimation(frag.FragName))
				return;

			// Determine the model name and the animation name.
			string modelName = frag.FragName.Substring(3, 3);
			string animationName = frag.FragName.Substring(0, 3);

			// Get or create a model def for this piece.
			var modelDef = FindAnimationDef(modelName);

			if (modelDef == null)
			{
				modelDef = new AnimationDef(modelName);
				_modelDefs.Add(modelName, modelDef);
			}

			// Get or create an animation def for this piece.
			var animDef = modelDef.FindAnimation(animationName);

			if (animDef == null)
			{
				animDef = new AnimationDefAnimation(animationName);
				modelDef.Animations.Add(animationName, animDef);
			}

			// Add the piece.
			animDef.Pieces.Add(new AnimationDefPiece(frag));
		}

		private bool IsNonBaseAnimation(string fragName)
		{
			if (fragName.Length < 3)
				return false;

			return fragName[1] >= '0' && fragName[1] <= '9';
		}

		private void PrintAnimations()
		{
			var sb = new System.Text.StringBuilder();
			
			foreach (var modelDef in _modelDefs.Values)
			{
				sb.Append("Model: " + modelDef.ModelName + " Animations: ");
					
				foreach (var anim in modelDef.Animations.Values)
				{
					sb.Append(anim.AnimationName + " (" + anim.Pieces.Count + ")");
				}
			}

			sb.AppendLine();
			RenderLog.Debug(sb.ToString());
		}
	}
}
