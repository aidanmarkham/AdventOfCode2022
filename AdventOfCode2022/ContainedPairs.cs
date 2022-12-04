using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class ContainedPairs
	{

		public static string inputFile = "pairs.txt";
		public static void GetContainedPairs()
		{
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);

			Console.WriteLine("Input file loaded! Calculating...");

			int contained = 0;
			for(int i = 0; i < input.Length; i++)
			{
				var line = input[i];
				var ranges = line.Split(',');

				var rangeA = ranges[0];
				var rangeB = ranges[1];

				var indicesA = rangeA.Split('-');
				var indicesB = rangeB.Split('-');

				int a1 = int.Parse(indicesA[0]);
				int a2 = int.Parse(indicesA[1]);

				int b1 = int.Parse(indicesB[0]);
				int b2 = int.Parse(indicesB[1]);

				if((a1 <= b1 && a2 >= b2) || (a1 >= b1 && a2 <= b2))
				{
					Console.WriteLine(a1 + "-" + a2 + ", " + b1 + "-" + b2);
					contained++;
				}
			}

			Console.WriteLine("contained: " + contained);
		}

		public static void GetOverlappedPairs()
		{
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);

			Console.WriteLine("Input file loaded! Calculating...");

			int contained = 0;
			for (int i = 0; i < input.Length; i++)
			{
				var line = input[i];
				var ranges = line.Split(',');

				var rangeA = ranges[0];
				var rangeB = ranges[1];

				var indicesA = rangeA.Split('-');
				var indicesB = rangeB.Split('-');

				int a1 = int.Parse(indicesA[0]);
				int a2 = int.Parse(indicesA[1]);

				int b1 = int.Parse(indicesB[0]);
				int b2 = int.Parse(indicesB[1]);

				if (a1 >= b2 && a1 <= b2 ||
					a2 >= b1 && a2 <= b2 ||
					b1 >= a1 && b1 <= a1 ||
					b2 >= a1 && b2 <= a2)
				{
					Console.WriteLine(a1 + "-" + a2 + ", " + b1 + "-" + b2);
					contained++;
				}
			}

			Console.WriteLine("contained: " + contained);
		}
	}
}
