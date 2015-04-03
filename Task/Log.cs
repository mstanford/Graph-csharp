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
using System.Text;

namespace Task
{
	public class Log : System.IO.TextWriter
	{
		private readonly System.IO.TextWriter _writer;
		private readonly DateTime _startTime;

		public Log(System.IO.TextWriter writer)
		{
			_writer = writer;
			_startTime = DateTime.UtcNow;
		}

		public int Indentation;

		public override Encoding Encoding
		{
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override void Write(object value)
		{
			if (value is System.Exception)
			{
				System.Exception exception = (System.Exception)value;

				while (exception.InnerException != null)
					exception = exception.InnerException;

				WriteLine("");
				WriteLine("ERROR");

				_writer.WriteLine("");
				_writer.WriteLine(exception.Message);
				_writer.WriteLine("");
				_writer.WriteLine(exception.StackTrace);
			}
			else
			{
				base.Write(value);
			}
		}

		public override void WriteLine(string value)
		{
			_writer.Write(new DateTime(DateTime.UtcNow.Subtract(_startTime).Ticks).ToString("HH:mm:ss") + " ");
			for (int i = 0; i < Indentation; i++)
				_writer.Write("   ");
			_writer.WriteLine(value);
		}

	}
}
