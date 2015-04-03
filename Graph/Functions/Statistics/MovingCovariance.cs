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
	public class MovingCovariance : MovingStatistic
	{
		private readonly MovingMean _movingMean;

		public MovingCovariance(int n)
			: base(n)
		{
			_movingMean = new MovingMean(n);
		}

		public override double Calculate2(double[] X1, double[] X2, int head, int tail)
		{
			double mean1 = _movingMean.Calculate(X1, head, tail);
			double mean2 = _movingMean.Calculate(X2, head, tail);

			double covariance = 0;
			for (int i = tail; i <= head; i++)
				if (!double.IsNaN(X1[i]) && !double.IsNaN(X2[i]))
					covariance += (X1[i] - mean1) * (X2[i] - mean2) / (_n - 1);
			return covariance;
		}

	}
}
