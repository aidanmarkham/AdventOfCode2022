using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class Day17
	{
		public static string inputFile = "tetris.txt";
		public static long rockCount = 1000000000000;

		public static void Solve()
		{
			// a list to represent the board 
			List<string> board = new List<string>();

			// read in the directions as an array of chars
			var directions = File.ReadAllText(inputFile);

			// define shapes of tetris pieces
			#region Piece Definitions
			List<Tetronimo> pieces = new List<Tetronimo>();
			// create tetronimos
			var pipe = new Tetronimo();
			pipe.dimensions = (4, 1);
			pipe.shape = new char[][]{
				new char[]{'#', '#', '#', '#'}
			};
			pieces.Add(pipe);

			var plus = new Tetronimo();
			plus.dimensions = (3, 3);
			plus.shape = new char[][]{
				new char[]{' ', '#', ' '},
				new char[]{'#', '#', '#'},
				new char[]{' ', '#', ' '}
			};
			pieces.Add(plus);

			var L = new Tetronimo();
			L.dimensions = (3, 3);
			L.shape = new char[][]{
				new char[]{'#', '#', '#'},
				new char[]{' ', ' ', '#'},
				new char[]{' ', ' ', '#'}
			};
			pieces.Add(L);

			var line = new Tetronimo();
			line.dimensions = (1, 4);
			line.shape = new char[][]{
				new char[]{'#'},
				new char[]{'#'},
				new char[]{'#'},
				new char[]{'#'}
			};
			pieces.Add(line);

			var box = new Tetronimo();
			box.dimensions = (2, 2);
			box.shape = new char[][]{
				new char[]{'#', '#'},
				new char[]{'#', '#'}
			};
			pieces.Add(box);
			#endregion

			// how many rocks left until we're done 
			long remainingRocks = rockCount;

			// the highest point so far 
			long highestPoint = 0;

			// loop iterator to determine what piece comes next 
			int pieceIndex = 0;
			// loop iterator to determine what direction comes next 
			int directionsIndex = 0;

			// how many steps does it take for both cycles to reset at the same time 
			int lcm = directions.Length * pieces.Count;

			// keep track of how many units of board we've trimmed off the bottom 
			long boardOffset = 0;

			List<string> boards = new List<string>();

			long firstMatch = -1;
			long lastMatch = 0;
			long lastDist = -1;
			long lastHeight = -1;
			bool skipped = false;
			long finalOffset = 0;
			while (remainingRocks > 0)
			{
				// progress report 
				long soFar = rockCount - remainingRocks;
				long currentHeight = GetTowerHeight(board, boardOffset);
				if (!skipped)
				{
					var boardString = OutputBoard(board, pieceIndex, directionsIndex);
					if (boards.Contains(boardString))
					{
						if (firstMatch == -1)
						{
							firstMatch = currentHeight;
						}
						else
						{
							long rockDist = soFar - lastMatch;
							long heightDist = currentHeight - lastHeight;

							if (rockDist == lastDist)
							{
								skipped = true;
								Console.WriteLine("Pattern found! Loop of " + heightDist + " rows happens starting after " + firstMatch + " rows. Rocks Contained: " + rockDist);

								long numSkips = remainingRocks / rockDist;

								long rockSkip = rockDist * numSkips;
								long heightSkip = heightDist * numSkips;

								Console.WriteLine("Skipping " + numSkips + " loops. " + rockDist + " rocks and " + heightDist + " rows");
								remainingRocks -= rockSkip;
								finalOffset += heightSkip;
							}

							lastMatch = soFar;
							lastDist = rockDist;
							lastHeight = currentHeight;
						}
						boards.Clear();
					}
					boards.Add(boardString);
				}


				// =============== spawn rock ====================
				// calculate rock drop position
				(long x, long y) rockPos = (2, highestPoint + 3);

				// figure out what piece we need and select it 
				if (pieceIndex == pieces.Count) pieceIndex = 0;
				var piece = pieces[pieceIndex];
				pieceIndex++;

				// place it at the rock position 
				piece.position = rockPos;

				// make sure the board is tall enough 
				while (rockPos.y + piece.dimensions.y > board.Count + boardOffset)
				{
					AddLines(board, 1);
				}

				// is the piece obstructed by something below it?
				bool settled = false;

				// how many steps have we fallen for (so we can know if we should move sideways or down)
				int fallStepCount = 0;

				//=========== drop rock ================
				// while the current piece isn't settled 
				while (!settled)
				{
					// do we move sideways, or up and down?
					bool sideways = fallStepCount % 2 == 0;
					fallStepCount += 1;

					// figure out what our next position will be 
					var nextPos = piece.position;

					// if we're moving sideways 
					if (sideways)
					{
						// get a direction 
						if (directionsIndex == directions.Length) directionsIndex = 0;
						char direction = directions[directionsIndex];
						directionsIndex++;

						// offset position by the direction
						if (direction == '<')
						{
							nextPos.x--;
						}
						else if (direction == '>')
						{
							nextPos.x++;
						}
						else
						{
							// bad data control
							Console.WriteLine("Huh???");
						}
					}
					// otherwise
					else
					{
						// just move down
						nextPos.y--;
					}

					// if the piece is outside the dimensions
					if (nextPos.y < 0 ||
						nextPos.y - boardOffset < 0 ||
						nextPos.x < 0 ||
						nextPos.x + piece.dimensions.x > 7)
					{
						// don't make that move
						nextPos = piece.position;

						// if the move was going to be moving down 
						if (!sideways)
						{
							// we're settled 
							settled = true;

							// imprint the piece on the board 
							PrintPiece(board, piece, boardOffset);
						}
					}
					else
					{
						// otherwise, loop through all the lines the piece is on 
						for (int i = 0; i < piece.dimensions.y; i++)
						{
							// get that line from the board
							var boardLine = board[(int)(nextPos.y + i - boardOffset)];

							// get that line from the piece
							var pieceLine = piece.GetLine(nextPos.y + i, nextPos);

							// go through the lines
							for (int j = 0; j < boardLine.Length; j++)
							{
								// if there's an intersection
								if (boardLine[j] != ' ' && pieceLine[j] != ' ')
								{
									// don't make that move
									nextPos = piece.position;

									//if it was a downward move
									if (!sideways)
									{
										// settle the piece
										settled = true;

										// and print it on the board 
										PrintPiece(board, piece, boardOffset);
									}
								}
							}
						}
					}

					// if we've made it this far, the piece isn't hitting anything yet, 
					// so set it's position to the new position
					piece.position = nextPos;
				}

				// now that we've got the new piece settled, keep track of the highest point on the tower 
				highestPoint = GetTowerHeight(board, boardOffset);

				// and decrement remaining rocks 
				remainingRocks--;
			}

			// at the end
			// draw a picture of the board (or at least what hasn't been trimmed)
			//DrawBoard(board, boardOffset);

			// output the tower height
			Console.WriteLine("Tower Height: " + GetTowerHeight(board, boardOffset) + " Final Offset: " + finalOffset);
			Console.WriteLine("Final Height: " + (GetTowerHeight(board, boardOffset) + finalOffset));
		}

		private static int GetLowestTetris(List<string> board)
		{
			for (int i = 0; i < board.Count; i++)
			{
				if (board[i] == "#######")
				{
					return i;
				}
			}

			return -1;
		}

		private static int GetHighestTetris(List<string> board)
		{
			for (int i = board.Count - 1; i >= 0; i--)
			{
				if (board[i] == "#######")
				{
					return i;
				}
			}

			return -1;
		}

		// go down the board starting from the top until we find blocks
		private static long GetTowerHeight(List<string> board, long offset)
		{
			for (int i = board.Count - 1; i >= 0; i--)
			{
				if (board[i] != "       ") return i + 1 + offset;
			}
			return offset;
		}

		// add some blank lines to the top of the board 
		private static void AddLines(List<string> board, int quantity)
		{
			for (int i = 0; i < quantity; i++)
			{
				board.Add("       ");
			}
		}

		// print out the board in a human readable format
		private static void DrawBoard(List<string> board, long offset)
		{
			for (int i = board.Count - 1; i >= 0; i--)
			{
				Console.WriteLine((offset + i).ToString().PadLeft(5, '0') + "|" + board[i] + "|");
			}
			Console.WriteLine((-1).ToString().PadLeft(5, '0') + "+-------+");
		}

		// output the current board as a string for comparison 
		private static string OutputBoard(List<string> board, int pieceIndex, int directionIndex)
		{
			var boardString = pieceIndex + " " + directionIndex + "\n";
			int lowerBound = Math.Max(0, board.Count - 10);
			for (int i = board.Count - 1; i >= lowerBound; i--)
			{
				boardString += board[i] + "\n";
			}

			return boardString;
		}

		// print the piece on the board in it's position 
		private static void PrintPiece(List<string> board, Tetronimo piece, long offset)
		{
			for (int i = board.Count - 1; i >= 0; i--)
			{
				if (i < piece.position.y - 1 - offset) return;

				var pieceString = piece.GetLine(offset + i, piece.position);
				if (pieceString.Length == 0) pieceString = board[i];

				for (int j = 0; j < board[i].Length; j++)
				{
					if (pieceString[j] != ' ') board[i] = board[i].Remove(j, 1).Insert(j, pieceString[j].ToString());
				}
			}
		}


		// data class representing a tetronimo
		public class Tetronimo
		{
			public (long x, long y) dimensions;
			public char[][] shape;
			public (long x, long y) position;

			public string GetLine(long line, (long x, long y) pos)
			{
				if (line < pos.y) return "";
				if (line > pos.y + dimensions.y - 1) return "";

				var outString = new string(shape[line - pos.y]);

				for (int i = 0; i < pos.x; i++)
				{
					outString = " " + outString;
				}

				while (outString.Length < 7)
				{
					outString += " ";
				}

				return outString;
			}
		}
	}
}


