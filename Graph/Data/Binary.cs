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

namespace Graph.Data
{
	public class Binary
	{

		public static void WriteDouble(double d, System.IO.Stream stream)
		{
			byte[] buffer = BitConverter.GetBytes(d);
			stream.Write(buffer, 0, buffer.Length);
		}

		public static void WriteInt32(int n, System.IO.Stream stream)
		{
			byte[] buffer = BitConverter.GetBytes(n);
			stream.Write(buffer, 0, buffer.Length);
		}

		public static void WriteInt64(long l, System.IO.Stream stream)
		{
			byte[] buffer = BitConverter.GetBytes(l);
			stream.Write(buffer, 0, buffer.Length);
		}

		public static void WriteFixedString(string s, int length, System.IO.Stream stream)
		{
			byte[] buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(s);
			if (buffer.Length != length)
				throw new System.Exception();
			stream.Write(buffer, 0, length);
		}

		public static void WriteString(string s, System.IO.Stream stream)
		{
			byte[] buffer = System.Text.ASCIIEncoding.ASCII.GetBytes(s);
			WriteInt32(buffer.Length, stream);
			stream.Write(buffer, 0, buffer.Length);
		}

		public static double ReadDouble(System.IO.Stream stream, byte[] buffer)
		{
			stream.Read(buffer, 0, 8);
			return BitConverter.ToDouble(buffer, 0);
		}

		public static int ReadInt32(System.IO.Stream stream, byte[] buffer)
		{
			stream.Read(buffer, 0, 4);
			return BitConverter.ToInt32(buffer, 0);
		}

		public static long ReadInt64(System.IO.Stream stream, byte[] buffer)
		{
			stream.Read(buffer, 0, 8);
			return BitConverter.ToInt64(buffer, 0);
		}

		public static string ReadFixedString(System.IO.Stream stream, int length)
		{
			byte[] buffer = new byte[length];
			stream.Read(buffer, 0, buffer.Length);
			return System.Text.ASCIIEncoding.ASCII.GetString(buffer);
		}

		public static string ReadString(System.IO.Stream stream)
		{
			int length = ReadInt32(stream, new byte[4]);
			byte[] buffer = new byte[length];
			stream.Read(buffer, 0, buffer.Length);
			return System.Text.ASCIIEncoding.ASCII.GetString(buffer);
		}

	}
}
