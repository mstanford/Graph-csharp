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
	public class TrailingBeta : Function
	{
		private readonly TrailingCovariance _covariance_stock_market;
		private readonly TrailingVariance _sigma2_market;

		// Beta = Corr * (Sigma(a) / Sigma(b))
		// Beta = Cov(a, b) / Var(b)

		public TrailingBeta(double alpha)
		{
			_covariance_stock_market = new TrailingCovariance(alpha);
			_sigma2_market = new TrailingVariance(alpha);

			Clear();
		}

		public override void Clear()
		{
			_covariance_stock_market.Clear();
			_sigma2_market.Clear();

			Count = 0;
			Value = double.NaN;
		}

		public override double Update2(double x, double y)
		{
			if (double.IsNaN(x) || double.IsNaN(y))
				return double.NaN;

			_covariance_stock_market.Update2(x, y);
			_sigma2_market.Update(y);

			if (double.IsNaN(_covariance_stock_market.Value) || double.IsNaN(_sigma2_market.Value))
				Value = double.NaN;
			else
				Value = _covariance_stock_market.Value / _sigma2_market.Value;

			Count++;
			return Value;
		}

		public override double[] Evaluate2(double[] X1, double[] X2)
		{
			double[] Y = (Graph.Functions.Function.ColumnPool.Count(X1.Length) == 0) ? new double[X1.Length] : Graph.Functions.Function.ColumnPool.Pop(X1.Length);
			double[] covAB = _covariance_stock_market.Evaluate2(X1, X2);
			double[] varB = _sigma2_market.Evaluate2(X2, X1);
			for (int i = 0; i < Y.Length; i++)
			{
				if (double.IsNaN(X1[i]) || double.IsNaN(X2[2]) || X1[i] == 0.0 || X2[2] == 0.0 || double.IsNaN(covAB[i]) || double.IsNaN(varB[i]))
				{
					if (i == 0)
						Y[i] = double.NaN;
					else
						Y[i] = Y[i - 1];
				}
				else
				{
					Y[i] = covAB[i] / varB[i];
				}
			}
			Graph.Functions.Function.ColumnPool.Push(Y.Length, covAB);
			Graph.Functions.Function.ColumnPool.Push(Y.Length, varB);
			return Y;
		}

	}
}
