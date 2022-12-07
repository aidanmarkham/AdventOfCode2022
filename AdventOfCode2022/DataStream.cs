using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class DataStream
	{
		public static string inputFile = "datastream.txt";

		public static void FindMarker()
		{
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);
			string stream = input[0];

			Console.WriteLine("Input file loaded! Calculating...");

			Queue<char> lastFour = new Queue<char>();

			for(int i =0; i < stream.Length; i++)
			{
				if(lastFour.Count == 14)
				{
					lastFour.Dequeue();
				}

				var newChar = stream[i];

				lastFour.Enqueue(newChar);

				if(lastFour.Distinct().Count() == 14)
				{
					Console.WriteLine("Marker ended " + (i+1) + " characters into the stream.");
					break;
				}
			}
		}
	}
}
