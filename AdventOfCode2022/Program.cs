using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("--- Welcome to Aidan's Advent of Code program. ---");
			bool quitting = false;

			Console.WriteLine("");

			while (!quitting)
			{
				Console.WriteLine("");
				Console.WriteLine("Please type the day you'd like to run (or 0 to quit):");
				string response = Console.ReadLine();
				Console.WriteLine("");


				if (int.TryParse(response, out int result))
				{
					switch (result)
					{
						case 0:
							quitting = true;
							break;
						case 1:
							// Day 1: Calorie Counter
							Console.WriteLine("Day 1: Calorie Counter. Running Now!");
							CalorieCounter.CountCalories();
							break;
						case 2:
							//Day 2: Rock Paper Scissors
							Console.WriteLine("Day 2: Rock Paper Scissors. Running Now!");
							RockPaperScissors.RunStrategyGuide();
							RockPaperScissors.RunStrategyGuideCalculate();
							break;
						default:
							Console.WriteLine("Sorry, I didn't get that. Try again?");
							break;
					}
				}
			}

			Console.WriteLine("");
			Console.WriteLine("All done? Goodbye! Press enter to quit.");
			Console.ReadLine();
		}
	}
}
