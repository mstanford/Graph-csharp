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

namespace Graph.Widgets.Scales
{
	public class LinearScale : Scale
	{

		protected LinearScale(double max, double min, int ticks, string format) : base(Create(max, min, ticks, format), max, min) { }

		//public static LinearScale X(params System.Drawing.PointF[][] points)
		//{
		//    int xTicks = 7;
		//    string xAxisFormat = "N2";
		//    return new LinearScale(MaxX(points), MinX(points), xTicks, xAxisFormat);
		//}

		public static LinearScale X(double[] values)
		{
			int xTicks = 7;
			string xAxisFormat = "N2";

			double min = double.NaN;
			double max = double.NaN;
			for (int j = 0; j < values.Length; j++)
			{
				if (double.IsNaN(values[j]))
					continue;

				if (double.IsNaN(max))
				{
					min = values[j];
					max = values[j];
				}
				else
				{
					min = System.Math.Min(min, values[j]);
					max = System.Math.Max(max, values[j]);
				}
			}
			//double min = values[0];
			//double max = values[0];
			//for (int j = 1; j < values.Length; j++)
			//{
			//    min = System.Math.Min(min, values[j]);
			//    max = System.Math.Max(max, values[j]);
			//}

			if (double.IsNaN(max))
			{
				max = 1.0; //TODO DELETE
				min = 0.0; //TODO DELETE
				//throw new System.Exception();
			}

			return new LinearScale(max, min, xTicks, xAxisFormat);
		}

		public static LinearScale X2(double max, double min)
		{
			int xTicks = 7;
			string xAxisFormat = "N2";
			return new LinearScale(max, min, xTicks, xAxisFormat);
		}

		public static LinearScale X3(double max, double min, string xAxisFormat)
		{
			int xTicks = 7;
			return new LinearScale(max, min, xTicks, xAxisFormat);
		}

		public static LinearScale XX(params double[][] values)
		{
			LinearScale[] scales = new LinearScale[values.Length];
			for (int i = 0; i < values.Length; i++)
				scales[i] = LinearScale.X(values[i]);
			double max = scales[0].Max;
			double min = scales[0].Min;
			for (int i = 1; i < values.Length; i++)
			{
				if (scales[i].Labels != null)
				{
					max = System.Math.Max(max, scales[i].Max);
					min = System.Math.Min(min, scales[i].Min);
				}
			}

			int xTicks = 7;
			string xAxisFormat = "N2";
			return new LinearScale(max, min, xTicks, xAxisFormat);
		}

		//public static LinearScale Y(params System.Drawing.PointF[][] points)
		//{
		//    int yTicks = 7;
		//    string yAxisFormat = "N2";
		//    return new LinearScale(MaxY(points), MinY(points), yTicks, yAxisFormat);
		//}

		//public static LinearScale Y(double min, params System.Drawing.PointF[][] points)
		//{
		//    int yTicks = 7;
		//    string yAxisFormat = "N2";
		//    return new LinearScale(MaxY(points), System.Math.Min(min, MinY(points)), yTicks, yAxisFormat);
		//}

		public static LinearScale Y(double[] values)
		{
			int yTicks = 7;
			string yAxisFormat = "N2";

			double min = double.NaN;
			double max = double.NaN;
			for (int j = 0; j < values.Length; j++)
			{
				if (double.IsNaN(values[j]))
					continue;

				if (double.IsNaN(max))
				{
					min = values[j];
					max = values[j];
				}
				else
				{
					min = System.Math.Min(min, values[j]);
					max = System.Math.Max(max, values[j]);
				}
			}
			//double min = values[0];
			//double max = values[0];
			//for (int j = 1; j < values.Length; j++)
			//{
			//    min = System.Math.Min(min, values[j]);
			//    max = System.Math.Max(max, values[j]);
			//}

			if (double.IsNaN(max))
			{
				max = 1.0; //TODO DELETE
				min = 0.0; //TODO DELETE
				//throw new System.Exception();
			}

			return new LinearScale(max, min, yTicks, yAxisFormat);
		}

		public static LinearScale Y2(double max, double min)
		{
			int yTicks = 7;
			string yAxisFormat = "N2";
			return new LinearScale(max, min, yTicks, yAxisFormat);
		}

		public static LinearScale Y3(double max, double min, string yAxisFormat)
		{
			int yTicks = 7;
			return new LinearScale(max, min, yTicks, yAxisFormat);
		}

		public static LinearScale YY(params double[][] values)
		{
			LinearScale[] scales = new LinearScale[values.Length];
			for (int i = 0; i < values.Length; i++)
				scales[i] = LinearScale.Y(values[i]);
			double max = scales[0].Max;
			double min = scales[0].Min;
			for (int i = 1; i < values.Length; i++)
			{
				if (scales[i].Labels != null)
				{
					max = System.Math.Max(max, scales[i].Max);
					min = System.Math.Min(min, scales[i].Min);
				}
			}

			int yTicks = 7;
			string yAxisFormat = "N2";
			return new LinearScale(max, min, yTicks, yAxisFormat);
		}

		public static Label[] Create(double max, double min, int ticks, string format)
		{
			//TODO infer the format from the scale.
			float scale = Calculate(max, min, ticks);
			if (float.IsNaN(scale))
				return null;
			List<Label> labels = new List<Label>();
			for (float i = (float)System.Math.Ceiling(min / scale) * scale; i <= max; i += System.Math.Abs(scale))
				labels.Add(new Label(i, i.ToString(format)));
			return labels.ToArray();
		}

		public static float Calculate(double max, double min, int ticks)
		{
			if (max == min)
				return (float)max;
			double scale = System.Math.Pow(10, System.Math.Floor(System.Math.Log10(max - min)) - 1);
			double[] scales = new double[] { 
                (scale * 20.0), 
                (scale * 10.0), 
                (scale * 5.0), 
                (scale * 2.0), 
                scale };
			for (int i = 0; i < scales.Length; i++)
				if (((max - min) / scales[i]) > ticks)
					return (float)scales[i - 1];
			return float.NaN;
		}

		//private static float MaxX(System.Drawing.PointF[][] points)
		//{
		//    float Max = points[0][0].X;
		//    for (int i = 0; i < points.Length; i++)
		//        for (int j = 0; j < points[i].Length; j++)
		//            Max = System.Math.Max(Max, points[i][j].X);
		//    return Max;
		//}

		//private static float MinX(System.Drawing.PointF[][] points)
		//{
		//    float Min = points[0][0].X;
		//    for (int i = 0; i < points.Length; i++)
		//        for (int j = 0; j < points[i].Length; j++)
		//            Min = System.Math.Min(Min, points[i][j].X);
		//    return Min;
		//}

		//private static float MaxY(System.Drawing.PointF[][] points)
		//{
		//    float Max = points[0][0].Y;
		//    for (int i = 0; i < points.Length; i++)
		//        for (int j = 0; j < points[i].Length; j++)
		//            Max = System.Math.Max(Max, points[i][j].Y);
		//    return Max;
		//}

		//private static float MinY(System.Drawing.PointF[][] points)
		//{
		//    float Min = points[0][0].Y;
		//    for (int i = 0; i < points.Length; i++)
		//        for (int j = 0; j < points[i].Length; j++)
		//            Min = System.Math.Min(Min, points[i][j].Y);
		//    return Min;
		//}

	}
}
