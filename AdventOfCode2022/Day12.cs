using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class Day12
	{
		public static string inputFile = "heightmap.txt";
		public static void Solve()
		{
			// build map
			var input = File.ReadAllLines(inputFile);
			char[,] map = new char[input[0].Length, input.Length];
			int[,] flooded = new int[input[0].Length, input.Length];

			(int, int) startPos = (0, 0);
			(int, int) endPos = (0, 0);
		
			for (int x = 0; x < input.Length; x++)
			{
				for (int y = 0; y < input[x].Length; y++)
				{
					var inputChar = input[x][y];

					if (char.IsUpper(inputChar))
					{
						if (inputChar == 'S')
						{
							// starting pos
							startPos = (y, x);
							map[y, x] = 'a';
							flooded[y, x] = 0;
						}
						else
						{
							endPos = (y, x);
							map[y, x] = 'z';
							flooded[y, x] = 9999;
						}
					}
					else
					{
						map[y, x] = input[x][y];
						flooded[y, x] = 9999;
					}

					
				}
			}

			int lowest = 9999;
			List<(int, int)> steps = new List<(int, int)>();
			for (int y = 0; y < map.GetLength(1); y++)
			{
				for (int x = 0; x < map.GetLength(0); x++)
				{
					if(map[x,y] == 'a')
					{
						ClearFlooded(flooded, (x, y));
						FillChildren(map, flooded, (x, y));
						steps = new List<(int, int)>();
						FindPath(steps, flooded, endPos);
						//Console.WriteLine(x + ","+ y + " " + steps.Count);
						if(steps.Count != 0 && steps.Count < lowest)
						{
							lowest = steps.Count;
						}
					}
				} 
			}

			
			Console.WriteLine("Lowest: " + lowest);


		}

		private static void DrawPath()
		{
			/*
			for (int y = 0; y < map.GetLength(1); y++)
			{
				for (int x = 0; x < map.GetLength(0); x++)
				{
					if (steps.Contains((x, y)))
					{
						Console.Write("░");
					}
					else
					{
						Console.Write(map[x, y]);
					}
				}
				Console.Write("\n");
			}
			*/
		}
		private static void ClearFlooded(int[,] flooded, (int, int) startPos)
		{
			for (int y = 0; y < flooded.GetLength(1); y++)
			{
				for (int x = 0; x < flooded.GetLength(0); x++)
				{
					if((x,y) == startPos)
					{
						flooded[x, y] = 0;
					}
					else
					{
						flooded[x, y] = 9999;
					}
				}
			}
		}

		private static void FillChildren(char[,] map, int[,] flooded, (int, int) pos)
		{
			var thisValue = flooded[pos.Item1, pos.Item2];

			// left
			if (pos.Item1 - 1 >= 0)
			{
				if (thisValue + 1 < flooded[pos.Item1 - 1, pos.Item2] && map[pos.Item1 - 1, pos.Item2] <= map[pos.Item1, pos.Item2] + 1)
				{
					flooded[pos.Item1 - 1, pos.Item2] = thisValue + 1;
					FillChildren(map, flooded, (pos.Item1 - 1, pos.Item2));
				}
			}

			//right
			if (pos.Item1 + 1 < map.GetLength(0))
			{
				if (thisValue + 1 < flooded[pos.Item1 + 1, pos.Item2] && map[pos.Item1 + 1, pos.Item2] <= map[pos.Item1, pos.Item2] + 1)
				{
					flooded[pos.Item1 + 1, pos.Item2] = thisValue + 1;

					FillChildren(map, flooded, (pos.Item1 + 1, pos.Item2));
				}
			}

			//up
			if (pos.Item2 - 1 >= 0)
			{
				if (thisValue + 1 < flooded[pos.Item1, pos.Item2 - 1] && map[pos.Item1, pos.Item2 - 1] <= map[pos.Item1, pos.Item2] + 1)
				{
					flooded[pos.Item1, pos.Item2 - 1] = thisValue + 1;

					FillChildren(map, flooded, (pos.Item1, pos.Item2 - 1));
				}
			}

			//down
			if (pos.Item2 + 1 < map.GetLength(1))
			{
				if (thisValue + 1 < flooded[pos.Item1, pos.Item2 + 1] && map[pos.Item1, pos.Item2 + 1] <= map[pos.Item1, pos.Item2] + 1)
				{
					flooded[pos.Item1, pos.Item2 + 1] = thisValue + 1;
					FillChildren(map, flooded, (pos.Item1, pos.Item2 + 1));
				}
			}

		}

		private static void FindPath(List<(int, int)> steps, int[,] flooded, (int,int) pos)
		{
			var thisValue = flooded[pos.Item1, pos.Item2];

			int lowest = 9999;

			// left
			if (pos.Item1 - 1 >= 0)
			{
				if(flooded[pos.Item1 - 1, pos.Item2] == thisValue - 1)
				{
					// this is the path 
					steps.Add(pos);
					FindPath(steps, flooded, (pos.Item1 - 1, pos.Item2));
					return;
				}
			}

			//right
			if (pos.Item1 + 1 < flooded.GetLength(0))
			{
				if (flooded[pos.Item1 + 1, pos.Item2] == thisValue - 1)
				{
					steps.Add(pos);
					FindPath(steps, flooded, (pos.Item1 + 1, pos.Item2));
					return;
				}
			}

			//up
			if (pos.Item2 - 1 >= 0)
			{
				if (flooded[pos.Item1, pos.Item2 - 1] == thisValue - 1)
				{
					steps.Add(pos);
					FindPath(steps, flooded, (pos.Item1, pos.Item2 - 1));
					return;
				}
			}

			//down
			if (pos.Item2 + 1 < flooded.GetLength(1))
			{
				if (flooded[pos.Item1, pos.Item2 + 1] == thisValue - 1)
				{
					steps.Add(pos);
					FindPath(steps, flooded, (pos.Item1, pos.Item2 + 1));
					return;
				}
			}
		}

		public static double Dist((int, int) a, (int, int) b)
		{
			return Math.Sqrt(Math.Pow(a.Item1 - b.Item1, 2) + Math.Pow(a.Item2 - b.Item2, 2));
		}

		public static int GetSpotCount(char[,] map, (int, int) currentPos, Dictionary<(int, int), int> visited)
		{
			char left = ' ';
			char right = ' ';
			char up = ' ';
			char down = ' ';

			// evaluate directions
			if (currentPos.Item1 - 1 >= 0)
			{
				left = map[currentPos.Item1 - 1, currentPos.Item2];
			}
			if (currentPos.Item1 + 1 < map.GetLength(0))
			{
				right = map[currentPos.Item1 + 1, currentPos.Item2];
			}
			if (currentPos.Item2 - 1 >= 0)
			{
				up = map[currentPos.Item1, currentPos.Item2 - 1];
			}
			if (currentPos.Item2 + 1 < map.GetLength(1))
			{
				down = map[currentPos.Item1, currentPos.Item2 + 1];
			}
			// figure out where is invalid
			if (left != ' ' && (int)left > (int)map[currentPos.Item1, currentPos.Item2] + 1)
			{
				//Console.WriteLine("Can't go left!");
				left = ' ';
			}
			if (right != ' ' && (int)right > (int)map[currentPos.Item1, currentPos.Item2] + 1)
			{
				//Console.WriteLine("Can't go right!");
				right = ' ';
			}
			if (up != ' ' && (int)up > (int)map[currentPos.Item1, currentPos.Item2] + 1)
			{
				//Console.WriteLine("Can't go up!");
				up = ' ';
			}
			if (down != ' ' && (int)down > (int)map[currentPos.Item1, currentPos.Item2] + 1)
			{
				//Console.WriteLine("Can't go down!");
				down = ' ';
			}

			// make sure we havn't been to these before
			if (visited.ContainsKey((currentPos.Item1 - 1, currentPos.Item2)))
			{
				left = ' ';
			}
			if (visited.ContainsKey((currentPos.Item1 + 1, currentPos.Item2)))
			{
				right = ' ';
			}
			if (visited.ContainsKey((currentPos.Item1, currentPos.Item2 - 1)))
			{
				up = ' ';
			}
			if (visited.ContainsKey((currentPos.Item1, currentPos.Item2 + 1)))
			{
				down = ' ';
			}

			int options = 0;
			if (left != ' ') options++;
			if (right != ' ') options++;
			if (up != ' ') options++;
			if (down != ' ') options++;
			return options;
		}
		public static void Solve1()
		{
			var input = File.ReadAllLines(inputFile);
			char[,] map = new char[input[0].Length, input.Length];

			(int, int) startPos = (0, 0);
			(int, int) endPos = (0, 0);

			for (int x = 0; x < input.Length; x++)
			{
				for (int y = 0; y < input[x].Length; y++)
				{
					var inputChar = input[x][y];

					if (char.IsUpper(inputChar))
					{
						if (inputChar == 'S')
						{
							// starting pos
							startPos = (y, x);
							map[y, x] = 'a';
						}
						else
						{
							endPos = (y, x);
							map[y, x] = 'z';
						}
					}
					else
					{
						map[y, x] = input[x][y];
					}
				}
			}

			var currentPos = startPos;
			List<(int, int)> steps = new List<(int, int)>();
			Dictionary<(int, int), int> visited = new Dictionary<(int, int), int>();
			steps.Add(currentPos);
			visited.Add(currentPos, 1);
			int iterations = 0;
			while (currentPos != endPos)
			{
				iterations++;
				char left = ' ';
				char right = ' ';
				char up = ' ';
				char down = ' ';

				// evaluate directions
				if (currentPos.Item1 - 1 >= 0)
				{
					left = map[currentPos.Item1 - 1, currentPos.Item2];
				}
				if (currentPos.Item1 + 1 < map.GetLength(0))
				{
					right = map[currentPos.Item1 + 1, currentPos.Item2];
				}
				if (currentPos.Item2 - 1 >= 0)
				{
					up = map[currentPos.Item1, currentPos.Item2 - 1];
				}
				if (currentPos.Item2 + 1 < map.GetLength(1))
				{
					down = map[currentPos.Item1, currentPos.Item2 + 1];
				}
				// figure out where is invalid
				if (left != ' ' && (int)left > (int)map[currentPos.Item1, currentPos.Item2] + 1)
				{
					//Console.WriteLine("Can't go left!");
					left = ' ';
				}
				if (right != ' ' && (int)right > (int)map[currentPos.Item1, currentPos.Item2] + 1)
				{
					//Console.WriteLine("Can't go right!");
					right = ' ';
				}
				if (up != ' ' && (int)up > (int)map[currentPos.Item1, currentPos.Item2] + 1)
				{
					//Console.WriteLine("Can't go up!");
					up = ' ';
				}
				if (down != ' ' && (int)down > (int)map[currentPos.Item1, currentPos.Item2] + 1)
				{
					//Console.WriteLine("Can't go down!");
					down = ' ';
				}

				// make sure we havn't been to these before
				if (visited.ContainsKey((currentPos.Item1 - 1, currentPos.Item2)))
				{
					left = ' ';
				}
				if (visited.ContainsKey((currentPos.Item1 + 1, currentPos.Item2)))
				{
					right = ' ';
				}
				if (visited.ContainsKey((currentPos.Item1, currentPos.Item2 - 1)))
				{
					up = ' ';
				}
				if (visited.ContainsKey((currentPos.Item1, currentPos.Item2 + 1)))
				{
					down = ' ';
				}


				int mostDirect = -1;
				double distance = double.PositiveInfinity;

				// left
				if (left != ' ' && Dist(endPos, (currentPos.Item1 - 1, currentPos.Item2)) < distance)
				{
					mostDirect = 0;
					distance = Dist(endPos, (currentPos.Item1 - 1, currentPos.Item2));
				}

				//right
				if (right != ' ' && Dist(endPos, (currentPos.Item1 + 1, currentPos.Item2)) < distance)
				{
					mostDirect = 1;
					distance = Dist(endPos, (currentPos.Item1 + 1, currentPos.Item2));
				}

				//up
				if (up != ' ' && Dist(endPos, (currentPos.Item1, currentPos.Item2 - 1)) < distance)
				{
					mostDirect = 2;
					distance = Dist(endPos, (currentPos.Item1, currentPos.Item2 - 1));
				}

				//down
				if (down != ' ' && Dist(endPos, (currentPos.Item1, currentPos.Item2 + 1)) < distance)
				{
					mostDirect = 3;
					distance = Dist(endPos, (currentPos.Item1, currentPos.Item2 + 1));
				}

				// go back if we run out of options
				if (mostDirect == -1 && steps.Count > 2)
				{
					foreach (KeyValuePair<(int, int), int> spot in visited)
					{
						if (GetSpotCount(map, spot.Key, visited) > 0)
						{
							currentPos = spot.Key;
							break;
						}
					}
				}
				else
				{
					switch (mostDirect)
					{
						case (0):
							//Console.WriteLine(steps.Count + ": Going Left to " + left);
							currentPos = (currentPos.Item1 - 1, currentPos.Item2);
							break;
						case (1):
							//Console.WriteLine(steps.Count + ": Going Right to " + right);
							currentPos = (currentPos.Item1 + 1, currentPos.Item2);
							break;
						case (2):
							//Console.WriteLine(steps.Count + ": Going Up + to " + up);
							currentPos = (currentPos.Item1, currentPos.Item2 - 1);
							break;
						case (3):
							//Console.WriteLine(steps.Count + ": Going Down to " + down);
							currentPos = (currentPos.Item1, currentPos.Item2 + 1);
							break;
					}
					steps.Add(currentPos);
					if (!visited.ContainsKey(currentPos))
					{
						visited.Add(currentPos, steps.Count);
					}
				}


				if (iterations % 1000 == 0)
				{
					Console.WriteLine("Iterations: " + iterations);
					Console.WriteLine("Visited: " + visited.Count + " out of " + (map.GetLength(0) * map.GetLength(1)));
					Console.WriteLine("Steps: " + steps.Count);
					for (int y = 0; y < map.GetLength(1); y++)
					{
						for (int x = 0; x < map.GetLength(0); x++)
						{
							if (currentPos == (x, y))
							{
								Console.Write("☃");
							}
							else if (steps.Contains((x, y)))
							{
								Console.Write("░");
							}
							else if (visited.ContainsKey((x, y)))
							{
								Console.Write("▒");
							}
							else
							{
								Console.Write(map[x, y]);
							}
						}
						Console.Write("\n");
					}
					Console.Write("\n");
					Console.Write("\n");
				}
			}

			// built
			Console.WriteLine("Built distances!");

			currentPos = endPos;
			steps = new List<(int, int)>();
			while (currentPos != startPos)
			{
				int left = 999999;
				int right = 999999;
				int up = 999999;
				int down = 999999;

				// evaluate directions
				if (visited.ContainsKey((currentPos.Item1 - 1, currentPos.Item2)))
				{
					left = visited[(currentPos.Item1 - 1, currentPos.Item2)];
				}
				if (visited.ContainsKey((currentPos.Item1 + 1, currentPos.Item2)))
				{
					right = visited[(currentPos.Item1 + 1, currentPos.Item2)];
				}
				if (visited.ContainsKey((currentPos.Item1, currentPos.Item2 - 1)))
				{
					up = visited[(currentPos.Item1, currentPos.Item2 - 1)];
				}
				if (visited.ContainsKey((currentPos.Item1, currentPos.Item2 + 1)))
				{
					down = visited[(currentPos.Item1, currentPos.Item2 + 1)];
				}

				int mostDirect = -1;
				int currentDist = visited[currentPos];
				Console.WriteLine("Steps Left: " + currentDist + " at " + currentPos.Item1 + ", " + currentPos.Item2);
				// left
				if (left == currentDist - 1)
				{
					mostDirect = 0;
				}
				//right
				if (right == currentDist - 1)
				{
					mostDirect = 1;
				}
				//up
				if (up == currentDist - 1)
				{
					mostDirect = 2;
				}
				//down
				if (down == currentDist - 1)
				{
					mostDirect = 3;
				}

				if (mostDirect == -1)
				{
					int lowest = 999999;
					int distance = 99999999;
					// left
					if (left < distance)
					{
						mostDirect = 0;
						distance = left;
					}

					//right
					if (right < distance)
					{
						mostDirect = 1;
						distance = right;
					}

					//up
					if (up < distance)
					{
						mostDirect = 2;
						distance = up;
					}

					//down
					if (down < distance)
					{
						mostDirect = 3;
						distance = down;
					}
				}

				switch (mostDirect)
				{
					case (0):
						//Console.WriteLine(steps.Count + ": Going Left to " + left);
						currentPos = (currentPos.Item1 - 1, currentPos.Item2);
						break;
					case (1):
						//Console.WriteLine(steps.Count + ": Going Right to " + right);
						currentPos = (currentPos.Item1 + 1, currentPos.Item2);
						break;
					case (2):
						//Console.WriteLine(steps.Count + ": Going Up + to " + up);
						currentPos = (currentPos.Item1, currentPos.Item2 - 1);
						break;
					case (3):
						//Console.WriteLine(steps.Count + ": Going Down to " + down);
						currentPos = (currentPos.Item1, currentPos.Item2 + 1);
						break;
					case (-1):
						Console.WriteLine("Hit a loop.");
						return;
				}

				steps.Add(currentPos);
			}


			for (int y = 0; y < map.GetLength(1); y++)
			{
				for (int x = 0; x < map.GetLength(0); x++)
				{
					if (steps.Contains((x, y)))
					{
						Console.Write(map[x, y]);
					}
					else
					{
						Console.Write(" ");
					}
				}
				Console.Write("\n");
			}

			Console.WriteLine("Final Steps: " + (steps.Count));
		}
		public static void Solve2()
		{
		}
	}
}
