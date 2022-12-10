using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class SignalStrengthCalculator
	{
		public static string inputFile = "signal.txt";

		public static void CalculateSignalStrength()
		{
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);

			Console.WriteLine("Input file loaded! Calculating...");

			int sampleStart = 20;
			int sampleFrequency = 40;

			int cycle = 1;
			int command = 0;
			bool complete = false;
			int register = 1;

			int sumStrength = 0;

			while (command < input.Length)
			{
				// parse command
				var words = input[command].Split(' ');
				var instr = words[0];
				var num = -1;

				if (instr == "addx")
				{
					num = int.Parse(words[1]);
				}

				if(instr == "noop")
				{
					// do nothing but burn a cycle
					cycle++;
					command++;
				}
				else // addx
				{
					// nothing happens on the first cycles
					cycle++;


					if (cycle >= sampleStart && (cycle - sampleStart) % sampleFrequency == 0)
					{
						sumStrength += cycle * register;
					}

					// but then we add to x
					register += num;

					// then move on 
					cycle++;
					command++;
				}

				if (cycle >= sampleStart && (cycle - sampleStart) % sampleFrequency == 0)
				{
					sumStrength += cycle * register;
				}
			}

			Console.WriteLine("Summed Strength:" + sumStrength);
		}

		public static void RenderSignal()
		{
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);

			Console.WriteLine("Input file loaded! Calculating...");

			int sampleStart = 20;
			int sampleFrequency = 40;

			int cycle = 1;
			int command = 0;
			int register = 1;

			int sumStrength = 0;

			while (command < input.Length)
			{
				// parse command
				var words = input[command].Split(' ');
				var instr = words[0];
				var num = -1;

				if (instr == "addx")
				{
					num = int.Parse(words[1]);
				}

				if (instr == "noop")
				{
					Render(cycle, sampleFrequency, register);

					// do nothing but burn a cycle
					cycle++;
					command++;
				}
				else // addx
				{
					Render(cycle, sampleFrequency, register);

					// nothing happens on the first cycles
					cycle++;

					Render(cycle, sampleFrequency, register);
					if (cycle >= sampleStart && (cycle - sampleStart) % sampleFrequency == 0)
					{
						sumStrength += cycle * register;
					}

					// but then we add to x
					register += num;

					// then move on 
					cycle++;
					command++;
				}

				if (cycle >= sampleStart && (cycle - sampleStart) % sampleFrequency == 0)
				{
					sumStrength += cycle * register;
				}
			}

			Console.WriteLine("Summed Strength:" + sumStrength);
		}
		private static void Render(int cycle, int freq, int register)
		{
			int pixel = cycle % freq;
			pixel--;
			if (register == pixel|| register == pixel - 1 || register == pixel + 1)
			{
				Console.Write("█"); //█
			}
			else
			{
				Console.Write(" ");
			}

			

			if (cycle % freq == 0)
			{
				Console.Write("\n");
			}
		}
	}

	
}
