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
	public class MovingCorrelation : Function
	{
		private MovingCovariance _covxy;
		private MovingStandardDeviation _sigma_x;
		private MovingStandardDeviation _sigma_y;

		public MovingCorrelation(int n)
		{
			_covxy = new MovingCovariance(n);
			_sigma_x = new MovingStandardDeviation(n);
			_sigma_y = new MovingStandardDeviation(n);
		}

		public override void Clear()
		{
			_covxy.Clear();
			_sigma_x.Clear();
			_sigma_y.Clear();

			Count = 0;
			Value = double.NaN;
		}

		public override double[] Evaluate2(double[] X1, double[] X2)
		{
			double[] Y = (ColumnPool.Count(X1.Length) == 0) ? new double[X1.Length] : ColumnPool.Pop(X1.Length);
			double[] covxy = _covxy.Evaluate2(X1, X2);
			double[] sigma_x = _sigma_x.Evaluate(X1);
			double[] sigma_y = _sigma_y.Evaluate(X2);
			for (int i = 0; i < Y.Length; i++)
			{
				if (double.IsNaN(X1[i]) || double.IsNaN(X2[i]))
				{
					Y[i] = double.NaN;
				}
				else
				{
					if (double.IsNaN(covxy[i]) || double.IsNaN(sigma_x[i]) || double.IsNaN(sigma_y[i]))
						Y[i] = double.NaN;
					else
						Y[i] = covxy[i] / (sigma_x[i] * sigma_y[i]);
				}
			}
			ColumnPool.Push(X1.Length, covxy);
			ColumnPool.Push(X1.Length, sigma_x);
			ColumnPool.Push(X1.Length, sigma_y);
			return Y;
		}

	}
}
