
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace distributor
{
	class Program
	{
		public static void Main(string[] args)
		{
			long sizeOfDisc;
			Console.WriteLine("Enter disc size (MB):");
			sizeOfDisc = Convert.ToInt64(Console.ReadLine()) * 1024 * 1024; //MB to B
			
			string targetDirectory;
			while (true)
			{
				Console.WriteLine("Enter path to target directory:");
				targetDirectory = Console.ReadLine();
				if (Directory.Exists(targetDirectory))
				{
					break;
				}
				else
				{
					Console.WriteLine("Enter correct path!");
				}
			}
			
			Console.WriteLine("Getting list of directories (may take a minute)...");
			List<Folder> folderList = new List<Folder>();
			foreach (string folderPath in Directory.GetDirectories(targetDirectory))
			{
				Folder newFolder = new Folder(folderPath);
				folderList.Add(newFolder);
			}
			folderList.Sort();
			
			Console.WriteLine("Calculating size of directories...");
			List<List<Folder>> discList = new List<List<Folder>>();
			List<Folder> biggestFolders = new List<Folder>();
			List<Folder> backupFolderList = new List<Folder>(folderList);
			foreach (Folder currentFolder in folderList)
			{
				if (currentFolder.size >= sizeOfDisc)
				{
					bool hasBigFile = false;
					foreach (FileInfo file in currentFolder.files)
					{
						if (file.Length >= sizeOfDisc)
						{
							Console.WriteLine("Size of file "
							                 + file.Name
							                + " is greater than size of disc!");
							backupFolderList.Remove(currentFolder);
							hasBigFile = true;
						}
					}
					if (hasBigFile) continue;
					biggestFolders.Add(currentFolder);
					backupFolderList.Remove(currentFolder);
				}
			}
			
			Console.WriteLine("Splitting directories that are too big to fit on disc...");
			foreach (Folder currentFolder in biggestFolders)
			{
				while (currentFolder.size >= sizeOfDisc)
				{
					Folder newFolder = new Folder(currentFolder);
					long sizeOfNewFolder = 0;
					foreach (FileInfo file in currentFolder.files)
					{
						if (sizeOfNewFolder + file.Length >= sizeOfDisc)
						{
							newFolder.Remove(file);
						}
						else
						{
							sizeOfNewFolder += file.Length;
						}
					}
					foreach (FileInfo file in newFolder.files)
					{
						currentFolder.Remove(file);
					}
					newFolder.Update(true);
					currentFolder.Update(true);
					backupFolderList.Add(newFolder);
				}
				backupFolderList.Add(currentFolder);
			}
			backupFolderList.Sort();
			folderList = new List<Folder>(backupFolderList);
			
			Console.WriteLine("Distributing directories...");
			while (folderList.Any<Folder>())
			{
				List<Folder> currentDisc = new List<Folder>();
				long sizeOfCurrentDisc = 0;
				foreach(Folder folder in folderList)
				{
					if (folder.size + sizeOfCurrentDisc < sizeOfDisc)
					{
						currentDisc.Add(folder);
						sizeOfCurrentDisc += folder.size;
						backupFolderList.Remove(folder);
					}
				}
				if (sizeOfCurrentDisc > 0) 
				{
					discList.Add(currentDisc);
				}
				folderList = new List<Folder>(backupFolderList);
			}
			
			Console.WriteLine("Extracting list to files.txt...");
			foreach (List<Folder> currentDisc in discList)
			{
				foreach (Folder folder in currentDisc)
				{
					File.AppendAllText(Environment.CurrentDirectory + "\\files.txt", folder.name + Environment.NewLine);
				}
				File.AppendAllText(Environment.CurrentDirectory + "\\files.txt", Environment.NewLine);
			}
			
			Console.Write("Done. Press any key to continue . . . ");
			Console.ReadKey();
		}
	}
}