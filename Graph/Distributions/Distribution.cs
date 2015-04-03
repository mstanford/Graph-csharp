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
	public abstract class Distribution
	{

		public Distribution() { }

		public abstract double CDF(double x);

		public virtual double Entropy() { throw new System.Exception(); }

		public abstract double Inverse(double p);

		public abstract double Mean();

		public abstract double Median();

		public abstract double Mode();

		public abstract double PDF(double x);

		public virtual double Random(System.Random random)
		{
			return Inverse(random.NextDouble());
		}

		public abstract double Variance();

		protected static double erf(double z)
		{
			// FROM http://www.cs.princeton.edu/introcs/21function/MyMath.java.html
			// fractional error in math formula less than 1.2 * 10 ^ -7.
			// although subject to catastrophic cancellation when z in very close to 0

			// Chebyshev fitting formula for erf(z) from Numerical Recipes
			double t = 1.0 / (1.0 + 0.5 * System.Math.Abs(z));
			// use Horner's method
			double ans = 1 - t * System.Math.Exp(-z * z - 1.26551223 +
					t * (1.00002368 +
					t * (0.37409196 +
					t * (0.09678418 +
					t * (-0.18628806 +
					t * (0.27886807 +
					t * (-1.13520398 +
					t * (1.48851587 +
					t * (-0.82215223 +
					t * (0.17087277))))))))));
			if (z >= 0) return ans;
			else return -ans;
		}

	}
}
