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
	public class NormalDistribution : Distribution
	{
        public readonly double Mu;
		public readonly double Sigma;

		public NormalDistribution(double mu, double sigma)
            : base()
        {
			Mu = mu;
			Sigma = sigma;
        }

		public override double CDF(double x)
        {
			return 0.5 * (1.0 + erf((x - Mu) / (System.Math.Sqrt(2.0) * Sigma)));
        }

		public override double Entropy()
		{
			// The entropy of a normal distribution of a given standard deviation.
			//double h = 0.5 * System.Math.Log(2 * System.Math.PI * System.Math.E * Sigma * Sigma, System.Math.E);
			//return h * (System.Math.Log(System.Math.E) / System.Math.Log(2)); // Convert to base 2.
			return 0.5 * System.Math.Log(2 * System.Math.PI * System.Math.E * Sigma * Sigma, 2);
		}

		public override double Inverse(double p)
		{
			return Mu + (Sigma * inv(p));
		}

		public override double Mean()
		{
			return Mu;
		}

		public override double Median()
		{
			return Mu;
		}

		public override double Mode()
		{
			return Mu;
		}

		public override double PDF(double x)
		{
			return (1.0 / (System.Math.Sqrt(2.0 * System.Math.PI) * Sigma)) * System.Math.Exp(-(((x - Mu) * (x - Mu)) / (2 * Sigma * Sigma)));
		}

		public override double Variance()
		{
			return Sigma * Sigma;
		}

		//
        // FROM http://home.online.no/~pjacklam/notes/invnorm/
        //
        // Lower tail quantile for standard normal distribution function.
        //
        // This function returns an approximation of the inverse cumulative
        // standard normal distribution function.  I.e., given P, it returns
        // an approximation to the X satisfying P = Pr{Z <= X} where Z is a
        // random variable from the standard normal distribution.
        //
        // The algorithm uses a minimax approximation by rational functions
        // and the result has a relative error whose absolute value is less
        // than 1.15e-9.
        //
        // Author:      Peter J. Acklam
        // Time-stamp:  2003-05-05 05:15:14
        // E-mail:      pjacklam@online.no
        // WWW URL:     http://home.online.no/~pjacklam
        //
        // An algorithm with a relative error less than 1.15·10-9 in the entire region.
        //

		// Coefficients in rational approximations
		private static readonly double[] a = new double[] { -3.969683028665376e+01,  2.209460984245205e+02, -2.759285104469687e+02,  1.383577518672690e+02, -3.066479806614716e+01,  2.506628277459239e+00 };
		private static readonly double[] b = new double[] { -5.447609879822406e+01,  1.615858368580409e+02, -1.556989798598866e+02,  6.680131188771972e+01, -1.328068155288572e+01 };
		private static readonly double[] c = new double[] { -7.784894002430293e-03, -3.223964580411365e-01, -2.400758277161838e+00, -2.549732539343734e+00, 4.374664141464968e+00,  2.938163982698783e+00 };
		private static readonly double[] d = new double[] { 7.784695709041462e-03, 3.224671290700398e-01, 2.445134137142996e+00,  3.754408661907416e+00 };

		public static double inv(double p)
        {
            // Define break-points.
            double plow = 0.02425;
            double phigh = 1 - plow;

            if (p < plow)
            {
                // Rational approximation for lower region:
                double q = System.Math.Sqrt(-2 * System.Math.Log(p));
                return (((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) /
                        ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1);
            }
            else if (phigh < p)
            {
                // Rational approximation for upper region:
                double q = System.Math.Sqrt(-2 * System.Math.Log(1 - p));
                return -(((((c[0] * q + c[1]) * q + c[2]) * q + c[3]) * q + c[4]) * q + c[5]) /
                        ((((d[0] * q + d[1]) * q + d[2]) * q + d[3]) * q + 1);
            }
            else
            {
                // Rational approximation for central region:
                double q = p - 0.5;
                double r = q * q;
                return (((((a[0] * r + a[1]) * r + a[2]) * r + a[3]) * r + a[4]) * r + a[5]) * q /
                        (((((b[0] * r + b[1]) * r + b[2]) * r + b[3]) * r + b[4]) * r + 1);
            }
        }

	}
}
