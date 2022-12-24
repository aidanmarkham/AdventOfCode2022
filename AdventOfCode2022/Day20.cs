using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class Day20
	{
		public static string inputFile = "gps_test.txt";
		public static void Solve()
		{
			// read in the directions as an array of chars
			var input = File.ReadAllLines(inputFile);

			List<(int value, int initialPosition)> encryptedFile = new List<(int value, int initialPosition)>();
			for(int i = 0; i < input.Length; i++)
			{
				encryptedFile.Add((int.Parse(input[i]), i));
			}

			for(int i = 0; i < encryptedFile.Count; i++)
			{
				Console.WriteLine("Value: " + encryptedFile[i].value + " Initial Position: " + encryptedFile[i].initialPosition);
			}
		}
	}
}
