using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class Rucksacks
	{
		public static string inputFile = "rucksacks.txt";
		public static void GetPriorities()
		{
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);

			Console.WriteLine("Input file loaded! Calculating...");

			int total = 0;

			for(int i = 0; i < input.Length; i++)
			{
				var rucksack = input[i];
				var pocketOne = rucksack.Substring(0, rucksack.Length / 2);
				var pocketTwo = rucksack.Substring(rucksack.Length / 2, rucksack.Length / 2);

				var common = pocketOne.Intersect(pocketTwo).ToList();

				int ascii = (int)common[0];

				if(ascii > 96 && ascii < 123)
				{
					ascii -= 96;
				}
				else if(ascii > 64 && ascii < 91)
				{
					ascii -= 38;
				}

				//Console.WriteLine("Item: " + common[0] + " Score: " + ascii);

				total += ascii;
			}

			Console.Write("Total Priority: " + total);
		}

		public static void GetBadgePriorities()
		{
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);

			Console.WriteLine("Input file loaded! Calculating...");

			int total = 0;

			for (int i = 0; i < input.Length; i += 3)
			{
				var rucksackA = input[i];
				var rucksackB = input[i+1];
				var rucksackC = input[i+2];

				var common = rucksackA.Intersect(rucksackB).Intersect(rucksackC).ToList();

				int ascii = (int)common[0];

				if (ascii > 96 && ascii < 123)
				{
					ascii -= 96;
				}
				else if (ascii > 64 && ascii < 91)
				{
					ascii -= 38;
				}

				//Console.WriteLine("Item: " + common[0] + " Score: " + ascii);

				total += ascii;
			}

			Console.Write("Total Priority: " + total);
		}
	}
}
