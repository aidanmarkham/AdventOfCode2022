using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace AdventOfCode2022
{
	public class Visualizer
	{
		// what we modify in between draws
		private ConsolePixel[,] buffer;

		// the current state of the screen
		private ConsolePixel[,] screen;

		private int width;

		private int height;

		public Visualizer()
		{
			Console.CursorVisible = false;
			width = Console.WindowWidth;
			height = Console.WindowHeight; 
			Console.SetWindowSize(width, height);
			Console.SetBufferSize(width, height+1);
			buffer = new ConsolePixel[width, height];
			screen = new ConsolePixel[width, height];
		}

		public Visualizer(int w, int h)
		{
			Console.CursorVisible = false;
			width = w;
			height = h;
			Console.SetWindowSize(width, height);
			Console.SetBufferSize(width, height+1); 
			buffer = new ConsolePixel[width, height];
			screen = new ConsolePixel[width, height];
		}

		public void Draw()
		{
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					if (buffer[x, y].Equals(screen[x, y]) == false)
					{
						// copy the buffer to our screen array 
						screen[x, y] = buffer[x, y];

						// draw the buffer to the screen
						RenderPixel(buffer[x, y], x, y);
					}
				}
			}
		}

		public void Point((int x, int y) pos, char c, ConsoleColor f, ConsoleColor b)
		{
			if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height) return;

			buffer[pos.x, pos.y].character = c;
			buffer[pos.x, pos.y].foreground = f;
			buffer[pos.x, pos.y].background = b;
		}

		public void Fill(char c, ConsoleColor f, ConsoleColor b)
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Point((x, y), c, f, b);
				}
			}
		}

		public void Wipe(ConsoleColor color)
		{
			Console.BackgroundColor = color;
			Console.Clear();
		}

		private void RenderPixel(ConsolePixel pixel, int x, int y)
		{
			Console.SetCursorPosition(x, y);
			Console.ForegroundColor = pixel.foreground;
			Console.BackgroundColor = pixel.background;
			Console.Write(pixel.character);
			Console.CursorVisible = false;
		}

		private struct ConsolePixel
		{
			public char character;
			public ConsoleColor foreground;
			public ConsoleColor background;

			public ConsolePixel(char c, ConsoleColor f, ConsoleColor b)
			{
				character = c;
				foreground = f;
				background = b;
			}
			public override string ToString()
			{
				return foreground.ToString() + " " + character;
			}
		}
	}
}
