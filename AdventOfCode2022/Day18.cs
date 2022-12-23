using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class Day18
	{
		public static string inputFile = "lava.txt";

		public static void Solve()
		{
			var input = File.ReadAllLines(inputFile);

			List<(int x, int y, int z)> coords = new List<(int x, int y, int z)>();

			(int x, int y, int z) min = (int.MaxValue, int.MaxValue, int.MaxValue);
			(int x, int y, int z) max = (int.MinValue, int.MinValue, int.MinValue);

			// get all coords
			for (int i = 0; i < input.Length; i++)
			{
				var parts = input[i].Split(',');
				coords.Add((int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2])));

				if (coords[coords.Count - 1].x < min.x) min.x = coords[coords.Count - 1].x;
				if (coords[coords.Count - 1].y < min.y) min.y = coords[coords.Count - 1].y;
				if (coords[coords.Count - 1].z < min.z) min.z = coords[coords.Count - 1].z;

				if (coords[coords.Count - 1].x > max.x) max.x = coords[coords.Count - 1].x;
				if (coords[coords.Count - 1].y > max.y) max.y = coords[coords.Count - 1].y;
				if (coords[coords.Count - 1].z > max.z) max.z = coords[coords.Count - 1].z;
			}

			Console.WriteLine("Min: " + min + " Max: " + max);
			char[,,] map = new char[max.x - min.x + 2, max.y - min.y + 2, max.z - min.z + 2];

			for (int i = 0; i < coords.Count; i++)
			{
				// transform position to be within min and max
				var coord = coords[i];
				coord.x -= min.x;
				coord.y -= min.y;
				coord.z -= min.z;
				coords[i] = coord;

				// fill the map
				map[coord.x, coord.y, coord.z] = '#';
			}

			int surfaceArea = 0;

			Stack<(int x, int y, int z)> stack = new Stack<(int x, int y, int z)>();

			stack.Push((0, 0, 0));

			PrintMap(map);

			while(stack.Count > 0)
			{
				var pos = stack.Pop();

				var coord = pos;
				var up = coord;
				up.y += 1;

				var down = coord;
				down.y -= 1;

				var right = coord;
				right.x += 1;

				var left = coord;
				left.x -= 1;

				var forward = coord;
				forward.z += 1;

				var back = coord;
				back.z -= 1;

				if (IsWithin(up, map))
				{
					// make sure we're at air
					if (map[up.x, up.y, up.z] == (char)0)
					{
						map[up.x, up.y, up.z] = '-';
						stack.Push(up);
					}

					if (IsWithin(down, map))
					{
						if (map[down.x, down.y, down.z] == (char)0)
						{
							map[down.x, down.y, down.z] = '-';
							stack.Push(down);
						}
					}


					if (IsWithin(right, map))
					{
						if (map[right.x, right.y, right.z] == (char)0)
						{
							map[right.x, right.y, right.z] = '-';
							stack.Push(right);
						}
					}
				}

				if (IsWithin(left, map))
				{
					if (map[left.x, left.y, left.z] == (char)0)
					{
						map[left.x, left.y, left.z] = '-';
						stack.Push(left);
					}
				}


				if (IsWithin(forward, map))
				{
					if (map[forward.x, forward.y, forward.z] == (char)0)
					{
						map[forward.x, forward.y, forward.z] = '-';
						stack.Push(forward);
					}
				}


				if (IsWithin(back, map))
				{
					if (map[back.x, back.y, back.z] == (char)0)
					{
						map[back.x, back.y, back.z] = '-';
						stack.Push(back);
					}
				}
			}

			PrintMap(map);

			// go through and find surface
			for (int i = 0; i < coords.Count; i++)
			{
				var coord = coords[i];

				var up = coord;
				up.y += 1;

				var down = coord;
				down.y -= 1;

				var right = coord;
				right.x += 1;

				var left = coord;
				left.x -= 1;

				var forward = coord;
				forward.z += 1;

				var back = coord;
				back.z -= 1;

				if (IsWithin(up, map))
				{
					if (map[up.x, up.y, up.z] == '-') surfaceArea++;
				}
				else
				{
					surfaceArea++;
				}

				if (IsWithin(down, map))
				{
					if (map[down.x, down.y, down.z] == '-') surfaceArea++;
				}
				else
				{
					surfaceArea++;
				}

				if (IsWithin(right, map))
				{
					if (map[right.x, right.y, right.z] == '-') surfaceArea++;
				}
				else
				{
					surfaceArea++;
				}

				if (IsWithin(left, map))
				{
					if (map[left.x, left.y, left.z] == '-') surfaceArea++;
				}
				else
				{
					surfaceArea++;
				}

				if (IsWithin(forward, map))
				{
					if (map[forward.x, forward.y, forward.z] == '-') surfaceArea++;
				}
				else
				{
					surfaceArea++;
				}

				if (IsWithin(back, map))
				{
					if (map[back.x, back.y, back.z] == '-') surfaceArea++;
				}
				else
				{
					surfaceArea++;
				}

			}

			Console.WriteLine("Surface Area: " + surfaceArea);
		}

		static bool IsWithin((int x, int y, int z) a, char[,,] b)
		{
			return a.x >= 0 && a.x < b.GetLength(0) &&
				a.y >= 0 && a.y < b.GetLength(1) &&
				a.z >= 0 && a.z < b.GetLength(2);
		}

		static void PrintMap(char[,,] map)
		{
			for (int x = 0; x < map.GetLength(0); x++)
			{
				Console.WriteLine("\nSlice " + x);

				for (int y = 0; y < map.GetLength(1); y++)
				{
					for (int z = 0; z < map.GetLength(2); z++)
					{
						Console.Write(map[x,y,z]);
					}
					Console.WriteLine();
				}
			}
		}
	}
}
