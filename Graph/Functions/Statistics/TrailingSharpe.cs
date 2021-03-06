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
	public class TrailingSharpe : Graph.Functions.Function
	{
		private readonly TrailingMean _mu;
		private readonly TrailingStandardDeviation _sigma;

		public TrailingSharpe(double alpha)
		{
			_mu = new TrailingMean(alpha);
			_sigma = new TrailingStandardDeviation(alpha);

			Clear();
		}

		public override void Clear()
		{
			_mu.Clear();
			_sigma.Clear();

			Count = 0;
			Value = double.NaN;
		}

		public override double Update2(double returns, double riskFreeReturns)
		{
			if (double.IsNaN(returns) || double.IsNaN(riskFreeReturns) || returns == 0.0)
				return Value;

			double r = returns - riskFreeReturns;
			_mu.Update(r);
			_sigma.Update(r);
			Value = double.NaN;
			if (_mu.Count > 252 && !double.IsNaN(_mu.Value) && !double.IsNaN(_sigma.Value))
			{
				Count++;
				Value = _mu.Value / _sigma.Value * System.Math.Sqrt(252);
			}
			return Value;
		}

	}
}
