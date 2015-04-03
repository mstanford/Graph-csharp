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
	public class MedianOfMedians : Function
	{
		private readonly int _n;
		private readonly List<double> _currentValues;
		private readonly List<double> _medians;
		private readonly List<double> _temp;

		public MedianOfMedians()
		{
			_n = 100;
			_currentValues = new List<double>();
			_medians = new List<double>();
			_temp = new List<double>();

			Clear();
		}

		public override void Clear()
		{
			_currentValues.Clear();
			_medians.Clear();
			_temp.Clear();
		}

		public override double Update(double x)
		{
			_currentValues.Add(x);
			if (_currentValues.Count == _n)
			{
				_medians.Add(ReduceList(_currentValues));
				_currentValues.Clear();
			}
			if (_medians.Count == 0)
				return ReduceList(_currentValues);
			_temp.Clear();
			_temp.AddRange(_medians);
			if (_currentValues.Count > 0)
				_temp.Add(ReduceList(_currentValues));
			return ReduceList(_temp);
		}

		private double ReduceList(List<double> X)
		{
			X.Sort();
			return (X.Count % 2 == 1)
				? X[X.Count / 2]
				: ((X[X.Count / 2] + X[(X.Count / 2) - 1]) / 2.0);
		}

		public override double Reduce(double[] X)
		{
			System.Array.Sort(X);
			return (X.Length % 2 == 1)
				? X[X.Length / 2]
				: ((X[X.Length / 2] + X[(X.Length / 2) - 1]) / 2.0);
		}

	}
}
