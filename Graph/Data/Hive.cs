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
	public class Hive
	{
		private readonly System.IO.Stream _rowKeyStream;
		private readonly System.IO.Stream _columnTagStream;
		private readonly System.IO.Stream _columnIndexStream;
		private readonly System.IO.Stream _columnDataStream;

		private Dictionary<long, string> _positions;
		public Cube<Column> Cube;

		public Lookup<string> ColumnTypes;
		public Dictionary<string, Lookup<string>> RowKeysByColumnType;

		private Dictionary<Tag, int> _columnTagDictionary;
		private List<Tag> _columnTagList;

		private Dictionary<string, Dictionary<string, string>> _columnSchemasByType;

		private List<Column> _cachedColumns;
		private Dictionary<int, byte[]> _columnBuffers;

		public Hive(System.IO.Stream rowKeyStream, System.IO.Stream columnTagStream, System.IO.Stream columnIndexStream, System.IO.Stream columnDataStream)
		{
			_rowKeyStream = rowKeyStream;
			_columnTagStream = columnTagStream;
			_columnIndexStream = columnIndexStream;
			_columnDataStream = columnDataStream;

			Initialize();
		}

		private void Initialize()
		{
			_positions = new Dictionary<long, string>();

			Cube = new Cube<Column>();

			ColumnTypes = new Lookup<string>();
			RowKeysByColumnType = new Dictionary<string, Lookup<string>>();

			_columnTagDictionary = new Dictionary<Tag, int>();
			_columnTagList = new List<Tag>();

			_columnSchemasByType = new Dictionary<string, Dictionary<string, string>>();

			_cachedColumns = new List<Column>();
			_columnBuffers = new Dictionary<int, byte[]>();


			byte[] buffer4 = new byte[4];
			byte[] buffer8 = new byte[8];


			// Row Keys
			while (_rowKeyStream.Position < _rowKeyStream.Length)
			{
				int expectedColumnTypeId = Binary.ReadInt32(_rowKeyStream, buffer4);
				string columnType = Binary.ReadString(_rowKeyStream);
				int columnTypeId = ColumnTypes.Add(columnType);
				if (columnTypeId != expectedColumnTypeId)
					throw new System.Exception();

				Lookup<string> rowKeyLookup = new Lookup<string>();
				RowKeysByColumnType.Add(columnType, rowKeyLookup);

				int n = Binary.ReadInt32(_rowKeyStream, buffer4);
				for (int i = 0; i < n; i++)
				{
					int expectedRowLabelId = Binary.ReadInt32(_rowKeyStream, buffer4);
					string rowLabel = Binary.ReadString(_rowKeyStream);
					int rowLabelId = rowKeyLookup.Add(rowLabel);
					if (rowLabelId != expectedRowLabelId)
						throw new System.Exception();
				}
			}
			_rowKeyStream.Flush();


			//Column Tags
			while (_columnTagStream.Position < _columnTagStream.Length)
			{
				if (_columnTagList.Count != Binary.ReadInt32(_columnTagStream, buffer4))
					throw new System.Exception();
				Tag tag = new Tag(Binary.ReadString(_columnTagStream), Binary.ReadString(_columnTagStream));

				_columnTagDictionary.Add(tag, _columnTagList.Count);
				_columnTagList.Add(tag);
			}
			_columnTagStream.Flush();


			// Data
			while (_columnIndexStream.Position < _columnIndexStream.Length)
			{
				//string columnType = RowKeys.ColumnTypes[IO.Binary.ReadInt32(_columnIndexStream, buffer4)];
				string columnType = null;
				Tag[] columnTags = new Tag[Binary.ReadInt32(_columnIndexStream, buffer4)];
				for (int i = 0; i < columnTags.Length; i++)
				{
					columnTags[i] = ColumnTag(Binary.ReadInt32(_columnIndexStream, buffer4));
					if (columnTags[i].Name.Equals("ColumnType"))
						columnType = columnTags[i].Value;
				}
				if (columnType == null)
					throw new System.Exception();

				long position = Binary.ReadInt64(_columnIndexStream, buffer8);

				int columnLength = RowKeysByColumnType[columnType].Count;

				if (!_columnBuffers.ContainsKey(columnLength))
					_columnBuffers.Add(columnLength, new byte[Column.BufferLength(columnLength)]);

				string query = Tag.Generate(columnTags);
				_positions.Add(position, query);
				Cube.Add(new Column(query, columnTags, _columnSchemasByType, _columnDataStream, _columnBuffers[columnLength], columnLength, position));
			}
		}

		public void Clear()
		{
			_rowKeyStream.SetLength(0);
			_columnTagStream.SetLength(0);
			_columnIndexStream.SetLength(0);
			_columnDataStream.SetLength(0);

			Initialize();
		}

		public bool ContainsRowKey(Tag rowKey)
		{
			return RowKeysByColumnType.ContainsKey(rowKey.Name) && RowKeysByColumnType[rowKey.Name].Contains(rowKey.Value);
		}

		public int GetRowKey(Tag rowKey)
		{
			if (!ContainsRowKey(rowKey))
				throw new System.Exception();
			return RowKeysByColumnType[rowKey.Name][rowKey.Value];
		}

		public List<string> GetRowKeys(string columnType)
		{
			int length = RowKeysByColumnType[columnType].Count;
			List<string> rowKeys = new List<string>();
			for (int i = 0; i < length; i++)
				rowKeys.Add(RowKeysByColumnType[columnType][i]);
			return rowKeys;
		}

		public void SetRowKeys(string columnType, List<string> rowKeys)
		{
			if (!RowKeysByColumnType.ContainsKey(columnType))
			{
				rowKeys.Sort(); // Row labels must be sorted.

				int columnTypeId = ColumnTypes.Add(columnType);
				Binary.WriteInt32(columnTypeId, _rowKeyStream);
				Binary.WriteString(columnType, _rowKeyStream);

				Lookup<string> rowKeyLookup = new Lookup<string>();
				RowKeysByColumnType.Add(columnType, rowKeyLookup);

				Binary.WriteInt32(rowKeys.Count, _rowKeyStream);
				for (int i = 0; i < rowKeys.Count; i++)
				{
					int rowLabelId = rowKeyLookup.Add(rowKeys[i]);
					Binary.WriteInt32(rowLabelId, _rowKeyStream);
					Binary.WriteString(rowKeys[i], _rowKeyStream);
				}

				_rowKeyStream.Flush();
			}
			else
			{
				// Verify
				rowKeys.Sort(); // Row labels must be sorted.
				Lookup<string> rowKeyLookup = RowKeysByColumnType[columnType];
				for (int i = 0; i < rowKeys.Count; i++)
					if (i != rowKeyLookup[rowKeys[i]])
						throw new System.Exception();
			}
		}

		public Tag ColumnTag(int index) { return _columnTagList[index]; }
		public int ColumnTag(Tag tag) { if (!_columnTagDictionary.ContainsKey(tag)) AddColumnTag(tag); return _columnTagDictionary[tag]; }

		public int ColumnTagCount { get { return _columnTagList.Count; } }

		public void AddColumnTag(Tag tag)
		{
			if (_columnTagDictionary.ContainsKey(tag))
				throw new System.Exception("Duplicate tag.");

			Binary.WriteInt32(_columnTagList.Count, _columnTagStream);
			Binary.WriteString(tag.Name, _columnTagStream);
			Binary.WriteString(tag.Value, _columnTagStream);
			_columnTagStream.Flush();

			_columnTagDictionary.Add(tag, _columnTagList.Count);
			_columnTagList.Add(tag);
		}

		public Cube<Column> Slice(string query)
		{
			Cube<Column> slice = Cube.Slice(Tag.Parse(query));
			for (int i = 0; i < slice.Messages.Count; i++)
			{
				if (!slice.Messages[i].Loaded)
				{
					slice.Messages[i].Load();
					_cachedColumns.Add(slice.Messages[i]);
				}
			}
			return slice;
		}

		public Column CreateOne(string baseQuery, string query)
		{
			Tag[] columnTags = Merge(Tag.Parse(baseQuery), Tag.Parse(query));
			List<Column> columns = Cube.Slice(columnTags).Messages;
			if (columns.Count == 0)
			{
				string columnType = null;
				for (int i = 0; i < columnTags.Length; i++)
					if (columnTags[i].Name.Equals("ColumnType"))
						columnType = columnTags[i].Value;
				if (columnType == null)
					throw new System.Exception();

				int columnLength = RowKeysByColumnType[columnType].Count;
				long position = _columnDataStream.Length;

				if (!_columnBuffers.ContainsKey(columnLength))
					_columnBuffers.Add(columnLength, new byte[Column.BufferLength(columnLength)]);

				string q = Tag.Generate(columnTags);
				_positions.Add(position, q);
				Column column = new Column(q, columnTags, _columnSchemasByType, _columnDataStream, _columnBuffers[columnLength], columnLength, position);
				if (!column.IsDefined("ColumnType"))
					throw new System.Exception();
				column.Initialize();

				Cube.Add(column);

				//IO.Binary.WriteInt32(RowKeys.ColumnTypes[columnType], _columnIndexStream);
				Binary.WriteInt32(columnTags.Length, _columnIndexStream);
				for (int i = 0; i < columnTags.Length; i++)
					Binary.WriteInt32(ColumnTag(columnTags[i]), _columnIndexStream);
				Binary.WriteInt64(position, _columnIndexStream);
				_columnIndexStream.Flush();

				_cachedColumns.Add(column);
				column.Protect(-1);
				//column.Protect(_protectionLimit);
				return column;
			}
			if (columns.Count != 1)
				throw new System.Exception();
			if (!columns[0].Loaded)
			{
				columns[0].Load();
				columns[0].Protect(columns[0].Length - 1);
				for (int i = 0; i < columns[0].Length; i++)
					columns[0][i] = double.NaN;
				_cachedColumns.Add(columns[0]);
			}
			//columns[0].Protect(_protectionLimit);
			columns[0].Protect(-1);
			return columns[0];
		}

		public Column ReadOne(string baseQuery, string query)
		{
			Tag[] columnTags = Merge(Tag.Parse(baseQuery), Tag.Parse(query));
			List<Column> columns = Cube.Slice(columnTags).Messages;
			if (columns.Count != 1)
				return null;
				//throw new System.Exception();
			if (!columns[0].Loaded)
			{
				columns[0].Load();
				_cachedColumns.Add(columns[0]);
			}
			columns[0].Protect(-1);
			return columns[0];
		}

		private static Tag[] Merge(Tag[] baseTags, Tag[] tags)
		{
			Lookup<string> tagNames = new Lookup<string>();
			Dictionary<string, Tag> tagsByName = new Dictionary<string, Tag>();
			int ignore;
			foreach (Tag tag in baseTags)
			{
				ignore = tagNames[tag.Name];
				tagsByName[tag.Name] = tag;
			}
			foreach (Tag tag in tags)
			{
				ignore = tagNames[tag.Name];
				tagsByName[tag.Name] = tag;
			}
			List<Tag> mergedTags = new List<Tag>();
			for (int i = 0; i < tagNames.Count; i++)
				mergedTags.Add(tagsByName[tagNames[i]]);
			return mergedTags.ToArray();
		}

		public void Flush()
		{
			foreach (Column column in _cachedColumns)
			{
				if (column.Modified)
				{
					column.Save();
					column.Modified = false;
				}
				column.Dispose();
			}
			_cachedColumns.Clear();
		}

		public void Close()
		{
			_rowKeyStream.Close();
			_columnTagStream.Close();
			_columnIndexStream.Close();
			_columnDataStream.Close();
		}

	}
}
