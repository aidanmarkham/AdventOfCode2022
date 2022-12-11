using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class MonkeyInTheMiddle
	{

		public static void Solve()
		{
			var input = File.ReadAllLines("eleven.txt");

			var monkeys = new List<Monkey>();

			// calc monks
			for (int i = 0; i < input.Length; i++)
			{
				var words = input[i].Split(' ');

				if (words[0] == "Monkey")
				{
					var monkey = new Monkey();

					i++;
					words = input[i].Split(' ');

					var items = words.Skip(4).ToArray();

					for (int j = 0; j < items.Length; j++)
					{
						monkey.AddItem(int.Parse(Regex.Replace(items[j], "[,]", string.Empty)));
					}

					i++;
					words = input[i].Split(' ');

					monkey.Operation = words.Skip(3).ToArray();

					i++;
					monkey.Divisor = int.Parse(input[i].Split(' ')[5]);

					i++;
					monkey.TrueMonkey = int.Parse(input[i].Split(' ')[9]);

					i++;
					monkey.FalseMonkey = int.Parse(input[i].Split(' ')[9]);

					monkeys.Add(monkey);

					i++;
				}
			}

			for (int t = 0; t < 10000; t++)
			{
				for (int i = 0; i < monkeys.Count; i++)
				{
					// inpect items
					for (int j = 0; j < monkeys[i].Items.Count; j++)
					{

						// do inspect the item
						double val = monkeys[i].Operation[4] == "old" ? monkeys[i].Items[j] : double.Parse(monkeys[i].Operation[4]);

						monkeys[i].Inspections++;

						if (monkeys[i].Operation[3] == "*")
						{
							monkeys[i].Items[j] *= val;
						}
						else if (monkeys[i].Operation[3] == "+")
						{
							monkeys[i].Items[j] += val;
						}

						// apply worry adjustment strategy
						monkeys[i].Items[j] /= 3;

						if (monkeys[i].Items[j] % monkeys[i].Divisor == 0)
						{
							monkeys[monkeys[i].TrueMonkey].Items.Add(monkeys[i].Items[j]);
						}
						else
						{
							monkeys[monkeys[i].FalseMonkey].Items.Add(monkeys[i].Items[j]);
						}
					}
					monkeys[i].Items.Clear();
				}
			}

			var sortedMonk = monkeys.OrderBy(o => -o.Inspections).ToList();

			Console.WriteLine("Most: " + sortedMonk[0].Inspections);

			Console.WriteLine("Answer: " + (sortedMonk[0].Inspections * sortedMonk[1].Inspections));

			Console.WriteLine("Leaderboard");
			for (int i = 0; i < monkeys.Count; i++)
			{
				Console.WriteLine("Monkey " + i + ": " + monkeys[i].Inspections);
			}
		}

		public static void Solve2()
		{
			var input = File.ReadAllLines("eleven.txt");

			var monkeys = new List<Monkey>();

			// calc monks
			for (int i = 0; i < input.Length; i++)
			{
				var words = input[i].Split(' ');

				if (words[0] == "Monkey")
				{
					var monkey = new Monkey();

					i++;
					words = input[i].Split(' ');

					var items = words.Skip(4).ToArray();

					for (int j = 0; j < items.Length; j++)
					{
						monkey.AddItem(int.Parse(Regex.Replace(items[j], "[,]", string.Empty)));
					}

					i++;
					words = input[i].Split(' ');

					monkey.Operation = words.Skip(3).ToArray();

					i++;
					monkey.Divisor = int.Parse(input[i].Split(' ')[5]);

					i++;
					monkey.TrueMonkey = int.Parse(input[i].Split(' ')[9]);

					i++;
					monkey.FalseMonkey = int.Parse(input[i].Split(' ')[9]);

					monkeys.Add(monkey);

					i++;
				}
			}

			var lcm = monkeys[0].Divisor;
			for (int i = 1; i < monkeys.Count; i++)
			{
				lcm *= monkeys[i].Divisor;
			}

			Console.WriteLine(lcm);

			for (int t = 0; t < 10000; t++)
			{
				for (int i = 0; i < monkeys.Count; i++)
				{
					// inpect items
					for (int j = 0; j < monkeys[i].Items.Count; j++)
					{

						// do inspect the item
						double val = monkeys[i].Operation[4] == "old" ? monkeys[i].Items[j] : double.Parse(monkeys[i].Operation[4]);

						monkeys[i].Inspections++;

						if (monkeys[i].Operation[3] == "*")
						{
							monkeys[i].Items[j] *= val;
						}
						else if (monkeys[i].Operation[3] == "+")
						{
							monkeys[i].Items[j] += val;
						}
						else
						{
							Console.WriteLine("Huh?");
						}

						// apply worry adjustment strategy
						monkeys[i].Items[j] %= lcm;

						if (monkeys[i].Items[j] % monkeys[i].Divisor == 0)
						{
							monkeys[monkeys[i].TrueMonkey].Items.Add(monkeys[i].Items[j]);
						}
						else
						{
							monkeys[monkeys[i].FalseMonkey].Items.Add(monkeys[i].Items[j]);
						}
					}
					monkeys[i].Items.Clear();
				}
			}

			var sortedMonk = monkeys.OrderBy(o => -o.Inspections).ToList();

			Console.WriteLine("Most: " + sortedMonk[0].Inspections);

			Console.WriteLine("Answer: " + (sortedMonk[0].Inspections * sortedMonk[1].Inspections));

			Console.WriteLine("Leaderboard");
			for (int i = 0; i < monkeys.Count; i++)
			{
				Console.WriteLine("Monkey " + i + ": " + monkeys[i].Inspections);
			}
		}


		public class Monkey
		{

			public void AddItem(int i)
			{
				Items.Add(i);
			}

			public List<double> Items = new List<double>();
			public string[] Operation;
			public int Divisor;
			public int TrueMonkey;
			public int FalseMonkey;
			public double Inspections = 0;
		}
	}
}
