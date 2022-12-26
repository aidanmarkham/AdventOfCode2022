using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class Day24
	{
		public static string inputFile = "blizzard.txt";

		public static void Solve()
		{
			#region Loading Input 
			// read in input file
			var input = File.ReadAllLines(inputFile);

			(int width, int height) map = (input[0].Length, input.Length);

			(int x, int y) start = (1, 0);
			(int x, int y) end = (map.width - 2, map.height - 1);

			List<Blizzard> blizzards = new List<Blizzard>();

			for (int y = 0; y < input.Length; y++)
			{
				for (int x = 0; x < input[y].Length; x++)
				{
					var mapChar = input[y][x];
					Blizzard bliz = new Blizzard(map);
					switch (mapChar)
					{
						case '>':
							bliz.direction = mapChar;
							bliz.startPosition = (x, y);
							blizzards.Add(bliz);
							break;
						case '<':
							bliz.direction = mapChar;
							bliz.startPosition = (x, y);
							blizzards.Add(bliz);
							break;
						case '^':
							bliz.direction = mapChar;
							bliz.startPosition = (x, y);
							blizzards.Add(bliz);
							break;
						case 'v':
							bliz.direction = mapChar;
							bliz.startPosition = (x, y);
							blizzards.Add(bliz);
							break;
					}

				}
			}

			#endregion

			Stack<State> states = new Stack<State>();

			// create starting state
			State startingState = new State();
			startingState.time = 0;
			startingState.position = start;
			startingState.path.Add(start);

			// add starting state to stack
			states.Push(startingState);

			State bestState = new State();
			bestState.time = int.MaxValue;

			int iterations = 0;

			while (states.Count > 0)
			{
				var state = states.Pop();

				// create positions for each direction
				(int x, int y)[] dirs = { state.position, state.position, state.position, state.position, state.position };

				if (iterations % 1000 == 0)
				{
					Console.WriteLine("States: " + states.Count + " Iterations: " + iterations);
				}

				// ordered by priority so DFS will try and go down and right first
				// then try waiting, then go up and left
				dirs[0].x--; // left
				dirs[1].y--; // up
				dirs[2] = state.position; // keep this one the same to act as waiting
				dirs[3].x++; // right
				dirs[4].y++; // down

				// evaluate possibilities
				for (int i = 0; i < dirs.Length; i++)
				{
					var d = dirs[i];

					// check if it's a bingo before anything else
					if (d == end)
					{
						if (state.time + 1 < bestState.time)
						{
							State endState = new State(state);
							endState.position = d;
							endState.path.Add(d);
							Console.WriteLine("New Best State Found!");
							DrawMap(map, start, end, blizzards, endState);
							bestState = endState;
						}
					}

					// ensure this won't take us off the edge of the map 
					bool offMap = d.x < 1 || d.x > map.width - 2 || d.y < 1 || d.y > map.height - 2;
					bool waiting = d == state.position;
					if (offMap && !waiting)
					{
						continue;
					}

					// check if the position will have blizzards
					bool obstructed = false;
					for (int b = 0; b < blizzards.Count; b++)
					{
						if (blizzards[b].GetPosition(state.time + 1) == d)
						{
							obstructed = true;
							continue;
						}
					}

					// this position is not in a wall, nor will it be in a blizzard next step
					if (!obstructed && state.time + 1 <= bestState.time)
					{
						State newState = new State(state);
						newState.position = d;
						newState.path.Add(d);
						states.Push(newState);
					}
				}

				iterations++;
			}
			DrawMap(map, start, end, blizzards, bestState);
		}

		public class State
		{
			public State()
			{
				time = 0;
				position = (0, 0);
				path = new List<(int x, int y)>();
			}

			public State(State s)
			{
				time = s.time + 1;
				position = s.position;
				path = s.path;
			}

			public override string ToString()
			{
				return position.ToString() + path.Count;
			}

			public int time;
			public (int x, int y) position;
			public List<(int x, int y)> path;
		}

		public class Blizzard
		{
			public char direction;

			public (int x, int y) startPosition;

			public (int width, int height) map;

			public Blizzard((int width, int height) m)
			{
				map = m;
			}

			public (int x, int y) GetPosition(int time)
			{
				if (time == 0) return startPosition;

				var nextPos = startPosition;
				switch (direction)
				{
					case '>':
						nextPos.x += time;
						break;
					case '<':
						nextPos.x -= time;
						break;
					case '^':
						nextPos.y -= time;
						break;
					case 'v':
						nextPos.y += time;
						break;
				}


				while (nextPos.x < 1) nextPos.x += map.width - 2;
				while (nextPos.x > map.width - 2) nextPos.x -= map.width - 2;
				while (nextPos.y < 1) nextPos.y += map.height - 2;
				while (nextPos.y > map.height - 2) nextPos.y -= map.height - 2;
				return nextPos;
			}
		}

		public static void DrawMap((int x, int y) rect, (int x, int y) start, (int x, int y) end, List<Blizzard> blizzards, State state)
		{
			Console.WriteLine("Time: " + state.time);
			for (int y = 0; y < rect.y; y++)
			{
				for (int x = 0; x < rect.x; x++)
				{
					// bliz on this tile
					bool bliz = false;

					Console.BackgroundColor = ConsoleColor.Black;
					if (state.path.Contains((x, y)))
					{
						Console.BackgroundColor = ConsoleColor.DarkRed;
					}

					if ((x, y) == state.position)
					{
						Console.Write('@');
					}
					else
					{
						for (int i = 0; i < blizzards.Count; i++)
						{
							if (!bliz && blizzards[i].GetPosition(state.time) == (x, y))
							{
								Console.Write(blizzards[i].direction);
								bliz = true;
							}
						}

						if (bliz)
						{
							// do nothing
						}
						else if ((x, y) == start || (x, y) == end)
						{
							Console.Write(".");
						}
						else if (x == 0 || x == rect.x - 1 || y == 0 || y == rect.y - 1)
						{
							Console.Write("#");
						}
						else
						{
							Console.Write(".");
						}
					}
				}
				Console.Write("\n");
			}
		}
	}
}
