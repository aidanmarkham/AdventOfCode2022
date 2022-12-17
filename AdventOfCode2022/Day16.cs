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

			// loop back again to create lengths now that they all exist 
			for (int i = 0; i < input.Length; i++)
			{
				var words = input[i].Split(' ');

				string linkedNames = String.Join(", ", words.Skip(9).ToArray());

				for (int v = 0; v < valves.Count; v++)
				{
					if (linkedNames.Contains(valves[v].name)) valves[i].linkedValves.Add(valves[v]);
				}
			}

			// debug list the valves
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

			for (int i = 0; i < closedValves.Count; i++)
			{
				for (int j = 0; j < closedValves.Count; j++)
				{
					Console.WriteLine("Distance between " + closedValves[i].name + " and " + closedValves[j].name + " " + distances[closedValves[i]][closedValves[j]]);
				}
			}

			int bestFinisher = 0;
			string bestHistory = "";

			Recurse(startValve, distances, ref bestFinisher, ref bestHistory, "","", 0, 0, timeLimit, closedValves);
			Console.WriteLine(bestHistory);
			Console.WriteLine("Best Finisher: " + bestFinisher);
		}


		public static void Recurse(Valve current, Dictionary<Valve, Dictionary<Valve, int>> distances, ref int bestFinisher, ref string bestHistory, string visited, string history, int rateOfPressure, int pressure, int remainingTime, List<Valve> closedValves)
		{
			// keep track of history
			history += "\n=== Minute " + (timeLimit  + 1 - remainingTime) + " ===\n";
			history += "At Node " + current.name + "\n";
			history += "Rate " + rateOfPressure + "\n";
			history += "Pressure " + pressure + "\n";

			// outta time!
			if (remainingTime == 1)
			{
				if (pressure > bestFinisher)
				{					
					bestFinisher = pressure;
					bestHistory = history + "\nOut of time ";
				}
				return;
			}

			// find closed valves still to open
			int left = closedValves.Count; ;
			for (int i = 0; i < closedValves.Count; i++)
			{
				if (visited.Contains(closedValves[i].name)) left--;
			}

			history += left + " valves remaining." + "\n";

			// all valves open
			if (left <= 0)
			{
				pressure += rateOfPressure * (remainingTime - 1);
				if (pressure > bestFinisher)
				{
					bestFinisher = pressure;
					bestHistory = history + "\nOut of valves";
				}
				return;
			}
			// we're at one we can close
			else if (closedValves.Contains(current) && !visited.Contains(current.name))
			{
				visited += " " + current.name;
				rateOfPressure += current.flowRate;
				history += "Staying here to open valve\n";
				Recurse(current, distances, ref bestFinisher, ref bestHistory, visited, history, rateOfPressure, pressure + rateOfPressure, remainingTime - 1, closedValves);
			}
			else
			{
				// go through the valves worth opening still
				for (int i = 0; i < closedValves.Count; i++)
				{
					if (visited.Contains(closedValves[i].name)) continue;

					var dist = distances[current][closedValves[i]];
					if (dist >= remainingTime)
					{
						pressure += rateOfPressure * (remainingTime - 1);
						if (pressure > bestFinisher)
						{
							bestFinisher = pressure;
							bestHistory = history + "\nOut of time while traveling " + dist + " units to " + closedValves[i];
						}
						return;
					}
					else
					{
						Recurse(closedValves[i], distances, ref bestFinisher, ref bestHistory, visited, history, rateOfPressure, pressure + (rateOfPressure * dist), remainingTime - dist, closedValves);
					}
				}
			}
		}



		public static void RecurseValves(ValveNode current, Dictionary<Valve, Dictionary<Valve, int>> distances, ref ValveNode bestFinisher)
		{
			if (current.closedValves.Count == 0)
			{
				var child = new ValveNode(current.valve, current.timeRemaining, current.pressureReleased, current.pressureRate, current.closedValves, current.timeRemaining);
				if (child.pressureReleased > bestFinisher.pressureReleased)
				{
					child.parent = current;
					bestFinisher = child;
				}
			}
			else if (current.closedValves.Contains(current.valve))
			{

				// create the child
				var child = new ValveNode(current.valve, current.timeRemaining, current.pressureReleased, current.pressureRate + current.valve.flowRate, current.closedValves, 1);
				child.closedValves.Remove(current.valve);
				child.CalcPressure();
				if (child.timeRemaining > 0)
				{
					// capture references
					child.parent = current;
					current.children.Add(child);

					// recurse
					RecurseValves(child, distances, ref bestFinisher);
				}
				else if (child.timeRemaining == 0)
				{
					// capture references
					child.parent = current;
					current.children.Add(child);

					if (child.pressureReleased > bestFinisher.pressureReleased)
					{
						bestFinisher = child;
					}
				}

			}
			// go through the valves worth opening still
			for (int i = 0; i < current.closedValves.Count; i++)
			{
				if (current.closedValves[i] == current.valve) continue;

				var dist = distances[current.valve][current.closedValves[i]];
				var child = new ValveNode(current.closedValves[i], current.timeRemaining, current.pressureReleased, current.pressureRate, current.closedValves, dist);
				child.CalcPressure();
				if (child.timeRemaining > 0)
				{
					// capture references
					child.parent = current;
					current.children.Add(child);

					// recurse
					RecurseValves(child, distances, ref bestFinisher);
				}
				else if (child.timeRemaining == 0)
				{
					// capture references
					child.parent = current;
					current.children.Add(child);

					if (child.pressureReleased > bestFinisher.pressureReleased)
					{
						bestFinisher = child;
					}
				}
			}

		}

		public class ValveNode
		{
			public ValveNode(Valve v, int t, int p, int rate, List<Valve> c, int timePassed)
			{
				valve = v;
				// t, how much time was remaining at the last node
				// timePassed, how much time was remaining when we got to this node
				timeRemaining = t - timePassed;
				timePass = timePassed;
				pressureReleased = p;
				pressureRate = rate;
				closedValves = new List<Valve>(c);
			}
			public void CalcPressure()
			{
				pressureReleased += pressureRate * timePass;
			}
			public Valve valve;
			public int timeRemaining = 0;
			public int pressureReleased = 0;
			public int pressureRate = 0;
			public List<Valve> closedValves;
			public List<ValveNode> children = new List<ValveNode>();
			public ValveNode parent;
			private int timePass;
			public override string ToString()
			{
				return valve.ToString();
			}
		}


		public static void Solve_Old()
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

				if (i == 0) startValve = valve;
			}

			// loop back again to create lengths now that they all exist 
			for (int i = 0; i < input.Length; i++)
			{
				var words = input[i].Split(' ');

				string linkedNames = String.Join(", ", words.Skip(9).ToArray());

				for (int v = 0; v < valves.Count; v++)
				{
					if (linkedNames.Contains(valves[v].name)) valves[i].linkedValves.Add(valves[v]);
				}
			}


			for (int v = 0; v < valves.Count; v++)
			{
				Console.Write("Valve " + valves[v].name + " Links: ");
				for (int l = 0; l < valves[v].linkedValves.Count; l++)
				{
					Console.Write(valves[v].linkedValves[l].name + ", ");
				}
				Console.Write("\n");
			}

			List<Valve> closedValves = new List<Valve>(valves);

			for (int i = closedValves.Count - 1; i >= 0; i--)
			{
				if (closedValves[i].flowRate == 0)
				{
					closedValves.RemoveAt(i);
				}
			}

			var time = timeLimit;
			var current = startValve;
			int pressure = 0;
			int maxDepth = 5;
			while (time > 0)
			{
				Console.WriteLine("\n==Minute " + ((timeLimit + 1) - time) + "==");

				// ======= Calculate current Valve Values ==============
				ResetDistances(valves);
				current.distanceFromStart = 0;
				CalculateDistancesFrom(current);
				current.GetPotential(maxDepth, 0, closedValves, time);
				Valve bestValve = null;
				int bestPotential = int.MinValue;
				for (int v = 0; v < closedValves.Count; v++)
				{
					var thisPot = 0;

					thisPot = closedValves[v].potential;

					Console.WriteLine("   Valve " + closedValves[v].name + " has value " + thisPot);

					if (bestPotential < thisPot)
					{
						bestPotential = thisPot;
						bestValve = closedValves[v];
					}
				}



				// ========= calculate relief ============
				var openedValves = valves.Except(closedValves).ToList();
				var pressureThisRound = 0;
				if (openedValves.Count > 0)
				{
					Console.Write("Valves ");
					for (int i = 0; i < openedValves.Count; i++)
					{
						Console.Write(openedValves[i].name + " ");
						pressureThisRound += openedValves[i].flowRate;
					}
					Console.Write("are open, releasing " + pressureThisRound + " pressure. \n");
				}
				else
				{
					Console.WriteLine("No valves are open.");
				}
				pressure += pressureThisRound;



				// =========== figure out the best move ===============

				// if we should stay
				if (bestValve == current && closedValves.Contains(current) == true)
				{
					Console.WriteLine("I open valve " + current.name);
					closedValves.Remove(current);
				}
				// if we should move
				else if (bestValve != current)
				{
					var nextValve = bestValve;
					while (nextValve.distanceFromStart > 1)
					{
						nextValve = nextValve.pathParent;
					}

					Console.WriteLine("I move to valve " + nextValve + " because I'm heading to valve " + bestValve);
					current = nextValve;
				}
				// if there's no point in moving because all valves are open
				else if (closedValves.Count == 0)
				{
					Console.WriteLine("All Valves are open! Sitting Still!");
				}
				// if we're in an unhandled state
				else
				{
					Console.WriteLine("Huh?");
				}

				// decrement time
				time--;
			}
			Console.WriteLine("Total pressure released: " + pressure);

		}

		public static void ResetDistances(List<Valve> valves)
		{
			for (int i = 0; i < valves.Count; i++)
			{
				valves[i].distanceFromStart = int.MaxValue;
			}
		}
		// recurse through and find distances
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
			public int potential = 0;
			public int GetPotential(int maxDepth, int currentDepth, List<Valve> closedValves, int remainingTime)
			{
				// only add to the value if we consider this one closed
				// we subtract one from remaining time to simulate if we spent the time opening it
				var myPotential = !closedValves.Contains(this) ? 0 : flowRate * (remainingTime - 1 - distanceFromStart);

				var stayingPotential = myPotential;
				var goingPotential = 0;

				if (currentDepth < maxDepth)
				{
					// ======== If we were going to stay ================
					// remove ourself from closed valves as we're open now
					var closedAfterOpeningThis = new List<Valve>(closedValves);
					closedAfterOpeningThis.Remove(this);

					var stayingNextPotential = 0;
					for (int i = 0; i < linkedValves.Count; i++)
					{
						// current depth goes up by one because we go in by one level
						// remaining time goes down by two though, because we spent an extra minute here
						stayingNextPotential += linkedValves[i].GetPotential(maxDepth, currentDepth + 1, closedAfterOpeningThis, remainingTime - 1);
					}
					// add that to stayingPotential
					stayingPotential += stayingNextPotential;


					// ========== if we were going to go 
					// calculate the next steps if we don't open this one
					for (int i = 0; i < linkedValves.Count; i++)
					{
						// current depth goes up by one because we go in by one level
						// remaining time goes down by one because we didn't spend time here opening the valve
						goingPotential += linkedValves[i].GetPotential(maxDepth, currentDepth + 1, closedValves, remainingTime - 1);
					}
				}

				potential = Math.Max(stayingPotential, goingPotential);

				// now we have staying potential and going potential, we want the max
				return Math.Max(stayingPotential, goingPotential);
			}

			public override string ToString()
			{
				return name;
			}
		}
	}
}


