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
			bool[,] visibleTrees = new bool[height, width];
			for (int i = 0; i < input.Length; i++)
			{
				for (int j = 0; j < input[i].Length; j++)
				{
					trees[i, j] = int.Parse("" + input[i][j]);
				}
			}

			// for each horizontal row 
			for (int y = 0; y < height; y++)
			{
				var currentHeight = -1;
				// check from left to right
				for (int x = 0; x < width; x++)
				{
					if (trees[y, x] > currentHeight)
					{
						visibleTrees[y, x] = true;
						currentHeight = trees[y, x];
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
						visibleTrees[y, x] = true;
						currentHeight = trees[y, x];
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
						visibleTrees[y, x] = true;
						currentHeight = trees[y, x];
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
						visibleTrees[y, x] = true;
						currentHeight = trees[y, x];
					}
				}
			}

			int visible = 0;
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{

					Console.Write(visibleTrees[x, y] ? '▓' : '░');

					if (visibleTrees[x, y]) visible++;
				}
				Console.Write("\n");
			}

			Console.WriteLine("Trees Visible: " + visible);
		}

		public static void CalculateTreeVisibility()
		{
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);

			var height = input.Length;
			var width = input[0].Length;

			// build array
			int[,] trees = new int[height, width];
			int[,] scenicScore = new int[height, width];
			bool[,] visibleTrees = new bool[height, width];
			for (int i = 0; i < input.Length; i++)
			{
				for (int j = 0; j < input[i].Length; j++)
				{
					trees[i, j] = int.Parse("" + input[i][j]);
				}
			}


			int highestScore = 0;
			// go through trees 
			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					scenicScore[y, x] = FindScenicScoreForTree(x, y, width, height, trees);
					Console.Write(scenicScore[y, x]);
					if (scenicScore[y,x] > highestScore)
					{
						highestScore = scenicScore[y, x];
					}
				}
				Console.Write("\n");
			}

			Console.WriteLine("Best Score: " + highestScore);
		}

		private static int FindScenicScoreForTree(int x_pos, int y_pos, int width, int height, int[,] trees)
		{
			var treeHeight =trees[y_pos, x_pos];

			int scenicScore = 0;
			int runningScore = 0;

			// look up
			for(int y = y_pos-1; y >= 0; y--)
			{
				if(trees[y, x_pos] < treeHeight)
				{
					runningScore++;
				}
				else
				{
					runningScore++;
					break;
				}
			}

			scenicScore += runningScore;
			runningScore = 0;


			// look down
			for (int y = y_pos+1; y < height; y++)
			{
				if (trees[y, x_pos] < treeHeight)
				{
					runningScore++;
				}
				else
				{
					runningScore++;
					break;
				}
			}

			scenicScore *= runningScore;
			runningScore = 0;

			// look left 
			for (int x = x_pos-1; x >= 0; x--)
			{
				if (trees[y_pos, x] < treeHeight)
				{
					runningScore++;
				}
				else
				{
					runningScore++;
					break;
				}
			}

			scenicScore *= runningScore;
			runningScore = 0;

			// look right
			for (int x = x_pos+1; x < width; x++)
			{
				if (trees[y_pos, x] < treeHeight)
				{
					runningScore++;
				}
				else
				{
					runningScore++;
					break;
				}
			}
			scenicScore *= runningScore;

			return scenicScore;
		}
	}
}
