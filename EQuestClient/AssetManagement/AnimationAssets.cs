using System;
using System.Collections.Generic;
using S3DFileLib;
using EQuestRender;
using EQuestRender.Animations;

namespace EQuestClient.AssetManagement
{
	public class AnimationAssets
	{
		private AnimationList _globalAnimations;
		private AnimationList _zoneAnimations;
		
		private AssetManager _asssets;
		private Dictionary<string, string> _animationDependancies;
		
		public AnimationAssets(AssetManager assets)
		{
			_asssets = assets;
			
			var dependencies = new AnimationDependencies();
			_animationDependancies = dependencies.Dependencies;
		}
		
		public void LoadGlobalAnimations(WLDFile wld)
		{
			_globalAnimations = new AnimationList(wld);
			_globalAnimations.LoadAnimations();
		}

		public void ClearGlobalAnimations()
		{
			_globalAnimations = null;
		}

		public void LoadZoneAnimations(WLDFile wld)
		{
			_zoneAnimations = new AnimationList(wld);
			_zoneAnimations.LoadAnimations();
		}

		public void ClearZoneAnimations()
		{
			_zoneAnimations = null;
		}

		public void AssignAnimations(MobModel model)
		{
			// Assign model animations.
			AssignAnimations(model, model.Name);

			// If there is an animation dependency entry, assign dependent animations.
			if (_animationDependancies.ContainsKey(model.Name))
				AssignAnimations(model, _animationDependancies[model.Name]);
		}

		public void AssignAnimations(MobModel model, string animationDefName)
		{
			var def = FindAnimationDef(animationDefName);

			if (def == null)
			{
				// Log.Debug("Unable to find animation def " + animationDefName + " for model " + model.Name);
				return;
			}

			def.AssignAnimations(model);
		}

		private AnimationDef FindAnimationDef(string name)
		{
			AnimationDef def = null;

			if (_globalAnimations != null)
				def = _globalAnimations.FindAnimationDef(name);

			if (def == null && _zoneAnimations != null)
				def = _zoneAnimations.FindAnimationDef(name);

			return def;
		}
	}
}
