using System;

namespace EQuestLib.Data
{
	public class ModelInfo
	{
		public ModelInfo(string name, float size)
		{
			this.Name = name;
			this.Size = size;
		}
		
		public string Name { get; private set; }
		public float Size { get; private set; }
	}
}
