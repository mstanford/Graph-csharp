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
	public class Operators
	{

		public static double[] ColumnToArray(Column column)
		{
			double[] ad = new double[column.Length];
			for (int i = 0; i < column.Length; i++)
			{
				column.Protect(i);
				ad[i] = column[i];
			}
			return ad;
		}

		public static List<string> GetRowKeys(Hive hive, string columnType)
		{
			return hive.GetRowKeys(columnType);
		}

		public static Column ReadOne(Hive hive, string query)
		{
			Column column = hive.ReadOne("", query);
			if (column == null)
				throw new System.Exception();
			return column;
		}

		public static void Create(string hivePath, Dictionary<string, object> STATE, string hiveName, System.IO.TextWriter LOG)
		{
			string path = System.IO.Path.Combine(hivePath, hiveName);
			Graph.Data.Hive hive = new Graph.Data.Hive(IO.File.CreateReadWrite(path + ".row"), Graph.IO.File.CreateReadWrite(path + ".col"), Graph.IO.File.CreateReadWrite(path + ".idx"), Graph.IO.File.CreateReadWrite(path + ".dat"));
			STATE[hiveName] = hive;
		}

		public static void Open(string hivePath, Dictionary<string, object> STATE, string hiveName, System.IO.TextWriter LOG)
		{
			if (STATE.ContainsKey(hiveName) && STATE[hiveName] != null)
				return;
			string path = System.IO.Path.Combine(hivePath, hiveName);
			Graph.Data.Hive hive = new Graph.Data.Hive(IO.File.OpenReadWrite(path + ".row"), Graph.IO.File.OpenReadWrite(path + ".col"), Graph.IO.File.OpenReadWrite(path + ".idx"), Graph.IO.File.OpenReadWrite(path + ".dat"));
			STATE[hiveName] = hive;
		}

		public static void Flush(string hivePath, Dictionary<string, object> STATE, string hiveName, System.IO.TextWriter LOG)
		{
			Graph.Data.Hive hive = (Graph.Data.Hive)STATE[hiveName];
			hive.Flush();
		}

		public static void PopulateKeys(Dictionary<string, object> STATE, string hiveName, string key)
		{
			Graph.Data.Hive hive = (Graph.Data.Hive)STATE[hiveName];
			List<string> keys = hive.GetRowKeys(key);
			STATE["Keys." + key] = keys;
		}

		public static void ExportSchema(Dictionary<string, object> STATE, string hiveName, string query)
		{
			Graph.Data.Hive hive = (Graph.Data.Hive)STATE[hiveName];

			string file = System.IO.Path.GetTempFileName() + ".csv";
			System.IO.TextWriter writer = System.IO.File.CreateText(file);

			writer.WriteLine("Query");
			foreach (Column column in hive.Cube.Slice(Tag.Parse(query)).Messages)
			{
				writer.Write(column.Query);
				writer.WriteLine("");
			}

			writer.Flush();
			writer.Close();
			System.Diagnostics.Process.Start(file);
		}

		public static void ExportColumn(Dictionary<string, object> STATE, string hiveName, string query)
		{
			try
			{
				Graph.Data.Hive hive = (Graph.Data.Hive)STATE[hiveName];

				string file = System.IO.Path.GetTempFileName() + ".csv";
				System.IO.TextWriter writer = System.IO.File.CreateText(file);

				writer.WriteLine(query);
				Column column = hive.ReadOne("", query);
				List<string> keys = hive.GetRowKeys(column["ColumnType"]);
				column.Protect(column.Length - 1);
				for (int i = 0; i < column.Length; i++)
				{
					writer.Write(keys[i] + ",");
					if (!double.IsNaN(column[i]))
						writer.Write(column[i]);
					writer.WriteLine("");
				}

				writer.Flush();
				writer.Close();
				System.Diagnostics.Process.Start(file);
			}
			catch (System.Exception exception)
			{
				int n = 0;
			}
		}

		public static void ExportColumn2(Dictionary<string, object> STATE, string hiveName, string query1, string query2)
		{
			try
			{
				Graph.Data.Hive hive = (Graph.Data.Hive)STATE[hiveName];

				string file = System.IO.Path.GetTempFileName() + ".csv";
				System.IO.TextWriter writer = System.IO.File.CreateText(file);

				Column column1 = hive.ReadOne("", query1);
				Column column2 = hive.ReadOne("", query2);
				if (!column1["ColumnType"].Equals(column2["ColumnType"]))
					throw new System.Exception();
				List<string> keys = hive.GetRowKeys(column1["ColumnType"]);
				column1.Protect(column1.Length - 1);
				column2.Protect(column2.Length - 1);
				for (int i = 0; i < column1.Length; i++)
				{
					writer.Write(keys[i] + ",");
					if (!double.IsNaN(column1[i]))
						writer.Write(column1[i]);
					writer.Write(",");
					if (!double.IsNaN(column2[i]))
						writer.Write(column2[i]);
					writer.Write(",");
					writer.WriteLine("");
				}

				writer.Flush();
				writer.Close();
				System.Diagnostics.Process.Start(file);
			}
			catch (System.Exception exception)
			{
				int n = 0;
			}
		}

		public static void ExportColumn3(Dictionary<string, object> STATE, string hiveName, string query1, string query2, string query3)
		{
			try
			{
				Graph.Data.Hive hive = (Graph.Data.Hive)STATE[hiveName];

				string file = System.IO.Path.GetTempFileName() + ".csv";
				System.IO.TextWriter writer = System.IO.File.CreateText(file);

				Column column1 = hive.ReadOne("", query1);
				Column column2 = hive.ReadOne("", query2);
				Column column3 = hive.ReadOne("", query3);
				if (!column1["ColumnType"].Equals(column2["ColumnType"]) || !column1["ColumnType"].Equals(column3["ColumnType"]))
					throw new System.Exception();
				List<string> keys = hive.GetRowKeys(column1["ColumnType"]);
				column1.Protect(column1.Length - 1);
				column2.Protect(column2.Length - 1);
				column3.Protect(column2.Length - 1);
				for (int i = 0; i < column1.Length; i++)
				{
					writer.Write(keys[i] + ",");
					if (!double.IsNaN(column1[i]))
						writer.Write(column1[i]);
					writer.Write(",");
					if (!double.IsNaN(column2[i]))
						writer.Write(column2[i]);
					writer.Write(",");
					if (!double.IsNaN(column3[i]))
						writer.Write(column3[i]);
					writer.Write(",");
					writer.WriteLine("");
				}

				writer.Flush();
				writer.Close();
				System.Diagnostics.Process.Start(file);
			}
			catch (System.Exception exception)
			{
				int n = 0;
			}
		}

		public static void Close(string hivePath, Dictionary<string, object> STATE, string hiveName, System.IO.TextWriter LOG)
		{
			Graph.Data.Hive hive = (Graph.Data.Hive)STATE[hiveName];
			hive.Flush();
			hive.Close();
			STATE[hiveName] = null;

			System.Threading.Thread.Sleep(1000);
		}

	}
}
