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
		public static string inputFile = "tetris_test.txt";
		public static long rockCount = 1000000000000;

		public static void Solve()
		{
			List<string> board = new List<string>();
			var directions = File.ReadAllText(inputFile);
			int directionsIndex = 0;
			List<Tetronimo> pieces = new List<Tetronimo>();
			int lcm = directions.Length * 5;
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

			long remainingRocks = rockCount;
			long highestPoint = 0;
			int pieceIndex = 0;
			long boardOffset = 0;
			Stopwatch watch = new Stopwatch();
			watch.Start();

			List<string> boards = new List<string>();
			int lastLoopDex = -1;
			int lastOffset = -1;
			while (remainingRocks != 0)
			{
				long soFar = rockCount - remainingRocks;

				if (remainingRocks % 10000 == 0)
				{
					double rocksSoFar = rockCount - remainingRocks;
					double progressScalar = rocksSoFar / (double)rockCount;
					var ticksSoFar = watch.ElapsedTicks;
					var time = new TimeSpan((long)(ticksSoFar / progressScalar));
					Console.WriteLine("Highest Tetris: " + GetHighestTetris(board));
					Console.WriteLine("Only " + remainingRocks + " left! Time remaining: " + time.ToString("h'h 'm'm 's's'"));
				}


				// =============== spawn rock ====================
				(long x, long y) rockPos = (2, highestPoint + 3);

				if (pieceIndex == pieces.Count) pieceIndex = 0;
				var piece = pieces[pieceIndex];
				pieceIndex++;

				piece.position = rockPos;

				// make sure the board is tall enough 
				while (rockPos.y + piece.dimensions.y > board.Count + boardOffset)
				{
					AddLines(board, 1);
				}

				bool obstructed = false;
				int fallStepCount = 0;

				// search for loops 
				int loopDex = TrimBoard(board, ref boardOffset, lcm, ref boards, (int)soFar);
				if (loopDex != -1)
				{
					var offset = loopDex - lastLoopDex;
					lastLoopDex = loopDex;
					if(offset == lastOffset)
					{
						Console.WriteLine("Locked on to loop!");
						while (remainingRocks > offset)
						{
							remainingRocks -= offset;
							boardOffset += offset;
						}
					}
					lastOffset = offset;
				}

				while (!obstructed)
				{
					var nextPos = piece.position;
					bool sideways = fallStepCount % 2 == 0;
					fallStepCount += 1;
					if (sideways)
					{
						if (directionsIndex == directions.Length) directionsIndex = 0;
						char direction = directions[directionsIndex];
						directionsIndex++;

						//Console.WriteLine("Pushing " + direction);

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
							Console.WriteLine("Huh???");
						}
					}
					else
					{
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
						if (!sideways)
						{
							obstructed = true;
							PrintPiece(board, piece, boardOffset);
						}
					}
					else
					{
						for (int i = 0; i < piece.dimensions.y; i++)
						{
							var boardLine = board[(int)(nextPos.y + i - boardOffset)];
							var pieceLine = piece.GetLine(nextPos.y + i, nextPos);

							for (int j = 0; j < boardLine.Length; j++)
							{
								if (boardLine[j] != ' ' && pieceLine[j] != ' ')
								{
									// don't make that move
									nextPos = piece.position;
									if (!sideways)
									{
										obstructed = true;
										PrintPiece(board, piece, boardOffset);
									}
								}
							}
						}
					}
					piece.position = nextPos;

				}

				highestPoint = GetTowerHeight(board, boardOffset);

				remainingRocks--;
			}

			DrawBoard(board, boardOffset);
			Console.WriteLine("Tower Height: " + GetTowerHeight(board, boardOffset));
		}

		private static int TrimBoard(List<string> board, ref long offset, int lcm, ref List<string> boards, int soFar)
		{
			int index = GetHighestTetris(board);
			if (index != -1)
			{
				if (index > lcm)
				{
					board.RemoveRange(0, index + 1);
					offset += index + 1;
					AddLines(board, 1);

					var boardString = OutputBoard(board, offset);
					Console.WriteLine(boardString);
					return soFar;
				}
			}

			return -1;
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

		private static long GetTowerHeight(List<string> board, long offset)
		{
			for (int i = board.Count - 1; i >= 0; i--)
			{
				if (board[i] != "       ") return i + 1 + offset;
			}
			return offset;
		}

		private static void AddLines(List<string> board, int quantity)
		{
			for (int i = 0; i < quantity; i++)
			{
				board.Add("       ");
			}
		}

		private static void DrawBoard(List<string> board, Tetronimo piece, long offset)
		{
			for (int i = board.Count - 1; i >= 0; i--)
			{
				var lineString = piece.GetLine(offset + i, piece.position);
				if (lineString.Length == 0) lineString = new string(board[i].ToArray());
				Console.WriteLine((offset + i).ToString().PadLeft(5, '0') + "|" + lineString + "|");
			}
			Console.WriteLine((-1).ToString().PadLeft(5, '0') + "+-------+");
		}
		private static void DrawBoard(List<string> board, long offset)
		{
			for (int i = board.Count - 1; i >= 0; i--)
			{
				Console.WriteLine((offset + i).ToString().PadLeft(5, '0') + "|" + board[i] + "|");
			}
			Console.WriteLine((-1).ToString().PadLeft(5, '0') + "+-------+");
		}
		private static string OutputBoard(List<string> board, long offset)
		{
			var boardString = "";
			for (int i = board.Count - 1; i >= 0; i--)
			{
				boardString += board[i] + "\n";
			}

			return boardString;
		}

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


