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
	public class SquareReturns : Function
	{

		public override double[] Evaluate(double[] cumulativeReturns)
		{
			double[] sr = (ColumnPool.Count(cumulativeReturns.Length) == 0) ? new double[cumulativeReturns.Length] : ColumnPool.Pop(cumulativeReturns.Length);
			sr[0] = double.NaN;
			for (int i = 1; i < sr.Length; i++)
			{
				if (double.IsNaN(sr[i - 1]))
					sr[i - 1] = 0.0;
				sr[i] = (((cumulativeReturns[i] / cumulativeReturns[i - 1]) - 1.0) * ((cumulativeReturns[i] / cumulativeReturns[i - 1]) - 1.0));
			}
			return sr;
		}

		public override void Clear() { }

	}
}
