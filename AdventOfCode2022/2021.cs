using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class Advent2021
	{
		public static void SonarSweep()
		{
			var input = File.ReadAllLines("sonar.txt");
			int times = 0;
			int depth = int.Parse(input[0]);
			for (int i = 0; i < input.Length; i++)
			{
				var newDepth = int.Parse(input[i]);
				if (newDepth > depth)
				{
					times++;
				}
				depth = newDepth;
			}
			Console.WriteLine(times);
		}

		public static void SonarSweepAvg()
		{
			var input = File.ReadAllLines("sonar.txt");
			int times = 0;
			int avgdepth = int.Parse(input[1]) + int.Parse(input[1- 1]) + int.Parse(input[1 + 1]);
			for (int i = 1; i < input.Length - 1; i++)
			{

				var newDepth = int.Parse(input[i]) + int.Parse(input[i-1]) + int.Parse(input[i+1]);
				if (newDepth > avgdepth)
				{
					times++;
				}
				avgdepth = newDepth;
			}
			Console.WriteLine(times);
		}
	}

}

