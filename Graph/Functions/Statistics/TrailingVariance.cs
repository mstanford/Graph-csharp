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
	public class TrailingVariance : Function
	{
		private readonly double _alpha;
		private readonly int _n;
		private readonly CumulativeMean _meanEstimate;
		private readonly CumulativeVariance _varianceEstimate;
		public double _mean;

		public TrailingVariance(double alpha)
		{
			_alpha = alpha;
			_n = (int)System.Math.Ceiling((2 / (1 - alpha)) + 1);
			_meanEstimate = new CumulativeMean();
			_varianceEstimate = new CumulativeVariance();

			Clear();
		}

		public override void Clear()
		{
			_meanEstimate.Clear();
			_varianceEstimate.Clear();
			_mean = double.NaN;

			Count = 0;
			Value = double.NaN;
		}

		public override double Update(double x)
		{
			if (double.IsNaN(x) || x == 0.0)
				return Value;

			if (_varianceEstimate.Count < _n)
			{
				_varianceEstimate.Update(x);
				_meanEstimate.Update(x);
				if (_varianceEstimate.Count == _n)
				{
					Value = _varianceEstimate.Value;
					_mean = _meanEstimate.Value;
				}
				return Value;
			}
			Count++;
			_mean = ((1.0 - _alpha) * x) + (_alpha * _mean);
			Value = ((1.0 - _alpha) * (x - _mean) * (x - _mean)) + (_alpha * Value);
			return Value;
		}

		public override double Update2(double x, double y)
		{
			// If the stock doesn't have a return don't update the benchmark variance for beta calculation.
			if (double.IsNaN(x) || x == 0.0 || double.IsNaN(y) || y == 0.0)
				return Value;

			return Update(x);
		}

	}
}
