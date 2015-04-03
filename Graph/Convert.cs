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

namespace Graph
{
	public class Convert
	{

		public static int ToInt32(object o)
		{
			if (o is string) return ToInt32((string)o);
			return System.Convert.ToInt32(o);
		}

		public static int ToInt32(string s)
		{
			if (s == null) throw new System.Exception();
			return System.Convert.ToInt32(s);
		}

		public static long ToInt64(object o)
		{
			if (o is string) return ToInt64((string)o);
			return System.Convert.ToInt64(o);
		}

		public static long ToInt64(string s)
		{
			if (s == null) throw new System.Exception();
			return System.Convert.ToInt64(s);
		}

		public static float ToSingle(object o)
		{
			if (o is string) return ToSingle((string)o);
			return System.Convert.ToSingle(o);
		}

		public static float ToSingle(string s)
		{
			if (s == null) throw new System.Exception();
			return System.Convert.ToSingle(s);
		}

		public static double ToDouble(object o)
		{
			if (o is string) return ToDouble((string)o);
			return System.Convert.ToDouble(o);
		}

		public static double ToDouble(string s)
		{
			if (s == null) throw new System.Exception();
			return System.Convert.ToDouble(s);
		}

		public static decimal ToDecimal(object o)
		{
			if (o is string) return ToDecimal((string)o);
			return System.Convert.ToDecimal(o);
		}

		public static decimal ToDecimal(string s)
		{
			if (s == null) throw new System.Exception();
			return System.Convert.ToDecimal(s);
		}

		public static string ToString(double d, string format)
		{
			if (double.IsNaN(d))
				return "";
			return d.ToString(format);
		}

		public static System.Drawing.PointF[] ToPoints(double[] values)
		{
			List<System.Drawing.PointF> points = new List<System.Drawing.PointF>();
			//System.Drawing.PointF[] points = new System.Drawing.PointF[values.Length];
			for (int i = 0; i < values.Length; i++)
				if (!double.IsNaN(values[i]))
					points.Add(new System.Drawing.PointF(i, (float)values[i]));
			return points.ToArray();
		}

		public static System.Drawing.PointF[] ToPoints2(double[] X, double[] Y)
		{
			System.Drawing.PointF[] points = new System.Drawing.PointF[X.Length];
			for (int i = 0; i < points.Length; i++)
				points[i] = new System.Drawing.PointF((float)X[i], (float)Y[i]);
			return points;
		}

	}
}
