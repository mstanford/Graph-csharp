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
	public class TrailingCorrelation : Graph.Functions.Function
	{
		private readonly double _alpha;
		private readonly TrailingCovariance _covxy;
		private readonly TrailingStandardDeviation _sigma_x;
		private readonly TrailingStandardDeviation _sigma_y;

		public TrailingCorrelation(double alpha)
		{
			_alpha = alpha;
			_covxy = new TrailingCovariance(alpha);
			_sigma_x = new TrailingStandardDeviation(alpha);
			_sigma_y = new TrailingStandardDeviation(alpha);

			Clear();
		}

		public override void Clear()
		{
			_covxy.Clear();
			_sigma_x.Clear();
			_sigma_y.Clear();

			Count = 0;
			Value = double.NaN;
		}

		public override double Update2(double x, double y)
		{
			if (double.IsNaN(x) || double.IsNaN(y))
				return double.NaN;

			_covxy.Update2(x, y);
			_sigma_x.Update(x);
			_sigma_y.Update(y);

			if (double.IsNaN(_covxy.Value) || double.IsNaN(_sigma_x.Value) || double.IsNaN(_sigma_y.Value))
				Value = double.NaN;
			else
				Value = _covxy.Value / (_sigma_x.Value * _sigma_y.Value);

			Count++;
			return Value;
		}

		public override double[] Evaluate2(double[] X1, double[] X2)
		{
			double[] Y = (ColumnPool.Count(X1.Length) == 0) ? new double[X1.Length] : ColumnPool.Pop(X1.Length);
			for (int i = 0; i < Y.Length; i++)
			{
				if (double.IsNaN(X1[i]) || double.IsNaN(X2[i]))
				{
					Y[i] = double.NaN;
				}
				else
				{
					_covxy.Update2(X1[i], X2[i]);
					_sigma_x.Update(X1[i]);
					_sigma_y.Update(X2[i]);

					if (double.IsNaN(_covxy.Value) || double.IsNaN(_sigma_x.Value) || double.IsNaN(_sigma_y.Value))
						Y[i] = double.NaN;
					else
						Y[i] = _covxy.Value / (_sigma_x.Value * _sigma_y.Value);
				}
			}
			return Y;
		}

	}
}
