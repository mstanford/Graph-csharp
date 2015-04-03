// ------------------------------------------------------------------------
// 
// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org/>
// 
// ------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Graph.IO
{
	public class Directory
	{

		public static void Copy(string source, string target)
		{
			foreach (string directory in System.IO.Directory.GetDirectories(source))
				Copy(directory, System.IO.Path.Combine(target, System.IO.Path.GetFileName(directory)));
			foreach (string file in System.IO.Directory.GetFiles(source))
				File.Copy(file, System.IO.Path.Combine(target, System.IO.Path.GetFileName(file)));
		}

		public static void CopyIfNotExists(string source, string target)
		{
			foreach (string directory in System.IO.Directory.GetDirectories(source))
				Copy(directory, System.IO.Path.Combine(target, System.IO.Path.GetFileName(directory)));
			foreach (string file in System.IO.Directory.GetFiles(source))
				//TODO Copy if their sha1s don't match.
				if (!System.IO.File.Exists(System.IO.Path.Combine(target, System.IO.Path.GetFileName(file))))
					File.Copy(file, System.IO.Path.Combine(target, System.IO.Path.GetFileName(file)));
		}

		public static void CreateDirectories(IEnumerable<string> paths)
		{
			foreach (string path in paths)
			{
				if (!System.IO.Directory.Exists(path))
					Directory.CreateDirectory(path);
			}
		}

		public static void CreateDirectory(string path)
		{
			string directory = System.IO.Path.GetDirectoryName(path);
			if (!System.IO.Directory.Exists(directory))
				CreateDirectory(directory);
			System.IO.Directory.CreateDirectory(path);
		}

		public static void Delete(string path)
		{
			for (int i = 0; i < 10; i++)
			{
				if (System.IO.Directory.Exists(path))
					System.IO.Directory.Delete(path, true);
				else
					return;
				System.Threading.Thread.Sleep(1000);
			}
		}

		public static void DeleteIfEmpty(string path)
		{
			if (System.IO.Directory.GetDirectories(path).Length > 0 || System.IO.Directory.GetFiles(path).Length > 0)
				throw new System.Exception("Directory not empty.");
			System.IO.Directory.Delete(path);
		}

		public static IEnumerable<string> GetAllFiles(string directory)
		{
			switch (System.IO.Path.GetFileName(directory))
			{
				case "$RECYCLE.BIN":
				case "MSOCache":
				case "System Volume Information":
					yield break;
			}
			foreach (string directory2 in System.IO.Directory.GetDirectories(directory))
				foreach (string file in GetAllFiles(directory2))
					yield return file;
			foreach (string file in System.IO.Directory.GetFiles(directory))
				yield return file;
		}

		public static IEnumerable<string> GetEmptyDirectories(string directory)
		{
			switch (System.IO.Path.GetFileName(directory))
			{
				case "$RECYCLE.BIN":
				case "MSOCache":
				case "System Volume Information":
					yield break;
			}
			if (System.IO.Directory.GetDirectories(directory).Length == 0)
			{
				if (System.IO.Directory.GetFiles(directory).Length == 0)
					yield return directory;
			}
			else
			{
				foreach (string directory2 in System.IO.Directory.GetDirectories(directory))
					foreach (string directory3 in GetEmptyDirectories(directory2))
						yield return directory3;
			}
		}

		public static string[] GetSortedDirectories(string path)
		{
			string[] directories = System.IO.Directory.GetDirectories(path);
			System.Array.Sort(directories);
			return directories;
		}

		public static void Move(string source, string target)
		{
			foreach (string directory in System.IO.Directory.GetDirectories(source))
				Move(directory, System.IO.Path.Combine(target, System.IO.Path.GetFileName(directory)));
			foreach (string file in System.IO.Directory.GetFiles(source))
				File.Move(file, System.IO.Path.Combine(target, System.IO.Path.GetFileName(file)));
			System.IO.Directory.Delete(source);
		}

	}
}
