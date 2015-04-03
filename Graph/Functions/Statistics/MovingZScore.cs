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
	public class MovingZScore : Graph.Functions.Function
	{
		private readonly MovingMean _mu;
		private readonly MovingStandardDeviation _sigma;

		public MovingZScore(int n)
		{
			_mu = new MovingMean(n);
			_sigma = new MovingStandardDeviation(n);
		}

		public override void Clear()
		{
			_mu.Clear();
			_sigma.Clear();

			Count = 0;
			Value = double.NaN;
		}

		public override double[] Evaluate(double[] X)
		{
			double[] Y = (ColumnPool.Count(X.Length) == 0) ? new double[X.Length] : ColumnPool.Pop(X.Length);
			double[] mu = _mu.Evaluate(X);
			double[] sigma = _sigma.Evaluate(X);
			for (int i = 0; i < Y.Length; i++)
			{
				if (double.IsNaN(mu[i]) || double.IsNaN(sigma[i]))
				    Y[i] = double.NaN;
				else
					Y[i] = (X[i] - mu[i]) / sigma[i];
			}
			ColumnPool.Push(Y.Length, mu);
			ColumnPool.Push(Y.Length, sigma);
			return Y;
		}

	}
}
