using System;
using System.Collections.Generic;

namespace EQuestClient.AssetManagement
{
	class AnimationDependencies
	{
		public Dictionary<string, string> Dependencies { get; private set; }

		public AnimationDependencies()
		{
			Dependencies = new Dictionary<string, string>();
			MakeDependencies();
		}
		
		private void MakeDependencies()
		{
			Add("BAF", "ELF");
			Add("BAM", "ELM");
			Add("BET", "SPI");
			Add("DAF", "ELF");
			Add("DAM", "ELM");
			Add("DWF", "DWM");
			Add("ELE", "ELM");
			Add("ELF", "ELM");
			Add("ERF", "ELF");
			Add("ERM", "ELM");
			Add("GNF", "DWM");
			Add("GNM", "DWM");
			Add("GNN", "ELM");
			Add("HAM", "ELM");
			Add("HAF", "ELF");
			Add("HIF", "ELF");
			Add("HIM", "ELM");
			Add("HLM", "ELM");
			Add("HLF", "ELM");
			Add("HOF", "ELF");
			Add("HOM", "ELM");
			Add("HUF", "ELF");
			Add("HUM", "ELM");
			Add("IMP", "GAR");
			Add("IVM", "HUM");
			Add("KOB", "WER");
			Add("OGM", "OGF");
			Add("SKE", "ELM");
			Add("TRF", "OGF");
			Add("TRM", "OGF");
			Add("WER", "ELM");
			Add("WOE", "ELM");
			Add("QCF", "ELF");
			Add("QCM", "ELM");
			Add("ZOM", "ELM");
		}

		private AnimationDependencies Add(string model1, string model2)
		{
			Dependencies[model1] = model2;
			return this;
		}
	}
}
