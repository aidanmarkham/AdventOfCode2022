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
						case 3:
							Console.WriteLine("Day 3: Rucksacks. Running Now!");
							Rucksacks.GetPriorities();
							Rucksacks.GetBadgePriorities();
							break;

						case 4:
							Console.WriteLine("Day 4: Pairs. Running Now!");
							ContainedPairs.GetContainedPairs();
							ContainedPairs.GetOverlappedPairs();
							break;
						case 5:
							Console.WriteLine("Day 5: Stacks. Running Now!");
							CrateStacks.GetStackList();
							CrateStacks.GetStackomaticList();
							break;
						case 6:
							Console.WriteLine("Day 6: Data Stream. Running Now!");
							DataStream.FindMarker();
							break;
						case 7:
							Console.WriteLine("Day 7: File System. Running Now!");
							FileSystem.DiskCheck();
							FileSystem.SpaceFinder();
							break;
						case 8:
							Console.WriteLine("Day 8: Trees. Running Now!");
							TreeVisibility.CalculateVisibility();
							TreeVisibility.CalculateTreeVisibility();
							break;
						case 9:
							Console.WriteLine("Day 9: ---. Running Now!");
							Rope.CalculateRope();
							break;
						case 10:
							Console.WriteLine("Day 10: Signals. Running Now!");
							SignalStrengthCalculator.CalculateSignalStrength();
							SignalStrengthCalculator.RenderSignal();
							break;
						case 11:
							Console.WriteLine("Day 11: Running Now!");
							Console.WriteLine("Pt. 1");
							MonkeyInTheMiddle.Solve();
							Console.WriteLine("\nPt. 2");
							MonkeyInTheMiddle.Solve2();
							break;
						case 12:
							Console.WriteLine("Day 12: Running Now!");
							Day12.Solve();
							Day12.Solve2();
							break;
						case 13:
							Day13.Solve();
							break;
						case 14:
							Day14.Solve();
							break;
						case 15:
							Day15.Solve();
							Day15.Solve2();
							break;
						case 16:
							Day16.Solve();
							break;
						case 2021:
							Console.WriteLine("Year 2021: Running Now!");
							Advent2021.SonarSweep();
							Advent2021.SonarSweepAvg();
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
