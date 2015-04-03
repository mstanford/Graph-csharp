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
	public class SplitAdjustedLogReturns : Function
	{
		private readonly double _upperThreshold;
		private readonly double _lowerThreshold;

		public SplitAdjustedLogReturns(double upperThreshold, double lowerThreshold)
		{
			_upperThreshold = upperThreshold;
			_lowerThreshold = lowerThreshold;
		}

		public override double[] Evaluate2(double[] X1, double[] X2)
		{
			double[] Y = (ColumnPool.Count(X1.Length) == 0) ? new double[X1.Length] : ColumnPool.Pop(X1.Length);
			Y[0] = double.NaN;
			for (int i = 1; i < Y.Length; i++)
			{
				if (!double.IsNaN(X1[i - 1]) && X1[i - 1] != 0.0 && !double.IsNaN(X1[i]) && X1[i] != 0.0)
				{
					if (!double.IsNaN(X2[i]))
					{
						Y[i] = System.Math.Log(X1[i] / (X1[i - 1] / X2[i]));
					}
					else
					{
						Y[i] = System.Math.Log(X1[i] / X1[i - 1]);
					}

					if (Y[i] > _upperThreshold)
					{
						Y[i] = 0.0;
					}
					else if (Y[i] < _lowerThreshold)
					{
						Y[i] = 0.0;
					}
				}
				else
				{
					Y[i] = 0.0;
				}
			}
			return Y;
		}

		public override void Clear() { }

	}
}
