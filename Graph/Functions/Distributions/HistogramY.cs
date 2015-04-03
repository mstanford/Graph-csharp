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

namespace Graph.Functions.Distributions
{
	public class HistogramY : Function
	{
		private readonly int _n;

		public HistogramY(int n)
		{
			_n = n;
		}

		public override double[] Evaluate2(double[] X1, double[] X2)
		{
			if (X1.Length != _n)
				throw new System.Exception();

			double binWidth = (X1[X1.Length - 1] - X1[0]) / (_n - 1);

			double[] Y = new double[_n];
			for (int i = 0; i < X2.Length; i++)
			{
				if (double.IsNaN(X2[i]))
					continue;
				int index = (int)System.Math.Round((X2[i] - X1[0]) / binWidth);
				Y[index]++;
			}

			return Y;
		}

		public override void Clear() { }

	}
}
