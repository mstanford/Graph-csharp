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
	public class CumulativeVariance : Function
	{
		private double _mean;
		private double _M2;

		public override void Clear()
		{
			_mean = 0.0;
			_M2 = 0.0;

			Count = 0;
			Value = double.NaN;
		}

		public override double Update(double x)
		{
			if (double.IsNaN(x))
				return double.NaN;
			Count++;
			double delta = x - _mean;
			_mean = _mean + delta / Count;
			_M2 = _M2 + delta * (x - _mean);
			Value = _M2 / (Count - 1);
			return Value;
		}

		//// Bessel's correction:  ad.Length - 1
		//public static double Variance(double[] ad) { double mean = Mean(ad); double variance = 0; for (int i = 0; i < ad.Length; i++) variance += Square(ad[i] - mean) / (ad.Length - 1); return variance; }

	}
}
