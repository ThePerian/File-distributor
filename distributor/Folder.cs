
using System;
using System.Collections.Generic;
using System.IO;

namespace distributor
{
	public class Folder : IComparable<Folder>
	{
		public List<FileInfo> files;
		public long size;
		public string name;
		public string path;
		
		public Folder(string path)
		{
			this.path = path;
			this.name = new DirectoryInfo(path).Name;
			this.size = 0;
			this.files = new List<FileInfo>();
			foreach (string fileName in Directory.GetFiles(path)) 
			{
				FileInfo newFile = new FileInfo(fileName);
				this.files.Add(newFile);
				this.size += newFile.Length;
			}
		}
		
		public Folder(Folder folder)
		{
			this.path = folder.path;
			this.name = new DirectoryInfo(path).Name;
			this.files = new List<FileInfo>(folder.files);
			this.size = 0;
			foreach (FileInfo file in this.files) 
			{
				this.size += file.Length;
			}
		}
		
		public int CompareTo(Folder folder)
		{
			if (this.size > folder.size) {return -1;}
			else if (this.size < folder.size) {return 1;}
			else {return 0;}
		}
		
		public void Remove(FileInfo fileToDelete)
		{
			foreach (FileInfo file in this.files)
			{
				if (file.FullName == fileToDelete.FullName)
				{
					this.files.Remove(file);
					return;
				}
			}
		}
		
		public void Remove(string fileToDelete)
		{
			foreach (FileInfo file in this.files)
			{
				if (file.FullName == fileToDelete)
				{
					this.files.Remove(file);
					return;
				}
			}
		}
		
		public void Update(bool updateName)
		{
			this.size = 0;
			foreach (FileInfo file in this.files) 
			{
				this.size += file.Length;
			}
			if (updateName)
			{
				this.name = new DirectoryInfo(this.path).Name + " (";
				foreach (FileInfo file in this.files)
				{
					this.name += file.Extension.Trim('.') + ",";
				}
				this.name = this.name.Trim(',');
				this.name += ")";
			}
		}
	}
}
