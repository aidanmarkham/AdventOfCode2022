using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	static class CalorieCounter
	{
		public static string inputFile = "calories.txt";

		public static void CountCalories()
		{
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);

			Console.WriteLine("Input file loaded! Calculating...");

			List<int> elves = new List<int>();

			elves.Add(0);

			for(int i = 0; i < input.Length; i++)
			{
				// we're at an empty line, so we've finished an elf
				if (input[i] == "")
				{
					elves.Add(0);
				}
				else
				{
					int lineValue = int.Parse(input[i]);

					// add it to the last list element
					elves[elves.Count - 1] += lineValue;
				}
			}

			elves.Sort();

			int lastThree = elves[elves.Count - 1] + elves[elves.Count - 2] + elves[elves.Count - 3];

			Console.WriteLine("All files parsed correctly!");

			Console.WriteLine("Max Calories: " + lastThree);
		}

	}

}
