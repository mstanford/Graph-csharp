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
	public class CumulativeStandardDeviation : Function
	{
		private CumulativeVariance _variance;

		public CumulativeStandardDeviation()
		{
			_variance = new CumulativeVariance();

			Clear();
		}

		public override void Clear()
		{
			_variance.Clear();

			Count = 0;
			Value = double.NaN;
		}

		public override double Update(double x)
		{
			_variance.Update(x);
			if (double.IsNaN(_variance.Value))
				return double.NaN;
			Count++;
			Value = System.Math.Sqrt(_variance.Value);
			return Value;
		}

		public static double Calculate(double[] X)
		{
			double sum1 = 0.0;
			double count = 0.0;
			for (int i = 0; i < X.Length; i++)
			{
				if (!double.IsNaN(X[i]))
				{
					sum1 += X[i];
					count++;
				}
			}
			double mean = sum1 / count;
			double sum2 = 0.0;
			for (int i = 0; i < X.Length; i++)
			{
				if (!double.IsNaN(X[i]))
					sum2 += Square(X[i] - mean);
			}
			return System.Math.Sqrt(sum2 / (count - 1));
		}

		private static double Square(double a) { return a * a; }

	}
}
