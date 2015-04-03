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

namespace Graph.Functions.Statistics
{
	public class CumulativeCovariance : Function
	{
		private double _sum_x;
		private double _sum_y;
		private double _sum_xy;
		private double _n;

		public CumulativeCovariance()
		{
			Clear();
		}

		public override void Clear()
		{
			_sum_x = 0;
			_sum_y = 0;
			_sum_xy = 0;
			_n = 0;

			Count = 0;
			Value = double.NaN;
		}

		public override double Update2(double x, double y)
		{
			if (double.IsNaN(x) || double.IsNaN(y))
				return double.NaN;
			// Naive covariance, numerically unstable.
			_sum_x += x;
			_sum_y += y;
			_sum_xy += x * y;
			_n++;
			Count++;
			Value = (_sum_xy - (_sum_x * _sum_y / _n)) / (_n - 1);
			return Value;
		}

		//public static double Covariance(double[] x, double[] y) { double x_mean = Mean(x); double y_mean = Mean(y); double sum = 0; for (int i = 0; i < x.Length; i++) { double xx = x[i] - x_mean; double yy = y[i] - y_mean; sum += xx * yy; } return sum / x.Length; }

		//private double _sum_xy;
		//private double _n;
		//private CumulativeMean _mu_x = new CumulativeMean();
		//private CumulativeMean _mu_y = new CumulativeMean();

		//public override double Next(double x, double y)
		//{
		//    _sum_xy += x * y;
		//    _n++;
		//    double mu_x = _mu_x.Next(x);
		//    double mu_y = _mu_y.Next(y);
		//    Value = (_sum_xy - (mu_x * mu_y / _n)) / (_n - 1);
		//    return Value;
		//}

		/*
def two_pass_covariance(data1, data2):
	n = len(data1)
 
	mean1 = sum(data1) / n
	mean2 = sum(data2) / n	
 
	covariance = 0
 
	for i in range(n):
		a = data1[i] - mean1		
		b = data2[i] - mean2
		covariance += a*b / n
 
	return covariance
		*/

		//public static double Calculate(double[] X, double[] Y)
		//{
		//    int n = X.Length;
		//    double mean_x = Mean.Calculate(X);
		//    double mean_y = Mean.Calculate(Y);
		//    double covariance = 0;
		//    for (int i = 0; i < X.Length; i++)
		//        covariance += (X[i] - mean_x) * (Y[i] - mean_y);
		//    return covariance / (n - 1);
		//}

	}
}
