using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class Day21
	{
		public static string inputFile = "monkeys.txt";

		public static void Solve()
		{
			// build a dictionary of monkeys
			var input = File.ReadAllLines(inputFile);
			var monkeys = new Dictionary<string, Monkey>();
			for (int i = 0; i < input.Length; i++)
			{
				var words = input[i].Split(' ');

				var monkey = new Monkey();
				monkey.name = words[0].Substring(0, 4);

				if (int.TryParse(words[1], out int result))
				{
					monkey.number = result;
				}
				else
				{
					monkey.number = -1;
					monkey.a = words[1];
					monkey.op = words[2][0];
					monkey.b = words[3];
				}

				monkeys.Add(monkey.name, monkey);
			}

			// out pt 1
			//Console.WriteLine("Part 1 Root Value: " + monkeys["root"].GetValue(monkeys, monkeys["humn"].number));

			monkeys["root"].op = '=';

			// get operation strings from monkeys
			Console.WriteLine("Side A: ");
			var opString = monkeys[monkeys["root"].a].GetEquation(monkeys);
			Console.WriteLine(opString);

			Console.WriteLine("Side B: ");
			long bVal = monkeys[monkeys["root"].b].GetValue(monkeys, 0);
			Console.WriteLine(bVal);

			List<Operation> ops = new List<Operation>();

			// loop through a until we've built a full list of terms
			bool completed = false;
			while (!completed)
			{
				// we're out of ops
				if (!opString.Contains('(') && !opString.Contains(')'))
				{
					completed = true;
					var lastOp = new Operation();
					lastOp.op = opString[1];
					lastOp.val = long.Parse(opString.Substring(2));
					lastOp.side = 1;
					ops.Add(lastOp);
					break;
				}

				var first = opString[0];
				var last = opString[opString.Length - 1];

				string removedOperation = "";
				int side = 0; // -1 = left, 1 = right

				// it's completely encapsulated in parens
				if (first == '(' && last == ')')
				{
					opString = opString.Substring(1, opString.Length - 2);
				}
				// there's a term on the end of the string
				else if (last != ')')
				{
					int parenEnd = opString.LastIndexOf(')');
					removedOperation = opString.Substring(parenEnd + 1);
					opString = opString.Substring(0, parenEnd + 1);
					side = 1;
				}
				// there's a term on the beginning of the string
				else if (first != '(')
				{
					int parenStart = opString.IndexOf('(');
					removedOperation = opString.Substring(0, parenStart);
					opString = opString.Substring(parenStart, opString.Length - parenStart);
					side = -1;
				}



				if (side != 0)
				{
					var newOp = new Operation();
					newOp.side = side;

					// left 
					if (side == -1)
					{
						// operator is last element of string
						newOp.op = removedOperation[removedOperation.Length - 1];
						newOp.val = long.Parse(removedOperation.Substring(0, removedOperation.Length - 1));
					}
					// right
					else
					{
						// operator is first element of string
						newOp.op = removedOperation[0];
						newOp.val = long.Parse(removedOperation.Substring(1, removedOperation.Length - 1));
					}
					ops.Add(newOp);
				}
			}

			// flip them so the order of ops is correcto
			//ops.Reverse();

			double scream = bVal;
			Console.WriteLine("Test ops with val " + scream + ": ");
			for (int i = 0; i < ops.Count; i++)
			{
				scream = ops[i].Inverse(scream);
				Console.WriteLine(ops[i] + ": " +scream);
			}

			Console.WriteLine("I have a mouth and I must scream " + Math.Round(scream));
		}

		public class Monkey
		{
			public string name;
			public long number;
			public string a;
			public char op;
			public string b;

			public long GetValue(Dictionary<string, Monkey> monkeys, long humn)
			{
				if (name == "humn")
				{
					return humn;
				}

				if (number != -1)
				{
					return number;
				}
				else
				{
					long aVal = monkeys[a].GetValue(monkeys, humn);
					long bVal = monkeys[b].GetValue(monkeys, humn);

					switch (op)
					{
						case '+':
							return aVal + bVal;
						case '-':
							return aVal - bVal;
						case '*':
							return aVal * bVal;
						case '/':
							return aVal / bVal;
						case '=':
							return aVal == bVal ? 1 : -1;
					}
				}

				Console.WriteLine("Hit unhandled case!");
				return 0;
			}
			public string GetEquation(Dictionary<string, Monkey> monkeys)
			{
				if (name == "humn")
				{
					return "X";
				}

				if (number != -1)
				{
					return number.ToString();
				}
				else
				{
					var aEq = monkeys[a].GetEquation(monkeys);
					var bEq = monkeys[b].GetEquation(monkeys);

					// try our best to simplify
					if (long.TryParse(aEq, out var aVal) && long.TryParse(bEq, out var bVal))
					{
						switch (op)
						{
							case '+':
								return (aVal + bVal).ToString();
							case '-':
								return (aVal - bVal).ToString();
							case '*':
								return (aVal * bVal).ToString();
							case '/':
								return (aVal / bVal).ToString();
							case '=':
								return "(" + aEq + op + bEq + ")";
						}
					}

					return "(" + aEq + op + bEq + ")";
				}
			}
		}

		public struct Operation
		{
			public int side;
			public char op;
			public long val;

			public override string ToString()
			{
				if(side == -1)
				{
					return val + op.ToString() + "x";
				}
				else
				{
					return "x" + op + val;
				}
			}

			public double Execute(double x)
			{
				double a = val;
				double b = x;
				if(side == 1)
				{
					a = x;
					b = val;
				}
				switch (op)
				{
					case '+':
						return a + b;
					case '-':
						return a - b;
					case '*':
						return a * b;
					case '/':
						return a / b;
					case '=':
						return a == b ? 1 : -1;
				}

				return 0;
			}

			public double Inverse(double x)
			{

				if (side == 1)
				{
					double a = x;
					double b = val;
					switch (op)
					{
						case '+':
							return a - b;
						case '-':
							return a + b; // good!
						case '*':
							return a / b;
						case '/':
							return a * b; // good!
						case '=':
							return a != b ? 1 : -1;
					}
				}
				else
				{
					double a = val;
					double b = x;
					switch (op)
					{
						case '+':
							return b - a; // good! 
						case '-':
							return a - b; // good!
						case '*':
							return b / a; // good!
						case '/':
							return a * b;
						case '=':
							return a != b ? 1 : -1;
					}
				}
				return 0;
			}
		}

	}
}
