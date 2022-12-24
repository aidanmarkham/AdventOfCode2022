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

		public static void Solve()
		{
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

			int totalTime = 24;
			State initialState = new State();
			initialState.timeRemaining = totalTime;
			initialState.oreBots = 1;

			Stack<State> states = new Stack<State>();
			states.Push(initialState);
			var blueprint = blueprints[0];
			int mostGeo = 0;
			State best = new State();

			while (states.Count > 0)
			{
				// the state to process
				var state = states.Pop();

				// if this is a new best for geodes
				if (state.geo > mostGeo)
				{
					mostGeo = state.geo;
					best = state;
					Console.WriteLine("Most Geo: " + mostGeo);
				}

				if((state.geoBots + state.timeRemaining) * state.timeRemaining < mostGeo)
				{
					continue;
				}

				// if we don't buy anything
				State nextState = new State(state);
				nextState.history += nextState;
				nextState.history += "\nWaited.";
				if (nextState.timeRemaining >= 0)
				{
					states.Push(nextState);
				}

				// can afford ore bot
				if (state.ore >= blueprint.oreBotCost)
				{
					nextState = new State(state);
					// add an ore bot 
					nextState.oreBots++;

					// lower the ore count 
					nextState.ore -= blueprint.oreBotCost;


					nextState.history += nextState;
					nextState.history += "\nBought ore bot.";

					if (nextState.timeRemaining >= 0)
					{
						states.Push(nextState);
					}
				}

				// can afford clay bot 
				if (state.ore >= blueprint.clayBotCost)
				{
					nextState = new State(state);

					// add a clay bot 
					nextState.clayBots++;

					// pay for it in ore
					nextState.ore -= blueprint.clayBotCost;

					nextState.history += nextState;
					nextState.history += "\nBought clay bot.";
					if (nextState.timeRemaining >= 0)
					{
						states.Push(nextState);
					}
				}

				// can afford obs bot
				if (state.ore >= blueprint.obsBotOreCost && state.clay > blueprint.obsBotClayCost)
				{
					nextState = new State(state);

					// add an obs bot 
					nextState.obsBots++;

					// pay for it in ore and clay
					nextState.ore -= blueprint.obsBotOreCost;
					nextState.clay -= blueprint.obsBotClayCost;

					nextState.history += nextState;
					nextState.history += "\nBought obs bot.";

					if (nextState.timeRemaining >= 0)
					{
						states.Push(nextState);
					}
				}

				// can afford geo bot
				if (state.ore >= blueprint.geoBotOreCost && state.obs > blueprint.geoBotObsCost)
				{
					nextState = new State(state);

					// add a geo bot 
					nextState.geoBots++;

					// pay for it in ore and obsidian
					nextState.ore -= blueprint.geoBotOreCost;
					nextState.obs -= blueprint.geoBotObsCost;

					nextState.history += nextState;
					nextState.history += "\nBought geo bot.";
					if (nextState.timeRemaining >= 0)
					{
						states.Push(nextState);
					}
				}
			}

			Console.WriteLine(best.history);
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

			public override string ToString()
			{
				return "";
				string history = "";
				history += "\n=== Minute " + (24 - timeRemaining) + " ===";
				history += "\n Ore: " + ore;
				history += "\n Clay: " + clay;
				history += "\n Obsidian: " + obs;
				history += "\n Geodes: " + geo;
				return history;
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
