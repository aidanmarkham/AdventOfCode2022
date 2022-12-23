using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class Day16
	{
		public static string inputFile = "valves.txt";
		public static int timeLimit = 30;

		public static int elephantTraining = 4;

		public static void Solve()
		{
			var input = File.ReadAllLines(inputFile);

			Valve startValve = null;

			List<Valve> valves = new List<Valve>();

			// create valves with names and flow rates
			for (int i = 0; i < input.Length; i++)
			{
				var words = input[i].Split(' ');

				var valve = new Valve();

				valve.name = words[1];

				valve.flowRate = int.Parse(words[4].Substring(5, words[4].Length - 6));

				valves.Add(valve);

				if (valve.name == "AA") startValve = valve;
			}

			// loop back again to create links now that they all exist 
			for (int i = 0; i < input.Length; i++)
			{
				var words = input[i].Split(' ');

				string linkedNames = String.Join(", ", words.Skip(9).ToArray());

				for (int v = 0; v < valves.Count; v++)
				{
					if (linkedNames.Contains(valves[v].name)) valves[i].linkedValves.Add(valves[v]);
				}
			}

			// list the valves and links to make sure we read them in correctly
			for (int v = 0; v < valves.Count; v++)
			{
				Console.Write("Valve " + valves[v].name + " Flow: " + valves[v].flowRate + " Links: ");
				for (int l = 0; l < valves[v].linkedValves.Count; l++)
				{
					Console.Write(valves[v].linkedValves[l].name + ", ");
				}
				Console.Write("\n");
			}

			// === pre build navigation map ===
			Dictionary<Valve, Dictionary<Valve, int>> distances = new Dictionary<Valve, Dictionary<Valve, int>>();
			for (int i = 0; i < valves.Count; i++)
			{
				ResetDistances(valves);
				valves[i].distanceFromStart = 0;
				CalculateDistancesFrom(valves[i]);

				var innerDict = new Dictionary<Valve, int>();

				for (int j = 0; j < valves.Count; j++)
				{
					innerDict.Add(valves[j], valves[j].distanceFromStart);
				}

				distances.Add(valves[i], innerDict);
			}

			// === pre build navigation map ===
			Dictionary<Valve, Dictionary<Valve, List<Valve>>> paths = new Dictionary<Valve, Dictionary<Valve, List<Valve>>>();
			for (int i = 0; i < valves.Count; i++)
			{
				ResetDistances(valves);
				valves[i].distanceFromStart = 0;
				CalculateDistancesFrom(valves[i]);

				var innerDict = new Dictionary<Valve, List<Valve>>();

				for (int j = 0; j < valves.Count; j++)
				{
					var path = new List<Valve>();
					var end = valves[j];
					var start = valves[i];
					var current = end;

					while (current.pathParent != start && start != end)
					{
						path.Add(current);
						current = current.pathParent;
					}
					if (start != end) path.Add(current);

					path.Reverse();

					innerDict.Add(valves[j], path);
				}

				paths.Add(valves[i], innerDict);
			}

			/*
			// test log out of paths
			for (int i = 0; i < valves.Count; i++)
			{
				for (int j = 0; j < valves.Count; j++)
				{
					var path = paths[valves[i]][valves[j]];
					Console.Write("Path from " + valves[i] + " to " + valves[j] + ": ");
					for (int p = 0; p < path.Count; p++)
					{
						Console.Write(path[p] + ",");
					}
					Console.Write("\n");
				}
			}
			*/

			// a list of closed valves minus any with 0 flow
			List<Valve> closedValves = new List<Valve>(valves);
			for (int i = closedValves.Count - 1; i >= 0; i--)
			{
				if (closedValves[i].flowRate == 0)
				{
					closedValves.RemoveAt(i);
				}
			}

			int bestFinisher = 0;
			string bestHistory = "";

			Recurse(startValve, distances, ref bestFinisher, ref bestHistory, "", "", 0, timeLimit, closedValves);

			Console.WriteLine("\n\n===== You =====");
			Console.WriteLine(bestHistory);
			Console.WriteLine("Best Finisher: " + bestFinisher);

			bestFinisher = 0;
			bestHistory = "";

			RecurseElephant(startValve, null, startValve, null, paths, ref bestFinisher, ref bestHistory, "", "", 0, timeLimit - elephantTraining, closedValves);

			Console.WriteLine("\n\n===== You and the Elephant =====");
			Console.WriteLine(bestHistory);
			Console.WriteLine("Best Finisher: " + bestFinisher);
		}

		public static void Recurse(Valve current, Dictionary<Valve, Dictionary<Valve, int>> distances, ref int bestFinisher, ref string bestHistory, string opened, string history, int pressure, int remainingTime, List<Valve> closedValves)
		{
			// keep track of history
			history += "\n=== Minute " + (timeLimit + 1 - remainingTime) + " ===\n";
			history += "At Node " + current.name + "\n";
			history += "Pressure " + pressure + "\n";

			// find closed valves still to open
			int left = closedValves.Count; ;
			for (int i = 0; i < closedValves.Count; i++)
			{
				if (opened.Contains(closedValves[i].name)) left--;
			}

			history += left + " valves remaining." + "\n";

			// if all the valves are open, just chill
			if (left <= 0)
			{
				// record us as the best finisher if we are
				if (pressure > bestFinisher)
				{
					Console.WriteLine("New Best: " + pressure);
					bestFinisher = pressure;
					bestHistory = history + "\nOut of valves";
				}
			}
			// travel to a valve and open it 
			else
			{
				// pick the valve
				for (int i = 0; i < closedValves.Count; i++)
				{
					// don't do anything if it's opened already
					if (opened.Contains(closedValves[i].name)) continue;

					// use the distance table to find out how far to travel
					var dist = distances[current][closedValves[i]];

					// if we won't have time to get there 
					if (dist > remainingTime)
					{
						// we're done so see if we've done better then the previous best 
						if (pressure > bestFinisher)
						{
							Console.WriteLine("New Best: " + pressure);
							bestFinisher = pressure;
							bestHistory = history + "\nOut of time while traveling " + dist + " units to " + closedValves[i];
						}
					}
					// if we can make it go ahead and recurse
					else
					{
						var newRemainingTime = remainingTime;

						// subtract the time it takes to walk
						newRemainingTime -= dist;

						// subtract the time it takes to open it 
						newRemainingTime -= 1;

						Recurse(closedValves[i], distances, ref bestFinisher, ref bestHistory, opened + " " + closedValves[i].name, history, pressure + (closedValves[i].flowRate * newRemainingTime), newRemainingTime, closedValves);
					}
				}
			}
		}


		public static void RecurseElephant(Valve you, Valve youTarget, Valve elephant, Valve eleTarget, Dictionary<Valve, Dictionary<Valve, List<Valve>>> paths, ref int bestFinisher, ref string bestHistory, string opened, string history, int pressure, int remainingTime, List<Valve> closedValves)
		{
			// keep track of history
			history += "\n=== Minute " + ((timeLimit - elephantTraining) + 1 - remainingTime) + " ===\n";
			history += "You're " + (you != null ? "at node " + you.name : "vibing") + (youTarget != null ? " heading to " + youTarget.name : "") + "\n";
			history += "Elephant is " + (elephant != null ? "at node " + elephant.name : "vibing") + (eleTarget != null ? " heading to " + eleTarget.name : "") + "\n";
			history += "Pressure " + pressure + "\n";

			// find closed valves still to open
			int left = closedValves.Count; ;
			for (int i = 0; i < closedValves.Count; i++)
			{
				if (opened.Contains(closedValves[i].name)) left--;
			}

			history += left + " valves remaining." + "\n";

			if(remainingTime == 0)
			{
				// record us as the best finisher if we are
				if (pressure > bestFinisher)
				{
					Console.WriteLine("New Best: " + pressure);
					bestFinisher = pressure;
					bestHistory = history + "\nOut of valves";
				}
			}
			// if all the valves are open, just chill
			else if (left <= 0)
			{
				// record us as the best finisher if we are
				if (pressure > bestFinisher)
				{
					Console.WriteLine("New Best: " + pressure);
					bestFinisher = pressure;
					bestHistory = history + "\nOut of valves";
				}
			}
			// travel to a valve and open it 
			else if (youTarget == null || eleTarget == null)
			{
				// ========= Figure out where each of you WANT to go ==========
				List<(Valve, Valve)> destinationPossibilites = new List<(Valve, Valve)>();

				if (youTarget == null && eleTarget == null)
				{
					for (int i = 0; i < closedValves.Count; i++)
					{
						// if i is open we should skip it 
						if (opened.Contains(closedValves[i].name)) continue;

						for (int j = 0; j < closedValves.Count; j++)
						{
							// you both can't have the same target
							if (i == j)
							{
								if (left == 1)
								{
									(Valve, Valve) possibility = (closedValves[i], null);
									if (!destinationPossibilites.Contains(possibility) && !destinationPossibilites.Contains((possibility.Item2, possibility.Item1)))
									{
										//destinationPossibilites.Add(possibility);
									}
									destinationPossibilites.Add(possibility);
								}
								continue;
							}
							else
							{
								// if j is open we should skip it 
								if (opened.Contains(closedValves[j].name)) continue;

								(Valve, Valve) possibility = (closedValves[i], closedValves[j]);
								if (!destinationPossibilites.Contains(possibility) && !destinationPossibilites.Contains((possibility.Item2, possibility.Item1)))
								{
									//destinationPossibilites.Add(possibility);
								}
								destinationPossibilites.Add(possibility);
							}
						}
					}
				}
				// the elephant already knows it's target
				else if (youTarget == null)
				{
					for (int i = 0; i < closedValves.Count; i++)
					{
						// don't do anything if it's opened already
						if (opened.Contains(closedValves[i].name)) continue;

						// the elephant is already headed to this one
						if (closedValves[i] == eleTarget) continue;

						(Valve, Valve) possibility = (closedValves[i], eleTarget);
						if (!destinationPossibilites.Contains(possibility) && !destinationPossibilites.Contains((possibility.Item2, possibility.Item1)))
						{
							//destinationPossibilites.Add(possibility);
						}
						destinationPossibilites.Add(possibility);
					}
				}
				// you already know your target
				else if (eleTarget == null)
				{
					for (int i = 0; i < closedValves.Count; i++)
					{
						// don't do anything if it's opened already
						if (opened.Contains(closedValves[i].name)) continue;

						// the you is already headed to this one
						if (closedValves[i] == youTarget) continue;

						(Valve, Valve) possibility = (youTarget, closedValves[i]);
						if (!destinationPossibilites.Contains(possibility) && !destinationPossibilites.Contains((possibility.Item2, possibility.Item1)))
						{
							
						}
						destinationPossibilites.Add(possibility);
					}
				}

				if (destinationPossibilites.Count == 0)
				{
					destinationPossibilites.Add((youTarget, eleTarget));
				}

				for (int d = 0; d < destinationPossibilites.Count; d++)
				{
					youTarget = destinationPossibilites[d].Item1;
					eleTarget = destinationPossibilites[d].Item2;

					// ========== Figure out where you actually WILL go ==============

					bool youWillOpen = false;
					bool eleWillOpen = false;

					int yourTime = remainingTime;
					int elephantsTime = remainingTime;

					// use the distance table to find out how far you each would need to travel and open your destination valve
					// if you don't need to travel (because the other one is on their way to the last valve, for example
					// keep your time at remainingTime
					if (you != null && youTarget != null)
					{
						if (youTarget != you)
						{
							yourTime = paths[you][youTarget].Count + 1;
						}
						else
						{
							yourTime = 1;
						}
					}
					if (elephant != null && eleTarget != null)
					{
						if (eleTarget != elephant)
						{
							elephantsTime = paths[elephant][eleTarget].Count + 1;
						}
						else
						{
							elephantsTime = 1;
						}
					}

					// find out how long this recursion will last
					// it's the smaller of the two
					var recurseTime = Math.Min(yourTime, elephantsTime);

					Valve youDest = youTarget;
					Valve eleDest = eleTarget;

					if (youTarget != null)
					{
						// you aren't going to make it in the next recurse
						if (recurseTime < yourTime)
						{
							youDest = paths[you][youTarget][recurseTime - 1];
						}
						// you will (unless you aren't going anywhere)
						else
						{
							youWillOpen = true;
						}
					}

					if (eleTarget != null)
					{
						// the elephant won't make it there in the next recurse
						if (recurseTime < elephantsTime)
						{
							eleDest = paths[elephant][eleTarget][recurseTime - 1];
						}
						// it will
						else
						{
							eleWillOpen = true;
						}
					}

					// neither of us are making it to another node
					if (recurseTime > remainingTime)
					{
						// we're done so see if we've done better then the previous best 
						if (pressure > bestFinisher)
						{
							Console.WriteLine("New Best: " + pressure);
							bestFinisher = pressure;
							bestHistory = history + "\nOut of time while traveling";
						}
					}
					// one of us will make it 
					else
					{
						var newRemainingTime = remainingTime;

						// subtract the time it takes for you and the elephant to do your thing
						newRemainingTime -= recurseTime;

						string newlyOpened = "";
						int addedPressure = 0;
						if (youWillOpen)
						{
							newlyOpened += " " + youTarget;
							addedPressure += youTarget.flowRate * newRemainingTime;
						}
						if (eleWillOpen)
						{
							newlyOpened += " " + eleTarget;
							addedPressure += eleTarget.flowRate * newRemainingTime;
						}

						RecurseElephant(youDest, youWillOpen ? null : youTarget, eleDest, eleWillOpen ? null : eleTarget, paths, ref bestFinisher, ref bestHistory, opened + newlyOpened, history, pressure + addedPressure, newRemainingTime, closedValves);
					}
				}
			}
		}

		// reset distances so we can calculate properly
		public static void ResetDistances(List<Valve> valves)
		{
			for (int i = 0; i < valves.Count; i++)
			{
				valves[i].distanceFromStart = int.MaxValue;
			}
		}

		// recurse through and find distances from the current node
		public static void CalculateDistancesFrom(Valve current)
		{
			for (int i = 0; i < current.linkedValves.Count; i++)
			{
				if (current.linkedValves[i].distanceFromStart > current.distanceFromStart + 1)
				{
					current.linkedValves[i].pathParent = current;
					current.linkedValves[i].distanceFromStart = current.distanceFromStart + 1;
					CalculateDistancesFrom(current.linkedValves[i]);
				}
			}
		}

		public class Valve
		{
			public string name = "";
			public int flowRate = 0;
			public List<Valve> linkedValves = new List<Valve>();
			public int distanceFromStart = int.MaxValue;
			public Valve pathParent;

			public override string ToString()
			{
				return name;
			}
		}
	}
}


