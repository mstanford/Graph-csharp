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
	public class BivariateNormalDistribution : BivariateDistribution
	{
		private readonly double[] _mu;
		private readonly double[][] _sigma;

		public BivariateNormalDistribution(double[] mu, double[][] sigma)
			: base()
		{
			_mu = mu;
			_sigma = sigma;
		}

		// http://mathworld.wolfram.com/BivariateNormalDistribution.html
		public override double PDF(double x0, double x1)
		{
			double s0 = System.Math.Sqrt((_sigma[0][0] * _sigma[0][0]) + (_sigma[0][1] * _sigma[0][1]));
			double s1 = System.Math.Sqrt((_sigma[1][0] * _sigma[1][0]) + (_sigma[1][1] * _sigma[1][1]));
			double rho = ((_sigma[0][0] * _sigma[1][0]) + (_sigma[0][1] * _sigma[1][1])) / (s0 * s1);
			double z = (((x0 - _mu[0]) * (x0 - _mu[0])) / (s0 * s0)) - ((2.0 * rho * (x0 - _mu[0]) * (x1 - _mu[1])) / (s0 * s1)) + (((x1 - _mu[1]) * (x1 - _mu[1])) / (s1 * s1));
			return (1.0 / (2.0 * System.Math.PI * s0 * s1 * System.Math.Sqrt(1.0 - (rho * rho)))) * System.Math.Exp(-z / (2.0 * (1.0 - (rho * rho))));
		}

	}
}
