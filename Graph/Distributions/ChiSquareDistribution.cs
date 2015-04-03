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

namespace Graph.Distributions
{
	public class ChiSquareDistribution : Distribution
	{
		private readonly int _alpha;

		public ChiSquareDistribution(int alpha)
			: base()
		{
			_alpha = alpha;
		}

		public override double CDF(double x)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override double Entropy()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override double Inverse(double p)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override double Mean()
		{
			return _alpha;
		}

		public override double Median()
		{
			return _alpha - 2.0 / 3.0;
		}

		public override double Mode()
		{
			if (_alpha >= 2)
				return _alpha - 2.0;
			return double.NaN;

		}

		public override double PDF(double x)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override double Random(Random random)
		{
			double sum = 0.0;
			for (int i = 0; i < _alpha; i++)
				sum += System.Math.Pow(new NormalDistribution(0, 1).Random(random), 2);
			return sum;
		}

		public override double Variance()
		{
			return 2.0 * _alpha;
		}

	}
}
