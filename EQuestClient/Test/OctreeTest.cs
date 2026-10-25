using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using EQuestLib.Octrees;
using EQuestClient.Game;

namespace EQuestClient.Test
{
	public class OctreeTest
	{
		public void Run(GameApp game)
		{
			string zoneName = "qeytoqrg";
			Debug.WriteLine("Loading zone " + zoneName);
			var zone = new Zone(game.AssetsPath, zoneName, null, game.Render);
			Debug.WriteLine("Zone has " + zone.Terrain.Count + " terrain models");
			
			var builder = new OctreeBuilder();
			var tree = builder.MakeOctee(zone.Terrain, zone.Objects);

			bool check0 = tree.Collision.BoxCollidesWithObjectInTree(new BoundingBox(new Vector3(0, 0, 0), new Vector3(10, 10, 10)));
			Debug.WriteLine("Box 1 collides: " + check0);
			
			bool check1 = tree.Collision.BoxCollidesWithObjectInTree(new BoundingBox(new Vector3(-10, -10, -10), new Vector3(30, 30, 30)));
			Debug.WriteLine("Box 1 collides: " + check1);

			bool check2 = tree.Collision.BoxCollidesWithObjectInTree(new BoundingBox(new Vector3(0, 0, 0), new Vector3(100, 100, 100)));
			Debug.WriteLine("Box 1 collides: " + check2);
		}
	}
}
