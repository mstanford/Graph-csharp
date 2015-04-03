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
	public class GaussianKernelDistribution : KernelDistribution
	{
		private readonly double _mu;
		private readonly double _sigma;
		private readonly double _min;
		private readonly double _increment;

		public GaussianKernelDistribution(double[] A)
		{
			List<double> B = new List<double>();
			for (int i = 0; i < A.Length; i++)
				if (!double.IsNaN(A[i]))
					B.Add(A[i]);
			A = B.ToArray(); //Remove NaNs

			int n = 200;



			_mu = Graph.Functions.Statistics.CumulativeMean.Calculate(A);
			_sigma = Graph.Functions.Statistics.CumulativeStandardDeviation.Calculate(A);

			// Silverman's Rule of Thumb
			double h = System.Math.Pow((4.0 * System.Math.Pow(_sigma, 5.0)) / (3.0 * A.Length), 0.2);

			//double max = A[0];
			//double min = A[0];
			//for (int i = 1; i < A.Length; i++)
			//{
			//    max = System.Math.Max(max, A[i]);
			//    min = System.Math.Min(min, A[i]);
			//}

			double max = _mu + (4.0 * _sigma);
			_min = _mu - (4.0 * _sigma);
			_increment = (max - _min) / (n - 1);

			X = new double[n];
			Y = new double[X.Length];
			YY = new double[X.Length];
			double total = 0.0;
			for (int i = 0; i < X.Length; i++)
			{
				X[i] = _min + (i * _increment);
				Y[i] = GaussianKernel(A, h, X[i]);
				total += Y[i];
			}
			for (int i = 0; i < Y.Length; i++)
			{
				Y[i] /= total; // Normalize
				YY[i] = Y[i];
				if (i > 0)
					YY[i] += YY[i - 1];
			}

			//for (int i = 0; i < _X.Length; i++)
			//    System.Console.Write(_X[i] + "\t" + _Y[i] + "\n");
		}

		public override double CDF(double x)
		{
			int i = (int)System.Math.Floor((x - _min) / _increment);
			if (x >= X[i] && x <= X[i + 1])
				return YY[i] + (x - X[i]) * (YY[i + 1] - YY[i]) / (X[i + 1] - X[i]);
			throw new Exception("The method or operation is not implemented.");
		}

		public override double Inverse(double p)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override double Mean()
		{
			return _mu;
		}

		public override double Median()
		{
			return _mu;
		}

		public override double Mode()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override double PDF(double x)
		{
			int i = (int)System.Math.Floor((x - _min) / _increment);
			if (x >= X[i] && x <= X[i + 1])
				return Y[i] + (x - X[i]) * (Y[i + 1] - Y[i]) / (X[i + 1] - X[i]);
			throw new Exception("The method or operation is not implemented.");
		}

		public override double Variance()
		{
			return _sigma * _sigma;
		}

		public static double GaussianKernel(
			double[] X, // The array of sample data points.
			double h,   // The bandwidth
			double x)   // The point at which to calculate the estimate
		{
			double k = 0.0;
			for (int i = 0; i < X.Length; i++)
				k += (1.0 / System.Math.Sqrt(2.0 * System.Math.PI)) * System.Math.Exp(-0.5 * Square((X[i] - x) / h)); // Standard normal distribution
			return k;
		}

		private static double Square(double a) { return a * a; }

	}
}
