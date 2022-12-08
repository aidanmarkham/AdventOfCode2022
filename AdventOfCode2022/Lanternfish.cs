using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class Lanternfish
	{
		public static string inputFile = "fish.txt";

		public static void CalculateFish()
		{
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);
			string[] letters = input[0].Split(',');

			List<int> fish = new List<int>();

			for(int i = 0; i < letters.Length; i++)
			{
				fish.Add(int.Parse(letters[i]));
			}
			Console.WriteLine("Input file loaded! Calculating...");

			int days = 80;

			for(int i = 0; i < days; i++)
			{
				int newFish = 0;
				for(int j = 0; j < fish.Count; j++)
				{
					if(fish[j] == 0)
					{
						fish[j] = 6;
						newFish++;
					}
					else
					{
						fish[j]--;
					}
				}
				for(int j = 0; j < newFish; j++)
				{
					fish.Add(8);
				}

				Console.WriteLine("Days: " + i);
			}

			Console.WriteLine("Fish: " + fish.Count);
		}

		public static void CalculateFishBig()
		{
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);

			Console.WriteLine("Input file loaded! Calculating...");

			string[] letters = input[0].Split(',');

			double[] fish = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
			int days = 256;

			for (int i = 0; i < letters.Length; i++)
			{
				int num = int.Parse(letters[i]);
				fish[num]++;
			}

			Console.WriteLine("Initial State: ");
			for(int i = 0; i < fish.Length; i++)
			{
				Console.WriteLine(i + ": " + fish[i]);
			}


			for (int i = 0; i < days; i++)
			{
				double eight = fish[8];
				double seven = fish[7];
				double six = fish[6];
				double five = fish[5];
				double four = fish[4];
				double three = fish[3];
				double two = fish[2];
				double one = fish[1];
				double zero = fish[0];

				fish[8] = zero;
				fish[7] = eight;
				fish[6] = seven + zero;
				fish[5] = six;
				fish[4] = five;
				fish[3] = four;
				fish[2] = three;
				fish[1] = two;
				fish[0] = one;
			}

			Console.WriteLine("End State: ");
			for (int i = 0; i < fish.Length; i++)
			{
				Console.WriteLine(i + ": " + fish[i]);
			}

			double totalFish = 0;
			for (int i = 0; i < fish.Length; i++)
			{
				totalFish += fish[i];
			}

			Console.WriteLine("Total Fish: " + totalFish);
		}
	}
}
