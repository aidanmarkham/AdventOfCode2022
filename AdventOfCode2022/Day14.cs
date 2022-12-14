using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace AdventOfCode2022
{
	class Day14
	{
		public static string inputFile = "cavern.txt";
		public static void Solve()
		{
			int margin = 400;

			// build map
			var input = File.ReadAllLines(inputFile);

			(int x, int y) min = (int.MaxValue, int.MaxValue);
			(int x, int y) max = (int.MinValue, int.MinValue);
			(int x, int y) sandEntry = (500, 0);

			// add sandentry to min
			if (sandEntry.x < min.x) min.x = sandEntry.x;
			if (sandEntry.x > max.x) max.x = sandEntry.x;
			if (sandEntry.y < min.y) min.y = sandEntry.y;
			if (sandEntry.y > max.y) max.y = sandEntry.y;

			// calculate max and min
			for (int i = 0; i < input.Length; i++)
			{
				var words = input[i].Split(new string[] { " -> " }, StringSplitOptions.None);

				for (int w = 0; w < words.Length; w += 1)
				{
					var coords = words[w].Split(',');
					int x = int.Parse(coords[0]);
					int y = int.Parse(coords[1]);

					if (x < min.x) min.x = x;
					if (x > max.x) max.x = x;
					if (y < min.y) min.y = y;
					if (y > max.y) max.y = y;
				}
			}

			// calculate width and height
			int width = max.x - min.x;
			int height = max.y - min.y;
			Console.WriteLine("Min: " + min + " Max: " + max + " Width: " + width + " Height: " + height);

			// adjust sand entry with margin
			sandEntry.x -= min.x - (margin / 2);
			sandEntry.y -= min.y - (margin / 2);

			// figure out floor 
			var floor = max;
			floor.x += (margin / 2);
			floor.y += margin / 2;
			floor.y += 2;


			// create map
			char[,] map = new char[width + margin, height + margin];

			// fill map
			for (int y = 0; y < map.GetLength(1); y++)
			{
				for (int x = 0; x < map.GetLength(0); x++)
				{
					if (y == floor.y)
					{
						map[x, y] = '█';
					}
					else
					{
						map[x, y] = ' ';
					}
				}
			}

			// build level
			for (int i = 0; i < input.Length; i++)
			{
				var words = input[i].Split(new string[] { " -> " }, StringSplitOptions.None);

				for (int w = 0; w < words.Length - 1; w++)
				{
					(int x, int y) start = (0, 0);
					(int x, int y) end = (0, 0);

					var coords = words[w].Split(',');
					start.x = int.Parse(coords[0]) - min.x + (margin / 2);
					start.y = int.Parse(coords[1]) - min.y + (margin / 2);

					coords = words[w + 1].Split(',');
					end.x = int.Parse(coords[0]) - min.x + (margin / 2);
					end.y = int.Parse(coords[1]) - min.y + (margin / 2);

					if (end.x > start.x)
					{
						for (int x = start.x; x <= end.x; x++)
						{
							map[x, start.y] = '█';
						}
					}
					else
					{
						for (int x = end.x; x <= start.x; x++)
						{
							map[x, start.y] = '█';
						}
					}

					if (end.y > start.y)
					{
						for (int y = start.y; y <= end.y; y++)
						{
							map[start.x, y] = '█';
						}
					}
					else
					{
						for (int y = end.y; y <= start.y; y++)
						{
							map[start.x, y] = '█';
						}
					}
				}
			}


			// run sand sim
			bool complete = false;
			int grains = 0;
			List<(int x, int y)> sandPath = new List<(int x, int y)>();
			Console.Clear();
			Console.CursorVisible = false;
			int sandScreenPos = 0;
			int screenOffset = 999;
			(int x, int y) prevSand = (0, 0);

			while (!complete)
			{
				(int x, int y) sand = sandEntry;

				bool atRest = false;
				grains++;

				sandPath.Clear();

				while (!atRest)
				{
					// blocked below
					if (map[sand.x, sand.y + 1] != ' ')
					{
						//check left for air
						if (map[sand.x - 1, sand.y + 1] == ' ')
						{
							prevSand = sand;
							sand.x--;
							sand.y++;
						}
						//check right for air
						else if (map[sand.x + 1, sand.y + 1] == ' ')
						{
							prevSand = sand;
							sand.x++;
							sand.y++;
						}
						// otherwise we're at rest
						else
						{
							prevSand = sandEntry;
							map[sand.x, sand.y] = '░';
							atRest = true;
						}
					}
					// air below
					else if (map[sand.x, sand.y + 1] == ' ')
					{
						prevSand = sand;
						sand.y++;
					}
					else
					{
						prevSand = sandEntry;
						map[sand.x, sand.y] = '░';
						atRest = true;
					}

					if (atRest && sand == sandEntry)
					{
						prevSand = sandEntry;
						map[sand.x, sand.y] = '░';
						complete = true;
					}

					sandPath.Add(sand);			
				}

				sandScreenPos = sand.y - (margin / 2) - (Console.WindowHeight / 2);
				if (sandScreenPos > screenOffset + (Console.WindowHeight / 2) || sandScreenPos < screenOffset - (Console.WindowHeight / 2))
				{
					screenOffset = sandScreenPos;
					Draw(margin, map, (999, 999), prevSand, sandEntry, screenOffset, true, sandPath);
				}
				else
				{
					Draw(margin, map, sand, prevSand, sandEntry, screenOffset, false, sandPath);
				}
			}

			

			Draw(margin, map, (999, 999), (999, 999), sandEntry, screenOffset, true);
			Console.WriteLine("Total Grains: " + (grains));
		}

		static void Draw(int margin, char[,] map, (int x, int y) sand, (int x, int y) prevSand, (int, int) sandEntry, int vertOffset, bool drawAll, List<(int x,int y)> sandpath = null)
		{
			Console.SetCursorPosition(0, 0);

			int maxHeight = Console.WindowHeight - 1;
			int maxWidth = Console.WindowWidth - 1;

			int minX = (margin / 2) - 10;
			int maxX = Math.Min(map.GetLength(0) - (margin / 2) + 10, minX + maxWidth);

			int minY = (margin / 2) - 1 + vertOffset;
			int maxY = Math.Min(map.GetLength(1) - (margin / 2) + 2 + vertOffset, minY + maxHeight);

			if (!drawAll)
			{
				if (sand.x >= minX && sand.x <= maxX && sand.y >= minY && sand.y <= maxY)
				{
					var x = sand.x - minX;
					var y = sand.y - minY;
					Console.SetCursorPosition(x, y);
					Console.Write('░');
					Thread.Sleep(1);
				}
				if (prevSand.x >= minX && prevSand.x <= maxX && prevSand.y >= minY && prevSand.y <= maxY)
				{
					var x = prevSand.x - minX;
					var y = prevSand.y - minY;
					Console.SetCursorPosition(x, y);
					//Console.Write(map[x, y]);
				}
				if (sandpath != null)
				{
					for (int i = 0; i < sandpath.Count; i++)
					{
						if (sandpath[i].x >= minX && sandpath[i].x <= maxX && sandpath[i].y >= minY && sandpath[i].y <= maxY)
						{
							var x = sandpath[i].Item1 - minX;
							var y = sandpath[i].Item2 - minY;
							Console.SetCursorPosition(x, y);
							Console.Write('░');
						}
					}
				}
			}
			else
			{
				for (int y = minY; y <= maxY; y++)
				{
					for (int x = minX; x <= maxX; x++)
					{
						Console.SetCursorPosition(x - minX, y - minY);
						if ((x, y) == sand)
						{
							Console.Write('░');
						}
						else if(sandpath != null && sandpath.Contains((x,y)))
						{
							Console.Write('░');
						}
						else if ((x, y) == prevSand)
						{
							Console.Write(map[x, y]);
						}
						else if (map[x, y] == '█')
						{
							Console.Write('█');
						}
						else if ((x, y) == sandEntry)
						{
							Console.Write('x');
						}
						else
						{
							Console.Write(map[x, y]);
						}
					}
					Console.Write('\n');
				}
			}
		}
	}
}
