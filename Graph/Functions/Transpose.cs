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

namespace Graph.Functions
{
	public class Transpose : Function
	{

		public override void EvaluateQuery(Dictionary<string, object> STATE, string hiveName, string x, string y)
		{
			Graph.Data.Hive hive = (Graph.Data.Hive)STATE[hiveName];

			List<Graph.Data.Column> xColumns = hive.Slice(x).Messages;

			string yRowType = xColumns[0]["ColumnType"];
			List<string> xRowKeys = hive.GetRowKeys(yRowType);

			string columnType = null;
			Data.Tag[] tags = Data.Tag.Parse(y);
			for (int i = 0; i < tags.Length; i++)
				if (tags[i].Name.Equals("ColumnType"))
					columnType = tags[i].Value;

			for (int i = 0; i < xRowKeys.Count; i++)
			{
				Graph.Data.Column yColumn = hive.CreateOne(y, yRowType + "=" + xRowKeys[i]);
			}

			List<Graph.Data.Column> yColumns = hive.Slice(y).Messages;

			if (xColumns.Count > yColumns[0].Length)
				throw new System.Exception();

			string xRowType = yColumns[0]["ColumnType"];
			List<string> yRowKeys = hive.GetRowKeys(xRowType);

			for (int i = 0; i < yColumns.Count; i++)
			{
				yColumns[i].Protect(yColumns[i].Length - 1);

				for (int j = 0; j < yColumns[i].Length; j++)
				{
					xColumns[j].Protect(i);
					yColumns[i][j] = xColumns[j][i];
				}
			}
		}

		public override void Clear() { }

	}
}
