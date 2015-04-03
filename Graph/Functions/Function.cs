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
	public abstract class Function
	{
		public static readonly Graph.Collections.Pool<int, double> ColumnPool = new Graph.Collections.Pool<int, double>();

		public int Count;

		public double Value = double.NaN;

		public abstract void Clear();

		public virtual double Update(double x) { throw new System.Exception(); }

		public virtual double Update2(double x1, double x2) { throw new System.Exception(); }

		public virtual double Reduce(double[] X) { throw new System.Exception(); }

		public virtual double[] Evaluate(double[] X)
		{
			double[] Y = (ColumnPool.Count(X.Length) == 0) ? new double[X.Length] : ColumnPool.Pop(X.Length);
			for (int i = 0; i < X.Length; i++)
				Y[i] = Update(X[i]);
			return Y;
		}

		public virtual double[] Evaluate2(double[] X1, double[] X2)
		{
			if (X1.Length != X2.Length)
				throw new System.Exception();

			double[] Y = (ColumnPool.Count(X1.Length) == 0) ? new double[X1.Length] : ColumnPool.Pop(X1.Length);
			for (int i = 0; i < X1.Length; i++)
			{
				if (double.IsNaN(X1[i]) || double.IsNaN(X2[i]))
					Y[i] = double.NaN;
				else
					Y[i] = Update2(X1[i], X2[i]);
			}
			return Y;
		}

		public virtual double[] Evaluate3(double[] X1, double[] X2, double[] X3) { throw new System.Exception(); }

		public virtual void EvaluateQuery(Dictionary<string, object> STATE, string hiveName, string x, string y)
		{
			Graph.Data.Hive hive = (Graph.Data.Hive)STATE[hiveName];

			List<Graph.Data.Column> columns = hive.Slice(x).Messages;
			if (columns.Count == 0)
				throw new System.Exception();

			foreach (Graph.Data.Column xColumn in columns)
			{
				double[] X = (Graph.Functions.Function.ColumnPool.Count(xColumn.Length) == 0) ? new double[xColumn.Length] : Graph.Functions.Function.ColumnPool.Pop(xColumn.Length);
				xColumn.Protect(xColumn.Length - 1);
				for (int i = 0; i < xColumn.Length; i++)
					X[i] = xColumn[i];

				Clear();
				double[] Y = Evaluate(X);

				Graph.Data.Column yColumn = hive.CreateOne(xColumn.Query, y);
				yColumn.Protect(Y.Length - 1);
				for (int i = 0; i < Y.Length; i++)
					yColumn[i] = Y[i];

				Graph.Functions.Function.ColumnPool.Push(X.Length, X);
				Graph.Functions.Function.ColumnPool.Push(Y.Length, Y);
			}

			hive.Flush();
		}

		public virtual void EvaluateQuery2(Dictionary<string, object> STATE, string hiveName, string x1, string x2, string y)
		{
			Graph.Data.Hive hive = (Graph.Data.Hive)STATE[hiveName];

			List<Graph.Data.Column> columns = hive.Slice(x1).Messages;
			if (columns.Count == 0)
				throw new System.Exception();

			foreach (Graph.Data.Column x1Column in columns)
			{
				Graph.Data.Column x2Column = hive.ReadOne(x1Column.Query, x2);
				if (x2Column == null)
					x2Column = hive.ReadOne("", x2);

				double[] X1 = (Graph.Functions.Function.ColumnPool.Count(x1Column.Length) == 0) ? new double[x1Column.Length] : Graph.Functions.Function.ColumnPool.Pop(x1Column.Length);
				double[] X2 = (Graph.Functions.Function.ColumnPool.Count(x1Column.Length) == 0) ? new double[x1Column.Length] : Graph.Functions.Function.ColumnPool.Pop(x1Column.Length);
				x1Column.Protect(x1Column.Length - 1);
				x2Column.Protect(x1Column.Length - 1);
				for (int i = 0; i < x1Column.Length; i++)
				{
					X1[i] = x1Column[i];
					X2[i] = x2Column[i];
				}

				Clear();
				double[] Y = Evaluate2(X1, X2);

				Graph.Data.Column yColumn = hive.CreateOne(x1Column.Query, y);
				yColumn.Protect(Y.Length - 1);
				for (int i = 0; i < Y.Length; i++)
					yColumn[i] = Y[i];

				Graph.Functions.Function.ColumnPool.Push(X1.Length, X1);
				Graph.Functions.Function.ColumnPool.Push(X2.Length, X2);
				Graph.Functions.Function.ColumnPool.Push(Y.Length, Y);
			}

			hive.Flush();
		}

		public double[] EvaluateScalar(double[] X, double y)
		{
			double[] Y = (ColumnPool.Count(X.Length) == 0) ? new double[X.Length] : ColumnPool.Pop(X.Length);
			for (int i = 0; i < X.Length; i++)
				Y[i] = Update2(X[i], y);
			return Y;
		}

	}
}
