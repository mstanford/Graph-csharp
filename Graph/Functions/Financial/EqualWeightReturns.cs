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

namespace Graph.Functions.Financial
{
	public class EqualWeightReturns : Function
	{

		public override void EvaluateQuery(Dictionary<string, object> STATE, string hiveName, string x, string y)
		{
			Graph.Data.Hive hive = (Graph.Data.Hive)STATE[hiveName];

			List<Graph.Data.Column> cumulativeReturnsColumns = hive.Slice(x).Messages;
			if (cumulativeReturnsColumns.Count == 0)
				throw new System.Exception();

			Graph.Data.Column yColumn = hive.CreateOne("", y);

			yColumn.Protect(0);
			yColumn[0] = 0.0;
			for (int i = 1; i < yColumn.Length; i++)
			{
				double returns = 0.0;
				double count = 0.0;

				for (int j = 0; j < cumulativeReturnsColumns.Count; j++)
				{
					cumulativeReturnsColumns[j].Protect(i);
					if (!double.IsNaN(cumulativeReturnsColumns[j][i]) && !double.IsNaN(cumulativeReturnsColumns[j][i - 1]))
					{
						returns += (cumulativeReturnsColumns[j][i] / cumulativeReturnsColumns[j][i - 1]) - 1.0;
						count++;
					}
				}

				yColumn.Protect(i);
				yColumn[i] = (count == 0) ? 0.0 : System.Math.Log((returns / count) + 1.0);
			}

			yColumn.Protect(yColumn.Length - 1);

			// Cumulative
			for (int i = 1; i < yColumn.Length; i++)
				yColumn[i] += yColumn[i - 1];

			for (int i = 0; i < yColumn.Length; i++)
				yColumn[i] = System.Math.Exp(yColumn[i]);
		}

		public override void Clear() { }

	}
}
