using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	class FileSystem
	{
		public static string inputFile = "cmd.txt";

		public static void DiskCheck()
		{
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);

			Console.WriteLine("Input file loaded! Calculating...");

			Directory root = null;
			Directory current = null;

			// build directory
			for (int i = 0; i < input.Length; i++)
			{
				var line = input[i].Split(' ');
				//Console.WriteLine(input[i]);
				if (line[0] == "$")
				{
					switch (line[1])
					{
						case "cd":
							if (line[2] == "..")
							{

								// go up a level
								if (current.Parent != null)
								{
									//Console.WriteLine("Going up a level to " + current.Parent.Name);
									current = current.Parent;
								}
							}
							else
							{
								// if this is the first time
								if (current == null)
								{
									// set it into current
									current = new Directory(null, line[2]);
									root = current;
								}
								// otherwise we're in a directory
								else
								{
									bool found = false;
									for (int f = 0; f < current.Children.Count; f++)
									{
										if (current.Children[f].Name == line[2])
										{
											// we already have this file and should go into it 
											found = true;
											//Console.WriteLine("Found Directory: " + line[2]);
											current = current.Children[f];
											break;
										}
									}

									if (!found) // create
									{
										// create a new directory
										var newDir = new Directory(current, line[2]);

										// add it to the children
										current.AddChild(newDir);

										// navigate to it 
										current = newDir;

										//Console.WriteLine("Created Directory: " + current.Name);
									}
								}
							}
							break;
						case "ls":
							// the next lines until we get to an dollar sign are files or contained directories

							// advance one line 
							i++;


							while (i < input.Length && input[i].Split(' ')[0] != "$")
							{
								//Console.WriteLine(input[i]);
								line = input[i].Split(' ');
								var arg0 = line[0];
								var arg1 = line[1];

								// this is a directory
								if (arg0 == "dir")
								{
									//Console.WriteLine("Added Directory: " + arg1);
									var newDir = new Directory(current, arg1);
									current.AddChild(newDir);
								}
								// this is a file
								else
								{
									//Console.WriteLine("Added File: " + arg1);
									var newFile = new DirectoryFile(arg1, int.Parse(arg0));
									current.AddFile(newFile);
								}

								i++;
							}
							i--;

							break;
					}

				}
			}

			var allDirectories = new List<Directory>();
			root.PopulateList(allDirectories);

			int total = 0;

			for (int i = 0; i < allDirectories.Count; i++)
			{
				if (allDirectories[i].GetSize() < 100000)
				{
					total += allDirectories[i].GetSize();
				}
			}

			Console.WriteLine("Total: " + total);
		}

		public static void SpaceFinder()
		{
			Console.WriteLine("Reading input file.");

			string[] input = File.ReadAllLines(inputFile);

			Console.WriteLine("Input file loaded! Calculating...");

			Directory root = null;
			Directory current = null;

			// build directory
			for (int i = 0; i < input.Length; i++)
			{
				var line = input[i].Split(' ');
				//Console.WriteLine(input[i]);
				if (line[0] == "$")
				{
					switch (line[1])
					{
						case "cd":
							if (line[2] == "..")
							{

								// go up a level
								if (current.Parent != null)
								{
									//Console.WriteLine("Going up a level to " + current.Parent.Name);
									current = current.Parent;
								}
							}
							else
							{
								// if this is the first time
								if (current == null)
								{
									// set it into current
									current = new Directory(null, line[2]);
									root = current;
								}
								// otherwise we're in a directory
								else
								{
									bool found = false;
									for (int f = 0; f < current.Children.Count; f++)
									{
										if (current.Children[f].Name == line[2])
										{
											// we already have this file and should go into it 
											found = true;
											//Console.WriteLine("Found Directory: " + line[2]);
											current = current.Children[f];
											break;
										}
									}

									if (!found) // create
									{
										// create a new directory
										var newDir = new Directory(current, line[2]);

										// add it to the children
										current.AddChild(newDir);

										// navigate to it 
										current = newDir;

										//Console.WriteLine("Created Directory: " + current.Name);
									}
								}
							}
							break;
						case "ls":
							// the next lines until we get to an dollar sign are files or contained directories

							// advance one line 
							i++;


							while (i < input.Length && input[i].Split(' ')[0] != "$")
							{
								//Console.WriteLine(input[i]);
								line = input[i].Split(' ');
								var arg0 = line[0];
								var arg1 = line[1];

								// this is a directory
								if (arg0 == "dir")
								{
									//Console.WriteLine("Added Directory: " + arg1);
									var newDir = new Directory(current, arg1);
									current.AddChild(newDir);
								}
								// this is a file
								else
								{
									//Console.WriteLine("Added File: " + arg1);
									var newFile = new DirectoryFile(arg1, int.Parse(arg0));
									current.AddFile(newFile);
								}

								i++;
							}
							i--;

							break;
					}

				}
			}

			var allDirectories = new List<Directory>();
			root.PopulateList(allDirectories);

			int total = 0;

			int totalSpace = 70000000;
			int usedSpace = root.GetSize();
			int requiredSpace = 30000000;
			int remaining = totalSpace - usedSpace;
			int mustClear = requiredSpace - remaining;

			Console.WriteLine("must clear: " + mustClear);

			Directory toDelete = null;

			for (int i = 0; i < allDirectories.Count; i++)
			{
				var size = allDirectories[i].GetSize();
				if (size > mustClear)
				{
					if(toDelete == null)
					{
						toDelete = allDirectories[i];
					}
					else if(size < toDelete.GetSize())
					{
						toDelete = allDirectories[i];
					}
				}
			}

			Console.WriteLine("To Delete: " + toDelete.Name + " Size: " + toDelete.GetSize());
		}
	}

}

class Directory
{
	public Directory Parent;
	public List<Directory> Children;
	public List<DirectoryFile> Files;
	public string Name;

	public Directory(Directory p, string n)
	{
		Parent = p;
		Children = new List<Directory>();
		Files = new List<DirectoryFile>();
		Name = n;
	}

	public void AddChild(Directory child)
	{
		Children.Add(child);
	}

	public void AddFile(DirectoryFile file)
	{
		Files.Add(file);
	}

	public void Print(int indentLevel)
	{
		string indent = "";

		for (int i = 0; i < indentLevel; i++)
		{
			indent += "  ";
		}

		Console.WriteLine(" - " + indent + Name + " (dir, size = " + GetSize() + ")");
		for (int i = 0; i < Children.Count; i++)
		{
			Children[i].Print(indentLevel + 1);
		}
		for (int i = 0; i < Files.Count; i++)
		{
			Console.WriteLine(indent + " - " + indent + Files[i].name + " (file, size = " + Files[i].size + ")");
		}
	}

	public int GetSize()
	{
		int totalSize = 0;

		for (int i = 0; i < Children.Count; i++)
		{
			totalSize += Children[i].GetSize();
		}
		for (int i = 0; i < Files.Count; i++)
		{
			totalSize += Files[i].size;
		}
		return totalSize;
	}

	public void PopulateList(List<Directory> ld)
	{
		ld.Add(this);
		for (int i = 0; i < Children.Count; i++)
		{
			Children[i].PopulateList(ld);
		}
	}
}
class DirectoryFile
{
	public string name;
	public int size;

	public DirectoryFile(string n, int s)
	{
		name = n;
		size = s;
	}
}
