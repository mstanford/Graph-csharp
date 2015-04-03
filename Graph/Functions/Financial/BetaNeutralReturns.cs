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
	public class BetaNeutralReturns : Function
	{

		public override double[] Evaluate3(double[] X1, double[] X2, double[] X3)
		{
			double[] Y = (ColumnPool.Count(X1.Length) == 0) ? new double[X1.Length] : ColumnPool.Pop(X1.Length);

			Y[0] = 1.0;
			for (int i = 1; i < Y.Length; i++)
			{
				Y[i] = Y[i - 1];

				if (
					!double.IsNaN(X1[i]) &&
					!double.IsNaN(X1[i - 1]) &&
					!double.IsNaN(X2[i]) &&
					!double.IsNaN(X2[i - 1]) &&
					!double.IsNaN(X3[i]) &&
					!double.IsNaN(X3[i - 1]))
				{
					double longReturns = new Financial.LeveragedReturns().Update3(X1[i], X1[i - 1], 1.0);
					double shortReturns = new Financial.LeveragedReturns().Update3(X2[i], X2[i - 1], -X3[i]);

					if (longReturns != 0.0)
						Y[i] = Y[i - 1] * (longReturns * shortReturns);
				}
			}

			return Y;
		}

		//public override void EvaluateQuery2(Dictionary<string, object> STATE, string hiveName, string x1, string x2, string y)
		//{
		//    Graph.Data.Hive hive = (Graph.Data.Hive)STATE[hiveName];

		//    List<string> dates = hive.GetRowKeys("Date");

		//    List<Graph.Data.Column> longCumulativeReturnsColumns = hive.Slice(x1).Messages;
		//    if (longCumulativeReturnsColumns.Count == 0)
		//        throw new System.Exception();

		//    foreach (Graph.Data.Column longCumulativeReturnsColumn in longCumulativeReturnsColumns)
		//    {
		//        Graph.Data.Column benchmarkCumulativeReturnsColumn = hive.ReadOne("", x2);

		//        Graph.Data.Column yColumn = hive.CreateOne(longCumulativeReturnsColumn.Query, y);

		//        yColumn.Protect(0);
		//        yColumn[0] = 1.0;
		//        for (int i = 1; i < yColumn.Length; i++)
		//        {
		//            longCumulativeReturnsColumn.Protect(i);
		//            benchmarkCumulativeReturnsColumn.Protect(i);

		//            yColumn.Protect(i);
		//            yColumn[i] = yColumn[i - 1];

		//            if (
		//                !double.IsNaN(longCumulativeReturnsColumn[i]) && 
		//                !double.IsNaN(longCumulativeReturnsColumn[i - 1]) && 
		//                !double.IsNaN(benchmarkCumulativeReturnsColumn[i]) && 
		//                !double.IsNaN(benchmarkCumulativeReturnsColumn[i - 1]))
		//            {
		//                double longReturns = new Financial.LeveragedReturns().Update3(longCumulativeReturnsColumn[i], longCumulativeReturnsColumn[i - 1], 1.0);
		//                double shortReturns = new Financial.LeveragedReturns().Update3(benchmarkCumulativeReturnsColumn[i], benchmarkCumulativeReturnsColumn[i - 1], -1.0);

		//                if (longReturns != 0.0)
		//                    yColumn[i] = yColumn[i - 1] * (longReturns * shortReturns);
		//            }
		//        }
		//    }
		//}

		public override void Clear() { }

	}
}
