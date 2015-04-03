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
	public class MovingLeastSquares : MovingStatistic
	{

		public MovingLeastSquares(int n) : base(n) { }

		public override double Calculate(double[] Y, int head, int tail)
		{
			double x_mean = 0.0;
			double y_mean = 0.0;
			for (int i = tail; i < head; i++)
			{
				x_mean += (i - tail);
				y_mean += Y[i];
			}
			x_mean /= _n;
			y_mean /= _n;

			double b1 = 0.0;
			double b2 = 0.0;
			for (int i = tail; i < head; i++)
			{
				b1 += Y[i] * ((i - tail) - x_mean);
				b2 += (i - tail) * ((i - tail) - x_mean);
			}

			double b = b1 / b2;
			double a = y_mean - x_mean * b;

			return a + (b * _n);
		}

	}
}
