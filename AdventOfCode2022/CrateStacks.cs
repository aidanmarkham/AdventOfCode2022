using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class CrateStacks
	{
		public static string inputFile = "crates.txt";

		
		public static void GetStackList()
		{

			int initialheight = 9;
			int take = 8;
			int skip = 10;
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);

			Console.WriteLine("Input file loaded! Calculating...");

			var stacksRegion = input.Take(take).ToList();
			var instructions = input.Skip(skip).ToList();

			// get stacks out of stacks 
			List<Stack<char>> stacks = new List<Stack<char>>();
			for (int i = 0; i < initialheight; i++)
			{
				var stack = new Stack<char>(0);

				int horizontalIndex = 1 + i * 4;
				for(int s = stacksRegion.Count - 1; s >= 0; s--)
				{

					if(stacksRegion[s][horizontalIndex] != ' ')
					{
						stack.Push(stacksRegion[s][horizontalIndex]);
					}
				}

				stacks.Add(stack);
			}

			for(int i = 0; i < instructions.Count; i++)
			{
				var words = instructions[i].Split(' ');
				int quantity = int.Parse(words[1]);
				int source = int.Parse(words[3]) - 1;
				int dest = int.Parse(words[5]) - 1;

				for(int q = 0; q < quantity; q++)
				{
					if (stacks[source].Count == 0) continue;
					stacks[dest].Push(stacks[source].Pop());
				}
			}


			string stackList = "";
			for(int i = 0; i < stacks.Count; i++)
			{
				stackList += stacks[i].Pop();
			}

			Console.WriteLine(stackList);
		}

		public static void GetStackomaticList()
		{

			int initialheight = 9;
			int take = 8;
			int skip = 10;
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);

			Console.WriteLine("Input file loaded! Calculating...");

			var stacksRegion = input.Take(take).ToList();
			var instructions = input.Skip(skip).ToList();

			// get stacks out of stacks 
			List<Stack<char>> stacks = new List<Stack<char>>();
			for (int i = 0; i < initialheight; i++)
			{
				var stack = new Stack<char>(0);

				int horizontalIndex = 1 + i * 4;
				for (int s = stacksRegion.Count - 1; s >= 0; s--)
				{

					if (stacksRegion[s][horizontalIndex] != ' ')
					{
						stack.Push(stacksRegion[s][horizontalIndex]);
					}
				}

				stacks.Add(stack);
			}

			for (int i = 0; i < instructions.Count; i++)
			{
				var words = instructions[i].Split(' ');
				int quantity = int.Parse(words[1]);
				int source = int.Parse(words[3]) - 1;
				int dest = int.Parse(words[5]) - 1;

				List<char> smallStack = new List<char>();
				for (int q = 0; q < quantity; q++)
				{
					if (stacks[source].Count == 0) continue;
					smallStack.Add(stacks[source].Pop());
				}

				for (int s = smallStack.Count - 1; s >= 0; s--)
				{
					stacks[dest].Push(smallStack[s]);
				}
			}


			string stackList = "";
			for (int i = 0; i < stacks.Count; i++)
			{
				stackList += stacks[i].Pop();
			}

			Console.WriteLine(stackList);
		}
	}
}
