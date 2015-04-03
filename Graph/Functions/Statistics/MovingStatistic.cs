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

namespace Graph.Functions.Statistics
{
	public abstract class MovingStatistic : Function
	{
		protected readonly int _n;

		public MovingStatistic(int n)
		{
			_n = n;
		}

		public virtual double Calculate(double[] X, int head, int tail) { throw new System.Exception(); }

		public virtual double Calculate2(double[] X1, double[] X2, int head, int tail) { throw new System.Exception(); }

		public override void Clear()
		{
			Count = 0;
			Value = double.NaN;
		}

		public override double[] Evaluate(double[] X)
		{
			double[] Y = (ColumnPool.Count(X.Length) == 0) ? new double[X.Length] : ColumnPool.Pop(X.Length);
			for (int j = 0; j < Y.Length; j++)
				Y[j] = double.NaN;

			int head = -1;
			int length = 0;
			while (length < _n && (head + 1) < X.Length)
			{
				head++;
				if (!double.IsNaN(X[head]))
					length++;
			}
			if (length != _n)
				return Y;
				//throw new System.Exception();
			
			int tail = 0;
			while (double.IsNaN(X[tail]))
				tail++;

			while (head < X.Length)
			{
				int count = 0;
				for (int j = tail; j <= head; j++)
					if (!double.IsNaN(X[j]))
						count++;
				if (count != _n)
				    throw new System.Exception();

				Y[head] = Calculate(X, head, tail);
				if (double.IsNaN(Y[head]) || double.IsInfinity(Y[head]))
					throw new System.Exception();

				head++;
				while (head < X.Length && double.IsNaN(X[head]))
					head++;
				if (head == X.Length)
					break;
				tail++;
				while (double.IsNaN(X[tail]))
					tail++;
			}

			return Y;
		}

		public override double[] Evaluate2(double[] X1, double[] X2)
		{
			if (X1.Length != X2.Length)
				throw new System.Exception();

			double[] Y = (ColumnPool.Count(X1.Length) == 0) ? new double[X1.Length] : ColumnPool.Pop(X1.Length);
			for (int j = 0; j < Y.Length; j++)
				Y[j] = double.NaN;

			int head = -1;
			int length = 0;
			while (length < _n && (head + 1) < X1.Length)
			{
				head++;
				if (!double.IsNaN(X1[head]) && !double.IsNaN(X2[head]))
					length++;
			}
			if (length != _n)
				return Y;
				//throw new System.Exception();

			int tail = 0;
			while (double.IsNaN(X1[tail]) || double.IsNaN(X2[tail]))
				tail++;

			while (head < X1.Length)
			{
				int count = 0;
				for (int j = tail; j <= head; j++)
					if (!double.IsNaN(X1[j]) && !double.IsNaN(X2[j]))
						count++;
				if (count != _n)
					throw new System.Exception();

				Y[head] = Calculate2(X1, X2, head, tail);
				if (double.IsNaN(Y[head]) || double.IsInfinity(Y[head]))
					throw new System.Exception();

				head++;
				while (head < X1.Length && (double.IsNaN(X1[head]) || double.IsNaN(X2[head])))
					head++;
				if (head == X1.Length)
					break;
				tail++;
				while (double.IsNaN(X1[tail]) || double.IsNaN(X2[tail]))
					tail++;
			}

			return Y;
		}

	}
}
