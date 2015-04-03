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
	public class LogNormalDistribution : Distribution
	{
        private readonly double _mu;
        private readonly double _sigma;

		public LogNormalDistribution(double muLog, double sigmaLog)
			: base()
        {
			_mu = muLog;
			_sigma = sigmaLog;
        }

		public override double CDF(double x)
		{
			return 0.5 + (0.5 * erf((System.Math.Log(x) - _mu) / (System.Math.Sqrt(2.0) * _sigma)));
		}

		public override double Entropy()
		{
			return 0.5 + (0.5 * System.Math.Log(2 * System.Math.PI * _sigma * _sigma, 2)) + _mu;
		}

		public override double Inverse(double p)
		{
			return System.Math.Exp(_mu + (_sigma * NormalDistribution.inv(p)));
		}

		public override double Mean()
		{
			//TODO I think this one is correct:  return System.Math.Exp(_mu - _sigma * _sigma / 2.0) - 1.0;
			return System.Math.Exp(_mu + 0.5 * _sigma * _sigma);
		}

		public override double Median()
		{
			return System.Math.Exp(_mu);
		}

		public override double Mode()
		{
			return System.Math.Exp(_mu - _sigma * _sigma);
		}

		public override double PDF(double x)
		{
			double pdf = (1.0 / (x * System.Math.Sqrt(2.0 * System.Math.PI) * _sigma)) * System.Math.Exp(-((System.Math.Log(x) - _mu) * (System.Math.Log(x) - _mu)) / (2.0 * _sigma * _sigma));
			if (double.IsNaN(pdf))
				return 0.0;
			return pdf;
		}

		public override double Variance()
		{
			return (System.Math.Exp(_sigma * _sigma) - 1.0) * System.Math.Exp(2.0 * _mu + _sigma * _sigma);
		}

	}
}
