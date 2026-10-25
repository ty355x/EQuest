using System;

namespace EQuestClient.Framework
{
	public interface IMouseInputListener
	{
		void OnMouseButtonPressed(MouseEventArgs args);
		void OnMouseButtonReleased(MouseEventArgs args);
	}
}
