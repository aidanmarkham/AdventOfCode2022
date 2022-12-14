using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class Day13
	{

		public static string inputFile = "distress.txt";
		public static void Solve()
		{
			// build map
			var input = File.ReadAllLines(inputFile);

			List<IntList> intLists = new List<IntList>();

			for (int i = 0; i < input.Length; i += 3)
			{
				var list = new IntList(input[i]);
				intLists.Add(list);

				list = new IntList(input[i + 1]);
				intLists.Add(list);
			}

			Console.WriteLine("Pre Sort: ");
			for (int i = 0; i < intLists.Count; i++)
			{
				Console.WriteLine(intLists[i].ToString());
			}

			intLists.Add(new IntList("[[2]]"));
			intLists.Add(new IntList("[[6]]"));

			intLists.Sort(delegate (IntList a, IntList b) { return -Compare(a, b); });

			Console.WriteLine("Is In Order: " + IsOrdered(intLists));
			int indices = 0;
			Console.WriteLine("\n\nPost Sort: ");
			for (int i = 0; i < intLists.Count; i++)
			{
				Console.WriteLine(intLists[i].ToString());
				if(intLists[i].ToString() == "[[2]]" || intLists[i].ToString() == "[[6]]")
				{
					if(indices == 0)
					{
						indices = i + 1;
					}
					else
					{
						indices *= i + 1;
					}
				}
			}

			Console.WriteLine("indi " + indices);
		}

		static bool IsOrdered(List<IntList> intLists)
		{
			for (int i = 0; i < intLists.Count - 1; i++)
			{
				int comp = Compare(intLists[i], intLists[i + 1]);
				if (comp == -1) return false;
			}
			return true;
		}

		static int Compare(IntList a, IntList b)
		{
			IntList aList = a;
			IntList bList = b;

			// both ints
			if (a.Int > -1 && b.Int > -1)
			{
				if (a.Int != b.Int)
				{
					return a.Int < b.Int ? 1 : -1;
				}
			}
			// only a is int
			else if (a.Int > -1)
			{
				aList = new IntList("[" + a.Int.ToString() + "]");
			}
			// only b is int
			else if (b.Int > -1)
			{
				bList = new IntList("[" + b.Int.ToString() + "]");
			}

			// both are lists or empty now 
			for (int x = 0; x < Math.Max(aList.List.Count, bList.List.Count); x++)
			{
				if (x >= aList.List.Count)
				{
					return 1;
				}
				if (x >= bList.List.Count)
				{
					return -1;
				}

				int comp = Compare(aList.List[x], bList.List[x]);
				if (comp != 0)
				{
					return comp;
				}
			}

			return 0;
		}

		static List<string> BracketSplit(string s)
		{
			// split off outside brackets
			s = s.Substring(1, s.Length - 2);
			List<string> result = new List<string>();

			int startIndex = 0;
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == ',')
				{
					result.Add(s.Substring(startIndex, i - startIndex));
					startIndex = i + 1;
				}
				if (s[i] == '[')
				{
					int level = 0;
					while (s[i] != ']' || level != 0)
					{

						i++;
						if (s[i] == '[')
						{
							level++;
						}
						while (s[i] == ']' && level > 0)
						{
							level--;
							i++;
						}
					}
					i++;
					result.Add(s.Substring(startIndex, i - startIndex));
					startIndex = i + 1;
				}
			}

			if (s.Length - startIndex > 0)
			{
				result.Add(s.Substring(startIndex));
			}
			return result;
		}

		public class IntList
		{
			public int Int = -1;
			public List<IntList> List = new List<IntList>();

			public IntList(string input)
			{
				if (int.TryParse(input, out var parsed))
				{
					Int = parsed;
				}
				else if (input.Length > 0)
				{
					// it's a list
					var split = BracketSplit(input);
					for (int i = 0; i < split.Count; i++)
					{
						List.Add(new IntList(split[i]));
					}
				}
			}

			public override string ToString()
			{
				string s = "";
				if (List.Count > 0) s += "[";
				for (int i = 0; i < List.Count; i++)
				{
					s += List[i].ToString();
				}
				if (List.Count > 0) s += "]";
				if (Int != -1) s += Int;
				return s + "";
			}
		}

	}
}
