using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	static class RockPaperScissors
	{
		public static string inputFile = "rockpaperscissors.txt";

		public static void RunStrategyGuide() {
			Console.WriteLine("Reading input file.");
			string[] input = File.ReadAllLines(inputFile);
			Console.WriteLine("Input file loaded! Calculating...");

			int score = 0;
			for (int i = 0; i < input.Length; i++)
			{
				var line = input[i];

				var opponentThrow = line[0];
				var yourThrow = line[2];

				//Console.WriteLine("Opponent: " + opponentThrow + " You: " + yourThrow);

				// calculate throw points
				switch (yourThrow)
				{
					case 'X':
						score += 1;
						yourThrow = 'A';
						break;
					case 'Y':
						score += 2;
						yourThrow = 'B';
						break;
					case 'Z':
						score += 3;
						yourThrow = 'C';
						break;
				}

				if (opponentThrow == yourThrow)
				{
					score += 3;
				}
				else if (opponentThrow == 'A')
				{
					switch (yourThrow)
					{
						case 'B':
							score += 6;
							break;
						case 'C':
							score += 0;
							break;
					}
				}
				else if (opponentThrow == 'B')
				{
					switch (yourThrow)
					{
						case 'A':
							score += 0;
							break;
						case 'C':
							score += 6;
							break;
					}
				}
				else if (opponentThrow == 'C')
				{
					switch (yourThrow)
					{
						case 'A':
							score += 6;
							break;
						case 'B':
							score += 0;
							break;
					}
				}

				//Console.WriteLine("Score:" + score);
			}

			Console.WriteLine("Final Score: " + score);
		}


		public static void RunStrategyGuideCalculate()
		{
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);

			Console.WriteLine("Input file loaded! Calculating...");

			int score = 0;
			for (int i = 0; i < input.Length; i++)
			{
				var line = input[i];

				var opponentThrow = line[0];
				var yourThrow = line[2];

				// calculate throw points
				switch (yourThrow)
				{
					// you need to lose
					case 'X':
						score += 0;
						break;
					case 'Y':
						score += 3;
						break;
					case 'Z':
						score += 6;
						break;
				}

				if (opponentThrow == 'A')
				{
					switch (yourThrow)
					{
						// you need to lose
						case 'X':
							score += 3;
							break;
						// you need to tie
						case 'Y':
							score += 1;
							break;
						// you need to win
						case 'Z':
							score += 2;
							break;
					}
				}
				else if (opponentThrow == 'B')
				{
					switch (yourThrow)
					{
						// you need to lose
						case 'X':
							score += 1;
							break;
						// you need to tie
						case 'Y':
							score += 2;
							break;
						// you need to win
						case 'Z':
							score += 3;
							break;
					}
				}
				else if (opponentThrow == 'C')
				{
					switch (yourThrow)
					{
						// you need to lose
						case 'X':
							score += 2;
							break;
						// you need to tie
						case 'Y':
							score += 3;
							break;
						// you need to win
						case 'Z':
							score += 1;
							break;
					}
				}
			}

			Console.WriteLine("Final Score: " + score);
		}
	}
}
