using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class Day23
	{
		public static string inputFile = "elves.txt";
		public static int rounds = 10;

		public static void Solve()
		{
			#region Loading
			var input = File.ReadAllLines(inputFile);

			HashSet<(int x, int y)> elves = new HashSet<(int x, int y)>();

			for (int x = 0; x < input.Length; x++)
			{
				for (int y = 0; y < input[x].Length; y++)
				{
					if (input[x][y] == '#')
					{
						elves.Add((x, y));
					}
				}
			}
			#endregion

			List<char> directions = new List<char>();
			directions.Add('N');
			directions.Add('S');
			directions.Add('W');
			directions.Add('E');

			Console.WriteLine("Initial State: ");
			// print the field at half time
			PrintField(elves);

			bool moving = true;
			int round = 0;
			while (moving)
			{
				round++;

				// key: proposed spot, value: elves that have proposed that spot
				Dictionary<(int x, int y), List<(int x, int y)>> proposals = new Dictionary<(int x, int y), List<(int x, int y)>>();

				// key: elf, value: proposed spot
				Dictionary<(int x, int y), (int x, int y)> elfPropMap = new Dictionary<(int x, int y), (int x, int y)>();

				foreach (var elf in elves)
				{

					// this elf isn't alone, so it should propose a move
					if (HasNeighbor(elf, elves))
					{
						for (int d = 0; d < directions.Count; d++)
						{
							// this direction is clear
							if (CheckDir(directions[d], elf, elves))
							{
								var proposed = elf;
								switch (directions[d])
								{
									case 'N':
										proposed.x--;
										break;
									case 'S':
										proposed.x++;
										break;
									case 'E':
										proposed.y++;
										break;
									case 'W':
										proposed.y--;
										break;
								}


								// build proposals
								if (proposals.ContainsKey(proposed))
								{
									proposals[proposed].Add(elf);
								}
								else
								{
									proposals.Add(proposed, new List<(int x, int y)>());
									proposals[proposed].Add(elf);
								}

								// add this to the elf / proposed map for later retrieval
								elfPropMap.Add(elf, proposed);

								break;
							}
						}
					}
				}

				/*
				// print the field at half time
				Console.Write("Half time: ");
				for (int i = 0; i < directions.Count; i++)
				{
					Console.Write(directions[i]);
				}
				Console.Write("\n");
				PrintField(elves, proposals);
				*/

				int moveCount = 0;
				// second half (make moves)
				// calculate min and max
				Dictionary<(int x, int y), (int x, int y)> moves = new Dictionary<(int x, int y), (int x, int y)>();
				foreach (var elf in elves)
				{
					if (elfPropMap.ContainsKey(elf))
					{
						var proposed = elfPropMap[elf];

						if (proposals.ContainsKey(proposed) && proposals[proposed].Count == 1)
						{
							// move!
							moves.Add(elf, proposed);
							moveCount++;
						}
					}
				}

				foreach(KeyValuePair<(int x, int y), (int x, int y)> move in moves)
				{
					elves.Remove(move.Key);
					elves.Add(move.Value);
				}

				if (round % 10 == 0)
				{
					Console.WriteLine("Rounds: " + round + " Last Moves: " + moveCount);
				}


				if (moveCount == 0)
				{
					moving = false;
				}

				// rotate proposals
				var dir = directions[0];
				directions.RemoveAt(0);
				directions.Add(dir);
			}

			Console.WriteLine("The board after " + round + " rounds.");
			// print the field at half time
			PrintField(elves);

			Console.WriteLine("Empties: " + CountEmpties(elves));
		}

		public static bool HasNeighbor((int x, int y) elf, HashSet<(int x, int y)> elves)
		{
			var has = false;
			if (elves.Contains((elf.x, elf.y - 1))) has = true;
			if (elves.Contains((elf.x + 1, elf.y - 1))) has = true;
			if (elves.Contains((elf.x + 1, elf.y))) has = true;
			if (elves.Contains((elf.x + 1, elf.y + 1))) has = true;
			if (elves.Contains((elf.x, elf.y + 1))) has = true;
			if (elves.Contains((elf.x - 1, elf.y + 1))) has = true;
			if (elves.Contains((elf.x - 1, elf.y))) has = true;
			if (elves.Contains((elf.x - 1, elf.y - 1))) has = true;
			return has;
		}

		public static bool CheckDir(char dir, (int x, int y) elf, HashSet<(int x, int y)> elves)
		{
			var clear = true;

			switch (dir)
			{
				case 'N':
					if (elves.Contains((elf.x - 1, elf.y + 1))) clear = false;
					if (elves.Contains((elf.x - 1, elf.y))) clear = false;
					if (elves.Contains((elf.x - 1, elf.y - 1))) clear = false;
					break;
				case 'S':
					if (elves.Contains((elf.x + 1, elf.y + 1))) clear = false;
					if (elves.Contains((elf.x + 1, elf.y))) clear = false;
					if (elves.Contains((elf.x + 1, elf.y - 1))) clear = false;
					break;
				case 'E':
					if (elves.Contains((elf.x + 1, elf.y + 1))) clear = false;
					if (elves.Contains((elf.x, elf.y + 1))) clear = false;
					if (elves.Contains((elf.x - 1, elf.y + 1))) clear = false;
					break;
				case 'W':
					if (elves.Contains((elf.x + 1, elf.y - 1))) clear = false;
					if (elves.Contains((elf.x, elf.y - 1))) clear = false;
					if (elves.Contains((elf.x - 1, elf.y - 1))) clear = false;
					break;
			}

			return clear;
		}

		public static void PrintField(HashSet<(int x, int y)> elves, Dictionary<(int x, int y), List<(int x, int y)>> proposals = null, int padding = 2)
		{
			(int x, int y) min = (int.MaxValue, int.MaxValue);
			(int x, int y) max = (int.MinValue, int.MinValue);

			// calculate min and max
			foreach (var e in elves)
			{
				if (e.x < min.x) min.x = e.x;
				if (e.x > max.x) max.x = e.x;
				if (e.y < min.y) min.y = e.y;
				if (e.y > max.y) max.y = e.y;
			}

			// add padding

			min.x -= padding;
			min.y -= padding;
			max.x += padding;
			max.y += padding;


			for (int x = min.x; x <= max.x; x++)
			{
				for (int y = min.y; y <= max.y; y++)
				{
					if (elves.Contains((x, y)))
					{
						Console.Write('#');
					}
					else if (proposals != null && proposals.ContainsKey((x, y)))
					{
						Console.Write(proposals[(x, y)].Count);
					}
					else
					{
						Console.Write('.');
					}
				}
				Console.Write("\n");
			}

			Console.Write("\n");
		}

		public static int CountEmpties(HashSet<(int x, int y)> elves)
		{
			(int x, int y) min = (int.MaxValue, int.MaxValue);
			(int x, int y) max = (int.MinValue, int.MinValue);

			// calculate min and max
			foreach(var e in elves)
			{
				if (e.x < min.x) min.x = e.x;
				if (e.x > max.x) max.x = e.x;
				if (e.y < min.y) min.y = e.y;
				if (e.y > max.y) max.y = e.y;
			}

			int empties = 0;

			for (int x = min.x; x <= max.x; x++)
			{
				for (int y = min.y; y <= max.y; y++)
				{
					if (!elves.Contains((x, y)))
					{
						empties++;
					}
				}
			}

			return empties;
		}
	}
}
