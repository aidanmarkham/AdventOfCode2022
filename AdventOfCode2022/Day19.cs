using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class Day19
	{
		public static string inputFile = "robots_test.txt";

		public static int totalTime = 24;
		public static void Solve()
		{
			// read in input 
			var input = File.ReadAllLines(inputFile);
			List<Blueprint> blueprints = new List<Blueprint>();
			for (int i = 0; i < input.Length; i++)
			{
				var words = input[i].Split(' ');

				var bp = new Blueprint();

				bp.oreBotCost = int.Parse(words[6]);
				bp.clayBotCost = int.Parse(words[12]);
				bp.obsBotOreCost = int.Parse(words[18]);
				bp.obsBotClayCost = int.Parse(words[21]);
				bp.geoBotOreCost = int.Parse(words[27]);
				bp.geoBotObsCost = int.Parse(words[30]);

				blueprints.Add(bp);
			}

			// total time for challenge
			int totalQuality = 0;



			for (int i = 0; i < 3; i++)
			{
				// set up initial state 
				State initialState = new State();
				initialState.timeRemaining = totalTime;
				initialState.oreBots = 1;

				// stack of states
				Stack<State> states = new Stack<State>();
				// add initial state
				states.Push(initialState);

				// grab the first blueprint for now
				var blueprint = blueprints[i];

				// used to save the best
				int mostGeo = 0;
				State best = new State();

				long totalIterations = 0;

				var maxOrePrice = Math.Max(Math.Max(blueprint.clayBotCost, blueprint.oreBotCost), Math.Max(blueprint.geoBotOreCost, blueprint.obsBotOreCost));
				var maxClayPrice = blueprint.obsBotClayCost;
				var maxObsPrice = blueprint.geoBotObsCost;

				while (states.Count > 0)
				{
					// the state to process
					var state = states.Pop();

					// keep track of iterations
					totalIterations++;
					if (totalIterations % 10000000 == 0)
					{
						Console.WriteLine("Iterations: " + totalIterations);
					}

					// if this is a new best for geodes
					if (state.geo > mostGeo)
					{
						mostGeo = state.geo;
						best = state;
						Console.WriteLine("Most Geo: " + mostGeo);
					}

					// if we don't buy anything
					State nextState = new State(state);
					nextState.history += "\n Waited.";
					if (ShouldIterate(nextState, mostGeo, blueprint))
					{
						states.Push(nextState);
					}

					// calculate maximum possible needed ores
					var maxNeededOre = Math.Max(Math.Max(blueprint.clayBotCost, blueprint.oreBotCost), Math.Max(blueprint.geoBotOreCost, blueprint.obsBotOreCost)) * state.timeRemaining;
					var maxNeededClay = blueprint.obsBotClayCost * state.timeRemaining;
					var maxNeededObs = blueprint.geoBotObsCost * state.timeRemaining;



					// if we can afford clay bot 
					if (state.clayBots < maxClayPrice && state.clay < maxNeededClay && state.ore >= blueprint.clayBotCost)
					{
						nextState = new State(state);
						// add a clay bot 
						nextState.clayBots++;

						// pay for it in ore
						nextState.ore -= blueprint.clayBotCost;
						nextState.history += "\n clay bot.";
						if (ShouldIterate(nextState, mostGeo, blueprint))
						{
							states.Push(nextState);
						}
					}

					// if we can afford ore bot
					if (state.oreBots < maxOrePrice && state.ore < maxNeededOre && state.ore >= blueprint.oreBotCost)
					{
						nextState = new State(state);
						// add an ore bot 
						nextState.oreBots++;

						// lower the ore count 
						nextState.ore -= blueprint.oreBotCost;
						nextState.history += "\n ore bot.";
						if (ShouldIterate(nextState, mostGeo, blueprint))
						{
							states.Push(nextState);
						}
					}

					// if we can afford obs bot
					if (state.obsBots < maxObsPrice && state.obs < maxNeededObs && state.ore >= blueprint.obsBotOreCost && state.clay >= blueprint.obsBotClayCost)
					{
						nextState = new State(state);
						// add an obs bot 
						nextState.obsBots++;

						// pay for it in ore and clay
						nextState.ore -= blueprint.obsBotOreCost;
						nextState.clay -= blueprint.obsBotClayCost;
						nextState.history += "\n obs bot.";
						if (ShouldIterate(nextState, mostGeo, blueprint))
						{
							states.Push(nextState);
						}
					}

					// if we can afford geo bot
					if (state.ore >= blueprint.geoBotOreCost && state.obs >= blueprint.geoBotObsCost)
					{
						nextState = new State(state);
						// add a geo bot 
						nextState.geoBots++;

						// pay for it in ore and obsidian
						nextState.ore -= blueprint.geoBotOreCost;
						nextState.obs -= blueprint.geoBotObsCost;
						nextState.history += "\n geo bot.";
						if (ShouldIterate(nextState, mostGeo, blueprint))
						{
							states.Push(nextState);
						}
					}
				}

				var thisQuality = best.geo;
				Console.WriteLine("Blueprint " + (i+1) + " quality: " + thisQuality + best.history);
				if(totalQuality == 0)
				{
					totalQuality += thisQuality;
				}
				else
				{
					totalQuality *= thisQuality;
				}
			}

			Console.WriteLine("Total Quality: " + totalQuality);
		}

		public static bool ShouldIterate(State state, int most, Blueprint bp)
		{
			var t = state.timeRemaining;
			bool time = t >= 0;

			// what we currently have
			var possibleGeo = state.geo;

			// what we could make with the bots we have
			possibleGeo += state.geoBots * t;

			// what we could possibly build if we had a new bot each round
			possibleGeo += (t * (t - 1)) / 2;

			bool botSprint = possibleGeo > most;
			return time && botSprint;
		}

		public struct State
		{
			public int timeRemaining;
			public int oreBots;
			public int clayBots;
			public int obsBots;
			public int geoBots;
			public int ore;
			public int clay;
			public int obs;
			public int geo;

			public string history;

			public State(State state) : this()
			{
				timeRemaining = state.timeRemaining - 1;

				oreBots = state.oreBots;
				clayBots = state.clayBots;
				obsBots = state.obsBots;
				geoBots = state.geoBots;

				ore = state.ore + state.oreBots;
				clay = state.clay + state.clayBots;
				obs = state.obs + state.obsBots;
				geo = state.geo + state.geoBots;
				history = state.history;
			}
		}

		public struct Blueprint
		{
			public int oreBotCost;
			public int clayBotCost;
			public int obsBotOreCost;
			public int obsBotClayCost;
			public int geoBotOreCost;
			public int geoBotObsCost;

			public override string ToString()
			{
				return " " + oreBotCost + clayBotCost + obsBotOreCost + obsBotClayCost + geoBotOreCost + geoBotObsCost;
			}
		}
	}
}
