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

			// a list of closed valves minus any with 0 flow
			List<Valve> closedValves = new List<Valve>(valves);
			for (int i = closedValves.Count - 1; i >= 0; i--)
			{
				if (closedValves[i].flowRate == 0)
				{
					closedValves.RemoveAt(i);
				}
			}

			// list out all differences between closed valves to ensure that's correct
			for (int i = 0; i < closedValves.Count; i++)
			{
				for (int j = 0; j < closedValves.Count; j++)
				{
					Console.WriteLine("Distance between " + closedValves[i].name + " and " + closedValves[j].name + " " + distances[closedValves[i]][closedValves[j]]);
				}
			}

			List<int> finishers = new List<int>();
			int bestFinisher = 0;
			string bestHistory = "";

			Recurse(startValve, distances, ref bestFinisher, ref bestHistory, "","", 0, timeLimit, closedValves, finishers);

			finishers.OrderBy(o => o);

			Console.WriteLine("Best Finisher from list: " + finishers[finishers.Count - 1]);

			Console.WriteLine(bestHistory);
			Console.WriteLine("Best Finisher: " + bestFinisher);
		}

		public static void Recurse(Valve current, Dictionary<Valve, Dictionary<Valve, int>> distances, ref int bestFinisher, ref string bestHistory, string opened, string history, int pressure, int remainingTime, List<Valve> closedValves, List<int> finishers)
		{
			// keep track of history
			history += "\n=== Minute " + (timeLimit  + 1 - remainingTime) + " ===\n";
			history += "At Node " + current.name + "\n";
			history += "Pressure " + pressure + "\n";

			/*
			// we're out of time! 
			if (remainingTime == 0)
			{
				if (pressure > bestFinisher)
				{
					finishers.Add(pressure);
					bestFinisher = pressure;
					bestHistory = history + "\nOut of time ";
				}
				return;
			}
			*/

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
					finishers.Add(pressure);
					bestFinisher = pressure;
					bestHistory = history + "\nOut of valves";
				}
				return;
			}
			// there's valves still open, we're at a valve that would release pressure, and we havn't opened it before
			else if (closedValves.Contains(current) && !opened.Contains(current.name))
			{
				// add ourself to the list of opened valves
				opened += " " + current.name;

				// add all the pressure this will release for the remaining time
				pressure += current.flowRate * (remainingTime-1);

				// record it for the final log out 
				history += "Staying here to open valve\n";

				// recurse again from here 
				Recurse(current, distances, ref bestFinisher, ref bestHistory, opened, history, pressure, remainingTime - 1, closedValves, finishers);
			}
			// there's valves still open, but we're not at a valve we can or should open
			else
			{
				// go through the valves worth opening
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
							finishers.Add(pressure);
							bestFinisher = pressure;
							bestHistory = history + "\nOut of time while traveling " + dist + " units to " + closedValves[i];
						}
						return;
					}
					// if we can make it go ahead and recurse
					else
					{
						Recurse(closedValves[i], distances, ref bestFinisher, ref bestHistory, opened, history, pressure, remainingTime - dist, closedValves, finishers);
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


