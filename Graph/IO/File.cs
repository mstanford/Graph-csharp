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
	public class File
	{

		public static System.IO.Stream Append(string path)
		{
			if (!System.IO.File.Exists(path))
				return Create(path);
			return Open(path, System.IO.FileMode.Append, System.IO.FileAccess.Write, System.IO.FileShare.None);
		}

		public static void Copy(string source, string target)
		{
			string directory = System.IO.Path.GetDirectoryName(target);
			if (!System.IO.Directory.Exists(directory))
				System.IO.Directory.CreateDirectory(directory);
			System.IO.File.Copy(source, target, true);
		}

		public static System.IO.Stream Create(string path)
		{
			string directory = System.IO.Path.GetDirectoryName(path);
			if (!System.IO.Directory.Exists(directory))
				System.IO.Directory.CreateDirectory(directory);
			return System.IO.File.Create(path);
		}

		public static System.IO.Stream CreateReadWrite(string path)
		{
			return Open(path, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite, System.IO.FileShare.Read);
		}

		public static System.IO.TextWriter CreateText(string path)
		{
			string directory = System.IO.Path.GetDirectoryName(path);
			if (directory.Length > 0 && !System.IO.Directory.Exists(directory))
				System.IO.Directory.CreateDirectory(directory);
			//return new System.IO.StreamWriter(System.IO.File.Create(path), System.Text.Encoding.GetEncoding(437));
			return new System.IO.StreamWriter(System.IO.File.Create(path));
		}

		public static void GZip(string source, string target)
		{
			byte[] bytes = ReadAllBytes(source);
			System.IO.Stream stream = Create(target);
			System.IO.Compression.GZipStream gzipStream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Compress, true);
			gzipStream.Write(bytes, 0, bytes.Length);
			gzipStream.Flush();
			gzipStream.Close();
			stream.Flush();
			stream.Close();
		}

		public static byte[] GZip(string source)
		{
			byte[] bytes = ReadAllBytes(source);
			System.IO.MemoryStream stream = new System.IO.MemoryStream();
			System.IO.Compression.GZipStream gzipStream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Compress, true);
			gzipStream.Write(bytes, 0, bytes.Length);
			gzipStream.Flush();
			gzipStream.Close();
			stream.Flush();
			bytes = stream.ToArray();
			stream.Close();
			return bytes;
		}

		public static void Move(string source, string target)
		{
			string directory = System.IO.Path.GetDirectoryName(target);
			if (!System.IO.Directory.Exists(directory))
				System.IO.Directory.CreateDirectory(directory);
			if (System.IO.File.Exists(target))
				System.IO.File.Delete(target);
			System.IO.File.Move(source, target);
		}

		public static System.IO.FileStream Open(string path, System.IO.FileMode fileMode, System.IO.FileAccess fileAccess, System.IO.FileShare fileShare)
		{
			string directory = System.IO.Path.GetDirectoryName(path);
			if (directory.Length > 0 && !System.IO.Directory.Exists(directory))
				System.IO.Directory.CreateDirectory(directory);
			return System.IO.File.Open(path, fileMode, fileAccess, fileShare);
		}

		public static System.IO.Stream OpenRead(string path)
		{
			return Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
		}

		public static System.IO.Stream OpenReadWrite(string path)
		{
			return Open(path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.Read);
		}

		public static System.IO.TextReader OpenText(string path)
		{
			//return new System.IO.StreamReader(OpenRead(path), System.Text.Encoding.GetEncoding(437));
			return new System.IO.StreamReader(OpenRead(path));
		}

		public static System.IO.Stream OpenWrite(string path)
		{
			return Open(path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.Read);
		}

		public static byte[] ReadAllBytes(string path)
		{
			System.IO.Stream stream = OpenRead(path);
			byte[] bytes = new byte[stream.Length];
			int bytesRead = stream.Read(bytes, 0, bytes.Length);
			stream.Close();
			if (bytesRead != bytes.Length)
				throw new System.Exception();
			return bytes;
		}

		public static void WriteAllBytes(string path, byte[] bytes)
		{
			System.IO.Stream stream = Create(path);
			stream.Write(bytes, 0, bytes.Length);
			stream.Close();
		}

	}
}
