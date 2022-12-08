using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class TreeVisibility
	{
		public static string inputFile = "trees.txt";

		public static void CalculateVisibility()
		{
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);

			var height = input.Length;
			var width = input[0].Length;

			int[,] trees = new int[height, width];

			for (int i = 0; i < input.Length; i++)
			{
				for (int j = 0; j < input[i].Length; j++)
				{
					trees[i, j] = int.Parse("" + input[i][j]);
				}
			}

			List<string> visibleTrees = new List<string>();

			// for each horizontal row 
			for (int y = 0; y < height; y++)
			{
				var currentHeight = -1;
				// check from left to right
				for (int x = 0; x < width; x++)
				{
					if (trees[y, x] > currentHeight)
					{
						if (!visibleTrees.Contains("Tree " + y + x))
						{
							visibleTrees.Add("Tree " + y + x);
							Console.WriteLine("Tree " + y + ", " + x + " is visible");
						}

						//Console.WriteLine("Tree " + y + ", " + x + " is visible from the left. Total: " + visibleTrees.Count);
						currentHeight = trees[y, x];
					}
					else
					{
						//Console.WriteLine("Tree " + y + ", " + x + " is NOT visible from the left. Total: " + visibleTrees.Count);
					}
				}
			}
			// for each horizontal row 
			for (int y = 0; y < height; y++)
			{
				var currentHeight = -1;
				// and right to left
				for (int x = width - 1; x >= 0; x--)
				{
					if (trees[y, x] > currentHeight)
					{
						if (!visibleTrees.Contains("Tree " + y + x))
						{
							visibleTrees.Add("Tree " + y + x);
							Console.WriteLine("Tree " + y + ", " + x + " is visible");
						}

						Console.WriteLine("Tree " + y + ", " + x + " is visible from the right. Total: " + visibleTrees.Count);
						currentHeight = trees[y, x];
					}
					else
					{
						Console.WriteLine("Tree " + y + ", " + x + " is NOT visible from the right. Total: " + visibleTrees.Count);
					}
				}
			}

			// same deal now vertical first
			// for each horizontal row 
			for (int x = 0; x < width; x++)
			{
				var currentHeight = -1;
				// check from left to right
				for (int y = 0; y < height; y++)
				{
					if (trees[y, x] > currentHeight)
					{
						if (!visibleTrees.Contains("Tree " + y + x))
						{
							visibleTrees.Add("Tree " + y + x);
							Console.WriteLine("Tree " + y + ", " + x + " is visible");
						}
						//Console.WriteLine("Tree " + y + ", " + x + " is visible from the top. Total: " + visibleTrees.Count);
						currentHeight = trees[y, x];
					}
					else
					{
						//Console.WriteLine("Tree " + y + ", " + x + " is NOT visible from the top. Total: " + visibleTrees.Count);
					}
				}
			}
			for (int x = 0; x < width; x++)
			{
				var currentHeight = -1;
				// and right to left
				for (int y = height - 1; y >= 0; y--)
				{
					if (trees[y, x] > currentHeight)
					{
						if (!visibleTrees.Contains("Tree " + y + x))
						{
							visibleTrees.Add("Tree " + y + x);
							Console.WriteLine("Tree " + y + ", " + x + " is visible");
						}
						//Console.WriteLine("Tree " + y + ", " + x + " is visible from the bottom. Total: " + visibleTrees.Count);
						currentHeight = trees[y, x];
					}
					else
					{
						//Console.WriteLine("Tree " + y + ", " + x + " is NOT visible from the bottom. Total: " + visibleTrees.Count);
					}
				}
			}

			Console.WriteLine("Trees Visible: " + visibleTrees.Count);
		}


	}
}
