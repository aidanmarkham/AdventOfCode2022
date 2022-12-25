using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class Day22
	{
		public static string inputFile = "path_test.txt";

		public static void Solve()
		{
			// read in input file
			var input = File.ReadAllLines(inputFile);

			// figure out what the longest file is 
			int longest = 0;
			for (int i = 0; i < input.Length - 2; i++)
			{
				if (input[i].Length > longest) longest = input[i].Length;
			}

			// create the map 
			char[,] largeMap = new char[longest, input.Length - 2];

			// get the steps string
			string stepString = input[input.Length - 1];

			// empty the map
			for (int y = 0; y < largeMap.GetLength(1); y++)
			{
				for (int x = 0; x < largeMap.GetLength(0); x++)
				{
					largeMap[x, y] = ' ';
				}
			}

			// populate the map
			for (int i = 0; i < input.Length - 2; i++)
			{
				for (int j = 0; j < input[i].Length; j++)
				{
					largeMap[j, i] = input[i][j];
				}
			}

			// create maptiles
			List<MapTile> tiles = new List<MapTile>();
			int width = largeMap.GetLength(0) / 4;
			int height = largeMap.GetLength(1) / 3;

			var one = new MapTile(width * 2, 0, width, height);
			var two = new MapTile(0, height, width, height); //2
			var three = new MapTile(width, height, width, height); //3
			var four = new MapTile(width * 2, height, width, height); //4
			var five = new MapTile(width * 2, height * 2, width, height); //5
			var six = new MapTile(width * 3, height * 2, width, height); //6

			one.name = "one";
			one.up = five;
			one.down = four;
			one.left = one;
			one.right = one;

			two.name = "two";
			two.up = two;
			two.down = two;
			two.left = four;
			two.right = three;

			three.name = "three";
			three.up = three;
			three.down = three;
			three.left = two;
			three.right = four;

			four.name = "four";
			four.up = one;
			four.down = five;
			four.left = three;
			four.right = two;

			five.name = "five";
			five.up = four;
			five.down = one;
			five.left = six;
			five.right = six;

			six.name = "six";
			six.up = six;
			six.down = six;
			six.left = five;
			six.right = five;

			tiles.Add(one);
			tiles.Add(two);
			tiles.Add(three);
			tiles.Add(four);
			tiles.Add(five);
			tiles.Add(six);

			// find the starting location 
			(int x, int y) startingLocation = (0, 0);

			// 0R, 1D, 2L, 3U
			int direction = 0;
			for (int i = 0; i < largeMap.GetLength(0); i++)
			{
				if (largeMap[i, 0] == '.')
				{
					startingLocation = (i, 0);
					break;
				}
			}

			// ========= build list of steps ===================
			List<string> steps = new List<string>();
			string currentStep = "";
			for (int i = 0; i < stepString.Length; i++)
			{
				if (!char.IsDigit(stepString[i]) && currentStep.Length > 0)
				{
					steps.Add(currentStep);
					steps.Add(stepString[i] + "");
					currentStep = "";
				}
				else
				{
					currentStep += stepString[i];
				}
			}
			steps.Add(currentStep);

			// record positions for debug
			List<(int x, int y)> positions = new List<(int x, int y)>();
			positions.Add(startingLocation);

			var currentPos = startingLocation;
			var currentTile = one;
			for (int i = 0; i < steps.Count; i++)
			{
				// let's walk
				if (int.TryParse(steps[i], out var stepCount))
				{
					for (int j = 0; j < stepCount; j++)
					{
						// tryMove
						var nextPos = currentPos;
						var nextTile = currentTile;

						switch (direction)
						{
							case 0:
								nextPos.x++;
								break;
							case 1:
								nextPos.y++;
								break;
							case 2:
								nextPos.x--;
								break;
							case 3:
								nextPos.y--;
								break;
						}

						// ====== loop around map edges =========

						if (!currentTile.Contains(nextPos))
						{
							switch (direction)
							{
								case 0:
									nextTile = currentTile.right;
									break;
								case 1:
									nextTile = currentTile.down;
									break;
								case 2:
									nextTile = currentTile.left;
									break;
								case 3:
									nextTile = currentTile.up;
									break;
							}
							// transform position into new tile space
							nextPos = MapTile.TransformPoint(direction, nextPos, currentTile, nextTile);
						}

						// sample map
						var mapChar = largeMap[nextPos.x, nextPos.y];
						if (mapChar == '#')
						{
							nextPos = currentPos;
							nextTile = currentTile;
						}

						currentPos = nextPos;
						currentTile = nextTile;
						positions.Add(currentPos);

						DrawMap(largeMap, positions, tiles);
					}
				}
				else
				{
					// turn
					if (steps[i] == "L")
					{
						direction--;
					}
					if (steps[i] == "R")
					{
						direction++;
					}
					if (direction < 0)
					{
						direction = 3;
					}
					if (direction > 3)
					{
						direction = 0;
					}
				}
			}

			DrawMap(largeMap, positions, tiles);

			int row = currentPos.y + 1;
			int column = currentPos.x + 1;
			Console.WriteLine("Final Data:\nRow: " + row + " Column: " + column + " Direction: " + direction);

			int password = (1000 * row) + (4 * column) + direction;
			Console.WriteLine("Password: " + password);
		}

		static void DrawMap(char[,] map, List<(int x, int y)> positions, List<MapTile> tiles)
		{
			// display the map 
			for (int y = 0; y < map.GetLength(1); y++)
			{
				for (int x = 0; x < map.GetLength(0); x++)
				{
					if (positions.Contains((x, y)))
					{
						Console.Write('@');

					}
					else
					{

						bool drawn = false;
						for (int i = 0; i < tiles.Count; i++)
						{
							if (tiles[i].Contains((x, y)))
							{
								drawn = true;
								Console.Write(i + 1);
							}
						}
						if (!drawn) { Console.Write(map[x, y]); }

					}
				}
				Console.Write("\n");
			}
			Console.Write("\n");
		}

		class MapTile
		{
			public (int x, int y, int width, int height) rect;

			public MapTile(int x, int y, int width, int height)
			{
				rect.x = x;
				rect.y = y;
				rect.width = width;
				rect.height = height;
			}
			public string name;
			public MapTile up;
			public MapTile down;
			public MapTile left;
			public MapTile right;

			public bool Contains((int x, int y) pos)
			{
				return pos.x >= rect.x && pos.x < rect.x + rect.width && pos.y >= rect.y && pos.y < rect.y + rect.height;
			}
			public override string ToString()
			{
				return name;
			}
			public static (int x, int y) TransformPoint(int goingDir, (int x, int y) pos, MapTile current, MapTile next)
			{
				// calculate comingDir
				int comingDir = -1;
				if (next.left == current) comingDir = 0;
				if (next.up == current) comingDir = 1;
				if (next.right == current) comingDir = 2;
				if (next.down == current) comingDir = 3;

				// verify connection
				if (comingDir == -1)
				{
					Console.WriteLine("Next isn't connected to Current!");
					return (0, 0);
				}

				// transform position to tile space
				var cPos = pos;
				cPos.x -= current.rect.x;
				cPos.y -= current.rect.y;

				// transform by the direction in tile space
				switch (goingDir)
				{
					case 0:
						cPos.x -= current.rect.width;
						break;
					case 1:
						cPos.y -= current.rect.height;
						break;
					case 2:
						cPos.x += current.rect.width;
						break;
					case 3:
						cPos.y += current.rect.height;
						break;
				}

				// transform that postion back to world space
				var nPos = cPos;
				nPos.x += next.rect.x;
				nPos.y += next.rect.y;
				return nPos;
			}
		}
	}
}
