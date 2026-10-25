using System;

namespace EQuestClient.Framework
{
	public interface IKeyInputListener
	{
		void OnKeyDown(KeyDownEventArgs args);
		void OnKeyUp(KeyUpEventArgs args);
	}
}
