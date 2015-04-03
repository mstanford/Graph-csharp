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
	public class TrailingVolatility : Function
	{
		private readonly Statistics.TrailingStandardDeviation _trailingStandardDeviation;
		private readonly int _tradingDaysPerYear;
		private double _priorPrice = double.NaN;

		public TrailingVolatility(double alpha, int tradingDaysPerYear)
		{
			_trailingStandardDeviation = new Statistics.TrailingStandardDeviation(alpha);
			_tradingDaysPerYear = tradingDaysPerYear;
		}

		public override double Update(double x)
		{
			if (!double.IsNaN(_priorPrice))
			{
				double logReturns = System.Math.Log(x / _priorPrice);
				_trailingStandardDeviation.Update(logReturns);
			}
			_priorPrice = x;

			Count++;
			if (!double.IsNaN(_trailingStandardDeviation.Value))
				Value = _trailingStandardDeviation.Value * System.Math.Sqrt(_tradingDaysPerYear);
			else
				Value = double.NaN;
			return Value;
		}

		public override void Clear()
		{
			_trailingStandardDeviation.Clear();

			Count = 0;
			Value = double.NaN;
			_priorPrice = double.NaN;
		}

	}
}
