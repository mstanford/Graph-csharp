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
	public class CumulativeBeta : Function
	{
		private CumulativeCovariance _covariance_stock_market;
		private CumulativeVariance _sigma2_market;

		// Beta = Corr * (Sigma(a) / Sigma(b))
		// Beta = Cov(a, b) / Var(b)

		public CumulativeBeta()
		{
			Clear();
		}

		public override void Clear()
		{
			_covariance_stock_market = new CumulativeCovariance();
			_sigma2_market = new CumulativeVariance();

			Count = 0;
			Value = double.NaN;
		}

		public override double[] Evaluate2(double[] X1, double[] X2)
		{
			double[] Y = (Graph.Functions.Function.ColumnPool.Count(X1.Length) == 0) ? new double[X1.Length] : Graph.Functions.Function.ColumnPool.Pop(X1.Length);
			double[] covAB = _covariance_stock_market.Evaluate2(X1, X2);
			double[] varB = _sigma2_market.Evaluate(X2);
			for (int i = 0; i < Y.Length; i++)
			{
				if (double.IsNaN(covAB[i]) || double.IsNaN(varB[i]))
					Y[i] = double.NaN;
				else
					Y[i] = covAB[i] / varB[i];
			}
			Graph.Functions.Function.ColumnPool.Push(Y.Length, covAB);
			Graph.Functions.Function.ColumnPool.Push(Y.Length, varB);
			return Y;
		}

	}
}
