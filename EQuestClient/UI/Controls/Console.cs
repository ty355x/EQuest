using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EQuestRender;

namespace EQuestClient.UI.Controls
{
	public struct ConsoleLine
	{
		public ConsoleLine(string text, Color color)
		{
			this.Text = text;
			this.Color = color;
		}

		public string Text;
		public Color Color;
	}

	public class Console : Control
	{
		public static int ConsolePadding = 10;
		public event EventHandler<string> ConsoleTextSubmitted;

		public SpriteFont TextFont { get; set; }
		public Image Background { get; set; }
		public int MaxLines { get; set; }

		private string _inputBuffer = "";
		private List<ConsoleLine> _lines = new List<ConsoleLine>();
		private int _textHeight = 20;

		public Console()
		{
			TextFont = Viewport.DefaultTextFont;
			MaxLines = 4;
		}

		public void DetermineMaxLines()
		{
			if (TextFont == null)
				return;
			
			_textHeight = (int) TextFont.MeasureString("M").Y + 4;
			int contentHeight = (int)(Bounds.Height - _textHeight - 12);
			MaxLines = (int)(contentHeight / _textHeight);
		}

		public void Write(string text)
		{
			Write(text, ConsoleColors.Default);
		}

		public void Write(string text, Color color)
		{
			if (TextFont == null)
				return;
			
			List<string> lines = new List<string>();
			string currentLine = "";
			
			foreach (char c in text)
			{
				currentLine += c;

				if (TextFont.MeasureString(currentLine).X >= (Bounds.Width - 20))
				{
					lines.Add(currentLine);
					currentLine = "";
				}
			}

			if (currentLine != "")
				lines.Add(currentLine);

			AddLines(lines, color);
		}

		public void AddLines(List<string> lines, Color color)
		{
			foreach (var line in lines)
			{
				if (_lines.Count > 0 && _lines.Count >= MaxLines)
					_lines.RemoveAt(0);

				_lines.Add(new ConsoleLine(line, color));
			}
		}

		public void StartConsoleCapture()
		{
			_inputBuffer = "|";
		}

		public void AddConsoleInput(int asciiCode)
		{
			if (asciiCode == 8 && _inputBuffer.Length > 1)
				_inputBuffer = _inputBuffer.Substring(0, _inputBuffer.Length - 2) + "|";
			else if (asciiCode != 8)
				AddConsoleInput(new string((char)asciiCode, 1));
		}

		public void AddConsoleInput(string key)
		{
			if (_inputBuffer.Length == 31)
				return;

			_inputBuffer = _inputBuffer.Substring(0, _inputBuffer.Length - 1) + key + "|";
		}

		public void SubmitConsoleInput()
		{
			if (_inputBuffer.Length > 1)
			{
				string consoleText = _inputBuffer.Substring(0, _inputBuffer.Length - 1);

				if (ConsoleTextSubmitted != null)
					ConsoleTextSubmitted(this, consoleText);
			}

			_inputBuffer = "";
		}

		public void CancelConsoleInput()
		{
			_inputBuffer = "";
		}

		public override void Draw(Render render, GameTime time)
		{
			if (Background != null)
				Background.Draw(render, new Rectangle(ParentWindow.X + X, ParentWindow.Y + Y, Width, Height), Color.White);

			if (TextFont != null)
			{
				int y = ParentWindow.Y + Bounds.Y - Console.ConsolePadding;

				foreach (ConsoleLine line in _lines)
					DrawText(line.Text, render, Bounds.X + Console.ConsolePadding, y += _textHeight, line.Color);
			}

			if (_inputBuffer.Length > 0)
			{
				DrawText(_inputBuffer, render, ParentWindow.X + Bounds.X + Console.ConsolePadding,
					ParentWindow.Y + Bounds.Y + Bounds.Height - _textHeight - 4, ConsoleColors.Default);
			}
		}

		private void DrawText(string text, Render render, int x, int y, Color color)
		{
			render.DrawText(TextFont, text, new Vector2(x, y), color);
		}
	}
}
