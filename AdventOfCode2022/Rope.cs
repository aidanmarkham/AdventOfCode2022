using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class Rope
	{
		public static string inputFile = "rope.txt";

		public static void CalculateRope()
		{
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);

			Console.WriteLine("Input file loaded! Calculating...");

			float distance = 0;

			List<(int, int)> snake = new List<(int, int)>();

			for(int i = 0; i < 10; i++)
			{
				snake.Add((0, 0));
			}

			List<(int, int)> locations = new List<(int, int)>();

			for (int i = 0; i < input.Length; i++)
			{
				char dir = input[i][0];

				int times = int.Parse(input[i].Split(' ')[1] + "");
				for (int t = 0; t < times; t++)
				{
					var head = snake[0];
					// move head
					switch (dir)
					{
						case 'U':
							head.Item2++;
							break;
						case 'D':
							head.Item2--;
							break;
						case 'L':
							head.Item1--;
							break;
						case 'R':
							head.Item1++;
							break;
					}
					snake[0] = head;

					for (int s = 1; s < snake.Count; s++)
					{
						var T = snake[s];
						var H = snake[s-1];
						// calculate the distance
						distance = (float)Math.Sqrt(Math.Pow(Math.Abs(H.Item1 - T.Item1), 2) + Math.Pow(Math.Abs(H.Item2 - T.Item2), 2));

						if (distance >= Math.Sqrt(2))
						{
							var xDist = H.Item1 - T.Item1;
							var yDist = H.Item2 - T.Item2;

							if (Math.Abs(xDist) > 0 && Math.Abs(yDist) > 0)
							{
								T.Item1 += 1 * Math.Sign(xDist);
								T.Item2 += 1 * Math.Sign(yDist);
							}
							else if (Math.Abs(xDist) > 0)
							{
								T.Item1 += 1 * Math.Sign(xDist);
							}
							else if (Math.Abs(yDist) > 0)
							{
								T.Item2 += 1 * Math.Sign(yDist);
							}
							else
							{
								Console.WriteLine("Huh?");
							}
						}

						snake[s] = T;
						snake[s - 1] = H;
					}
					locations.Add((snake[9].Item1, snake[9].Item2));
				}
			}

			Console.WriteLine("Locations: " + locations.Distinct().Count());
		}
	}

}