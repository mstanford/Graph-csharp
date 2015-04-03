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
	public class Column : Message
	{
		private readonly System.IO.Stream _columnDataStream;
		private readonly byte[] _columnBuffer;
		private readonly int _columnLength;
		private readonly long _position;
		private int _protectionLimit;

		public Column(double[] values)
			: base(null, new Tag[0])
		{
			Values = values;
			_protectionLimit = Values.Length;
		}

		public Column(string query, Tag[] tags, Dictionary<string, Dictionary<string, string>> columnSchemasByType, System.IO.Stream columnDataStream, byte[] columnBuffer, int columnLength, long position)
			: base(query, tags)
		{
			if (!IsDefined("ColumnType"))
				throw new System.Exception();

			string type = this["RowType"];
			if (!columnSchemasByType.ContainsKey(type))
			{
				Dictionary<string, string> columnSchema = new Dictionary<string, string>();
				foreach (Tag tag in tags)
					columnSchema.Add(tag.Name, tag.Name);
				columnSchemasByType.Add(type, columnSchema);
			}
			else
			{
				Dictionary<string, string> columnSchema = columnSchemasByType[type];
				if (tags.Length != columnSchema.Count)
					throw new System.Exception("Inconsistent schema.");
				foreach (Tag tag in tags)
					if (!columnSchema.ContainsKey(tag.Name))
						throw new System.Exception("Inconsistent schema.");
			}

			if (BufferLength(columnLength) != columnBuffer.Length)
				throw new System.Exception();
			_columnDataStream = columnDataStream;
			_columnBuffer = columnBuffer;
			_columnLength = columnLength;
			_position = position;
		}

		public int Length { get { return Values.Length; } }
		public bool Loaded { get { return Values != null; } }
		public bool Modified = false;
		public double[] Values;

		public double this[int index]
		{
			get { if (_protectionLimit == Values.Length || index > _protectionLimit) throw new System.Exception(); return Values[index]; }
			set { if (_protectionLimit == Values.Length || index > _protectionLimit) throw new System.Exception(); if (double.IsInfinity(value) || double.IsNegativeInfinity(value)) throw new System.Exception(); Values[index] = value; Modified = true; }
		}

		public void Protect(int protectionLimit)
		{
			_protectionLimit = protectionLimit;
		}

		public void Load()
		{
			_columnDataStream.Position = _position;

			int bytesRead = 0;
			while (bytesRead < _columnBuffer.Length)
				bytesRead += _columnDataStream.Read(_columnBuffer, 0, _columnBuffer.Length);

			int index = 0;

			// Length
			int length = BitConverter.ToInt32(_columnBuffer, index);
			if (length != (_columnLength * 8))
				throw new System.Exception("Invalid length.");
			index += 4;

			// Values
			if (Values != null)
				throw new System.Exception();
			Values = (Graph.Functions.Function.ColumnPool.Count(_columnLength) == 0) ? new double[_columnLength] : Graph.Functions.Function.ColumnPool.Pop(_columnLength);

			_protectionLimit = Values.Length;
			for (int i = 0; i < Values.Length; i++)
			{
				Values[i] = BitConverter.ToDouble(_columnBuffer, index);
				index += 8;
			}

			// CheckSum
			int checkSum = 0;
			for (int i = 0; i < (_columnLength * 8); i++)
				checkSum += _columnBuffer[i + 4];
			if ((byte)(checkSum % 256) != _columnBuffer[index])
				throw new System.Exception("Invalid CheckSum.");
			index += 1;
		}

		public void Initialize()
		{
			if (Values != null)
				throw new System.Exception();
			Values = (Graph.Functions.Function.ColumnPool.Count(_columnLength) == 0) ? new double[_columnLength] : Graph.Functions.Function.ColumnPool.Pop(_columnLength);

			_protectionLimit = Values.Length;
			for (int i = 0; i < Values.Length; i++)
				Values[i] = double.NaN;

			Save();
		}

		public void Save()
		{
			int index = 0;

			System.Array.Clear(_columnBuffer, 0, _columnBuffer.Length);

			// Length
			byte[] buffer = BitConverter.GetBytes(Values.Length * 8);
			System.Array.Copy(buffer, 0, _columnBuffer, index, 4);
			index += 4;

			// Values
			for (int i = 0; i < Values.Length; i++)
			{
				//TODO there must be a less expensive way.
				buffer = BitConverter.GetBytes(Values[i]);
				System.Array.Copy(buffer, 0, _columnBuffer, index, 8);
				index += 8;
			}

			// CheckSum
			int checkSum = 0;
			for (int i = 0; i < (Values.Length * 8); i++)
				checkSum += _columnBuffer[i + 4];
			_columnBuffer[index] = (byte)(checkSum % 256);
			index += 1;

			_columnDataStream.Position = _position;

			_columnDataStream.Write(_columnBuffer, 0, _columnBuffer.Length);
			_columnDataStream.Flush();
		}

		public void Dispose()
		{
			if (Values != null)
			{
				Graph.Functions.Function.ColumnPool.Push(Values.Length, Values);
				Values = null;
			}
		}

		public static int BufferLength(int length) { return 4 + (length * 8) + 1; }

	}
}
