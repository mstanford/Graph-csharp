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
	public class StudentsTDistribution : Distribution
	{
		private readonly int _degreesOfFreedom;

		public StudentsTDistribution(int degreesOfFreedom)
			: base()
		{
			_degreesOfFreedom = degreesOfFreedom;
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
			if (_degreesOfFreedom > 1)
				return 0.0;
			return double.NaN;
		}

		public override double Median()
		{
			return 0.0;
		}

		public override double Mode()
		{
			return 0.0;
		}

		public override double PDF(double x)
		{
			double gamma1 = Gamma((_degreesOfFreedom + 1.0) / 2.0);
			double gamma2 = Gamma(1.0 / 2.0);
			double gamma3 = Gamma(_degreesOfFreedom / 2.0);
			double pdf = System.Math.Pow(_degreesOfFreedom, -0.5) * System.Math.Pow(1 + x * x / _degreesOfFreedom, -(_degreesOfFreedom + 1) / 2) * gamma1 / gamma2 / gamma3;
			if (double.IsNaN(pdf) || double.IsInfinity(pdf))
				return new NormalDistribution(0, 1).PDF(x);
			return pdf;
		}

		public override double Random(Random random)
		{
			//return new NormalDistribution(0, 1).Random() / new ChiDistribution(_degreesOfFreedom).Random();
			return new NormalDistribution(0, 1).Random(random) / System.Math.Sqrt(new ChiSquareDistribution(_degreesOfFreedom).Random(random) / _degreesOfFreedom);
		}

		public override double Variance()
		{
			if (_degreesOfFreedom > 2)
				return _degreesOfFreedom / (_degreesOfFreedom - 2.0);
			return double.NaN;
		}

		private static double Gamma(double x)
		{
			double[] p = {0.99999999999980993, 676.5203681218851, -1259.1392167224028,
			     	  771.32342877765313, -176.61502916214059, 12.507343278686905,
			     	  -0.13857109526572012, 9.9843695780195716e-6, 1.5056327351493116e-7};
			int g = 7;
			if (x < 0.5) return System.Math.PI / (System.Math.Sin(System.Math.PI * x) * Gamma(1 - x));

			x -= 1;
			double a = p[0];
			double t = x + g + 0.5;
			for (int i = 1; i < p.Length; i++)
			{
				a += p[i] / (x + i);
			}

			return System.Math.Sqrt(2 * System.Math.PI) * System.Math.Pow(t, x + 0.5) * System.Math.Exp(-t) * a;
		}

	}
}
