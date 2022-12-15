using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class Day15
	{
		public static string inputFile = "beacons.txt";
		public static int searchSpace = 4000000;
		public static void Solve()
		{
			int line = 2000000;
			var input = File.ReadAllLines(inputFile);

			List<(int x, int y)> sensors = new List<(int x, int y)>();
			List<(int x, int y)> beacons = new List<(int x, int y)>();

			List<List<(int x, int y)>> procluded = new List<List<(int x, int y)>>();

			for (int i = 0; i < input.Length; i++)
			{
				var words = input[i].Split(' ');

				sensors.Add((
					int.Parse(words[2].Substring(2, words[2].Length - 3)),
					int.Parse(words[3].Substring(2, words[3].Length - 3))));
				beacons.Add((
					int.Parse(words[8].Substring(2, words[8].Length - 3)),
					int.Parse(words[9].Substring(2, words[9].Length - 2))));
				procluded.Add(GetProcludedOnLine(sensors[i], Dist(sensors[i], beacons[i]), line));
			}

			procluded = procluded.Distinct().ToList();

			List<(int x, int y)> noBeacons = new List<(int x, int y)>();
			foreach (List<(int x, int y)> list in procluded)
			{
				foreach ((int x, int y) point in list)
				{
					if (!beacons.Contains(point) && !sensors.Contains(point) && point.y == line)
					{
						noBeacons.Add(point);
					}
				}
			}

			Console.WriteLine(noBeacons.Distinct().ToList().Count + " positions can't contain a beacon.");
		}

		public static void Solve2()
		{
			var input = File.ReadAllLines(inputFile);

			List<(int x, int y)> sensors = new List<(int x, int y)>();
			List<(int x, int y)> beacons = new List<(int x, int y)>();
			List<int> distances = new List<int>();

			List<List<(int x, int y)>> procluded = new List<List<(int x, int y)>>();
			(int x, int y) min = (int.MaxValue, int.MaxValue);
			(int x, int y) max = (int.MinValue, int.MinValue);


			for (int i = 0; i < input.Length; i++)
			{
				var words = input[i].Split(' ');

				sensors.Add((
					int.Parse(words[2].Substring(2, words[2].Length - 3)),
					int.Parse(words[3].Substring(2, words[3].Length - 3))));
				beacons.Add((
					int.Parse(words[8].Substring(2, words[8].Length - 3)),
					int.Parse(words[9].Substring(2, words[9].Length - 2))));

				distances.Add(Dist(sensors[i], beacons[i]));

				if (sensors[i].x - distances[i] < min.x) min.x = sensors[i].x - distances[i];
				if (sensors[i].y - distances[i] < min.y) min.y = sensors[i].y - distances[i];
				if (sensors[i].x + distances[i] > max.x) max.x = sensors[i].x + distances[i];
				if (sensors[i].y + distances[i] > max.y) max.y = sensors[i].y + distances[i];
			}

			for (int i = 0; i < searchSpace; i++)
			{
				CheckLine(i, searchSpace, sensors, distances);
			}

		}
		private static int Dist((int x, int y) a, (int x, int y) b)
		{
			return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
		}

		private static List<(int x, int y)> GetProcludedOnLine((int x, int y) point, int distance, int line)
		{
			List<(int x, int y)> procluded = new List<(int x, int y)>();

			for (int x = 0; x < distance + 1; x++)
			{

				var a = point;
				var b = point;

				a.x += x;
				a.y = line;

				b.x -= x;
				b.y = line;

				if (Dist(point, a) <= distance) procluded.Add(a);
				if (Dist(point, b) <= distance) procluded.Add(b);
			}

			return procluded.Distinct().ToList();
		}

		private static void CheckLine(int line, int width, List<(int x, int y)> sensors, List<int> distances)
		{
			List<(int l, int r)> ranges = new List<(int l, int r)>();

			for (int i = 0; i < sensors.Count; i++)
			{
				// get the sensor's distance from a beacon
				var dist = distances[i];

				// subtract the vertical
				dist -= Math.Abs(sensors[i].y - line);

				if (dist > 0)
				{
					(int l, int r) range = (sensors[i].x - dist, sensors[i].x + dist);

					ranges.Add(range);
				}
			}


			int index = 0;

			while (index < width)
			{
				int usedRangeDex = 0;
				bool found = true;
				// find a range we're inside
				for (int s = 0; s < ranges.Count; s++)
				{
					if (ranges[s].l <= index + 1 && ranges[s].r >= index + 1)
					{
						usedRangeDex = s;
						index = ranges[s].r;
						found = false;
						break;
					}
				}
				if (ranges.Count > 0) ranges.RemoveAt(usedRangeDex);
				if (found)
				{
					var x = (index + 1);
					var y = line;
					Console.WriteLine("Unprecluded point found at " + (index + 1) + "," + line);
					Console.WriteLine("Freq: " + (((double)x * 4000000) + (double)y));
					index = width;
				}
			}
		}
	}
}
