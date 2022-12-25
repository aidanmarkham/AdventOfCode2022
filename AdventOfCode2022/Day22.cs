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
			char[,] map = new char[longest, input.Length - 2];

			// get the steps string
			string stepString = input[input.Length - 1];

			// empty the map
			for (int y = 0; y < map.GetLength(1); y++)
			{
				for (int x = 0; x < map.GetLength(0); x++)
				{
					map[x, y] = ' ';
				}
			}

			// populate the map
			for (int i = 0; i < input.Length - 2; i++)
			{
				for (int j = 0; j < input[i].Length; j++)
				{
					map[j, i] = input[i][j];
				}
			}

			// find the starting location 
			(int x, int y) startingLocation = (0, 0);

			// 0R, 1D, 2L, 3U
			int direction = 0;
			for (int i = 0; i < map.GetLength(0); i++)
			{
				if (map[i, 0] == '.')
				{
					startingLocation = (i, 0);
					break;
				}
			}

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
			for (int i = 0; i < steps.Count; i++)
			{
				// let's walk
				if (int.TryParse(steps[i], out var stepCount))
				{
					for (int j = 0; j < stepCount; j++)
					{
						// tryMove
						var nextPos = currentPos;
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
						// we're off the left edge
						if (nextPos.x < 0)
						{
							nextPos.x = map.GetLength(0) - 1;
						}
						// we're off the right edge
						if (nextPos.x >= map.GetLength(0))
						{
							nextPos.x = 0;
						}
						// we're off the top edge
						if (nextPos.y < 0)
						{
							nextPos.y = map.GetLength(1) - 1;
						}
						// we're off the bottom edge 
						if (nextPos.y >= map.GetLength(1))
						{
							nextPos.y = 0;
						}

						// sample map
						var mapChar = map[nextPos.x, nextPos.y];
						switch (mapChar)
						{
							case ' ': // void
								var loopPos = nextPos;
								// nextPos is always next to current pos, so if we go until we hit current, we'll know we've looped around
								bool found = false;
								while (loopPos != currentPos && !found)
								{
									// move in direction
									switch (direction)
									{
										case 0:
											loopPos.x++;
											break;
										case 1:
											loopPos.y++;
											break;
										case 2:
											loopPos.x--;
											break;
										case 3:
											loopPos.y--;
											break;
									}

									// ====== loop around map edges =========
									// we're off the left edge
									if (loopPos.x < 0)
									{
										loopPos.x = map.GetLength(0) - 1;
									}
									// we're off the right edge
									if (loopPos.x >= map.GetLength(0))
									{
										loopPos.x = 0;
									}
									// we're off the top edge
									if (loopPos.y < 0)
									{
										loopPos.y = map.GetLength(1) - 1;
									}
									// we're off the bottom edge 
									if (loopPos.y >= map.GetLength(1))
									{
										loopPos.y = 0;
									}

									var loopMapChar = map[loopPos.x, loopPos.y];
									switch (loopMapChar)
									{
										case '.': // open space
											nextPos = loopPos;
											found = true;
											break;
										case '#': // wall
											nextPos = currentPos;
											found = true;
											break;
									}
								}
								break;
							case '.': // open space
									  // do nothing, just move babey
								break;
							case '#': // wall
									  // don't move if it would go into the wall 
								nextPos = currentPos;
								break;
						}
						currentPos = nextPos;
						positions.Add(currentPos);
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


			DrawMap(map, positions);

			int row = currentPos.y + 1;
			int column = currentPos.x + 1;
			Console.WriteLine("Final Data:\nRow: " + row + " Column: " + column + " Direction: " + direction);

			int password = (1000 * row) + (4 * column) + direction;
			Console.WriteLine("Password: " + password);
		}

		static void DrawMap(char[,] map, List<(int x, int y)> positions)
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
						string face = GetFace(map, (x, y)).ToString();
						Console.Write(face != "0" ? face : " ");

					}
				}
				Console.Write("\n");
			}
		}

		static int GetFace(char[,] map, (int x, int y) pos)
		{
			int hQuad = map.GetLength(1) / 3;
			int wQuad = map.GetLength(0) / 4;

			int x = pos.x;
			int y = pos.y;

			var xQuad = x / wQuad;
			var yQuad = y / hQuad;

			if (yQuad == 0)
			{
				return xQuad == 2 ? 1 : 0;
			}
			else if (yQuad == 1)
			{
				return xQuad + 2 < 5 ? xQuad + 2 : 0;
			}
			else if (yQuad == 2)
			{
				return xQuad + 3 > 4 ? xQuad + 3 : 0;
			}

			return 0;
		}
	}
}
