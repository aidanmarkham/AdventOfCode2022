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
		public static string inputFile = "gps.txt";
		public static long decryptionKey = 811589153;
		public static bool debug = false;
		public static void Solve()
		{
			// read in the directions as an array of chars
			var input = File.ReadAllLines(inputFile);

			(long value, long initialPosition) zero = (0, 0);

			// create encrypted file list
			List<(long value, long initialPosition)> encryptedFile = new List<(long value, long initialPosition)>();
			for (int i = 0; i < input.Length; i++)
			{
				encryptedFile.Add((long.Parse(input[i]) * decryptionKey, i));
				if (encryptedFile[i].value == 0) zero = encryptedFile[i];
			}

			var length = encryptedFile.Count;

			// copy that list into the mixed list
			var mixedFile = new List<(long value, long initialPosition)>(encryptedFile);

			// output the mixed file

			if (debug)
			{
				Console.WriteLine("Initial Order");
				for (int m = 0; m < mixedFile.Count; m++)
				{
					Console.Write(mixedFile[m].value + " ");
				}
				Console.Write("\n\n");
			}

			for (int x = 0; x < 10; x++)
			{
				// go through the encrypted file and mix the values
				for (int i = 0; i < encryptedFile.Count; i++)
				{
					long index = mixedFile.IndexOf(encryptedFile[i]);
					long distance = encryptedFile[i].value;
					long newIndex = LoopIndex(length, index + distance);

					if (debug)
					{
						if (distance == 0)
						{
							Console.WriteLine(distance + " does not move.");
						}
						else
						{
							Console.WriteLine(distance + " moves between " + mixedFile[(int)newIndex].value + " and " + mixedFile[(int)LoopIndex(length, newIndex + 1)].value);
						}
					}
					mixedFile.RemoveAt((int)index);
					mixedFile.Insert((int)newIndex, encryptedFile[i]);

					if (debug)
					{
						// print the mixed file
						for (int m = 0; m < mixedFile.Count; m++)
						{
							Console.Write(mixedFile[m].value + " ");
						}
						Console.Write("\n\n");
					}
				}
			}

			var zeroIndex = mixedFile.IndexOf(zero);
			long oneK = mixedFile[(int)LoopIndex(length + 1, zeroIndex + 1000)].value;
			long twoK = mixedFile[(int)LoopIndex(length + 1, zeroIndex + 2000)].value;
			long threeK = mixedFile[(int)LoopIndex(length + 1, zeroIndex + 3000)].value;

			Console.WriteLine("Values after zero at index " + zeroIndex + ": " + oneK + " " + twoK + " " + threeK + " Total: " + (oneK + twoK + threeK));
		}

		public static long LoopIndex(long length, long index)
		{
			long x = index;
			long m = length - 1;
			return (x % m + m) % m;
		}

	}
}
