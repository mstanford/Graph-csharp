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
	public class LogarithmicScale : Scale
	{

        private LogarithmicScale(double max, double min, int ticks, string format) : base(Create(max, min, ticks, format), max, min) { }

		public static LogarithmicScale Y(double[] values)
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
					min = Log10(values[j]);
					max = Log10(values[j]);
				}
				else
				{
					min = System.Math.Min(min, Log10(values[j]));
					max = System.Math.Max(max, Log10(values[j]));
				}
			}
			return new LogarithmicScale(max, min, yTicks, yAxisFormat);
		}

		private static double Log10(double d)
		{
			if (d > 0)
				return System.Math.Log10(d);
			return -System.Math.Log10(-d);
		}

		public static LogarithmicScale Y2(double max, double min)
		{
			int yTicks = 7;
			string yAxisFormat = "N2";
			return new LogarithmicScale(Log10(max), Log10(min), yTicks, yAxisFormat);
		}

		public static LogarithmicScale Y3(double max, double min, string yAxisFormat)
		{
			int yTicks = 7;
			return new LogarithmicScale(Log10(max), Log10(min), yTicks, yAxisFormat);
		}

		public static LogarithmicScale YY(params double[][] values)
		{
			LogarithmicScale[] scales = new LogarithmicScale[values.Length];
			for (int i = 0; i < values.Length; i++)
				scales[i] = LogarithmicScale.Y(values[i]);
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
			return new LogarithmicScale(max, min, yTicks, yAxisFormat);
		}

		//public override float AdjustX(float x, float width, float value)
		//{
		//    throw new System.Exception();
		//}

		//public override float AdjustY(float y, float height, float value)
		//{
		//    float yy = base.AdjustY(y, height, (float)System.Math.Exp(value));
		//    if (float.IsInfinity(yy))
		//        throw new System.Exception();
		//    return yy;
		//}

		private static Label[] Create(double max, double min, int ticks, string format)
		{
			Label[] labels = LinearScale.Create(max, min, ticks, format);
			if (labels == null)
				return null;
			for (int i = 0; i < labels.Length; i++)
				labels[i] = new Label(labels[i].Value, System.Math.Pow(10.0, labels[i].Value).ToString(format));
			return labels;
		}

	}

	/*
	public class LogarithmicScale : Scale
	{

		private LogarithmicScale(double max, double min, int ticks, string format) : base(Create(System.Math.Pow(10, max), System.Math.Pow(10, min), ticks, format), max, min) { }

		//public static LogarithmicScale X(params System.Drawing.PointF[][] points)
		//{
		//    int xTicks = configuration.ContainsKey("XTicks") ? int.Parse(configuration["XTicks"]) : 7;
		//    string xAxisFormat = configuration.ContainsKey("XAxisFormat") ? configuration["XAxisFormat"] : "N2";
		//    return new LogarithmicScale(MaxX(points), MinX(points), xTicks, xAxisFormat);
		//}

		//public static LogarithmicScale X(double max, double min)
		//{
		//    int xTicks = configuration.ContainsKey("XTicks") ? int.Parse(configuration["XTicks"]) : 7;
		//    string xAxisFormat = configuration.ContainsKey("XAxisFormat") ? configuration["XAxisFormat"] : "N2";
		//    return new LogarithmicScale(max, min, xTicks, xAxisFormat);
		//}

		//public static LogarithmicScale Y(params System.Drawing.PointF[][] points)
		//{
		//    int yTicks = 7;
		//    string yAxisFormat = "N2";
		//    return new LogarithmicScale(MaxY(points), MinY(points), yTicks, yAxisFormat);
		//}

		//public static LogarithmicScale Y(double min, params System.Drawing.PointF[][] points)
		//{
		//    int yTicks = configuration.ContainsKey("YTicks") ? int.Parse(configuration["YTicks"]) : 7;
		//    string yAxisFormat = configuration.ContainsKey("YAxisFormat") ? configuration["YAxisFormat"] : "N2";
		//    return new LogarithmicScale(MaxY(points), System.Math.Min(min, MinY(points)), yTicks, yAxisFormat);
		//}

		public static LogarithmicScale Y(double max, double min)
		{
			int yTicks = 7;
			string yAxisFormat = "N2";
			return new LogarithmicScale(max, min, yTicks, yAxisFormat);
		}

		public override float AdjustX(float x, float width, float value)
		{
			throw new System.Exception();
			//return base.AdjustX(x, width, (float)System.Math.Log10(value));
		}

		public override float AdjustY(float y, float height, float value)
		{
			return base.AdjustY(y, height, (float)System.Math.Log10(value));
		}

		private static Label[] Create(double max, double min, int ticks, string format)
		{
			//TODO infer the format from the scale.
			double scale = Calculate(max, min, ticks);
			List<Label> labels = new List<Label>();
			for (double i = System.Math.Ceiling(min / scale) * scale; i <= max; i += scale)
				labels.Add(new Label((float)i, i.ToString(format)));
			return labels.ToArray();
		}

		private static double Calculate(double max, double min, int ticks)
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
			throw new System.Exception();
		}

		//private static float MaxX(System.Drawing.PointF[][] points)
		//{
		//    float Max = (float)System.Math.Log10(points[0][0].X);
		//    for (int i = 0; i < points.Length; i++)
		//        for (int j = 0; j < points[i].Length; j++)
		//            Max = System.Math.Max(Max, System.Math.Log10(points[i][j].X));
		//    return Max;
		//}

		//private static float MinX(System.Drawing.PointF[][] points)
		//{
		//    float Min = (float)System.Math.Log10(points[0][0].X);
		//    for (int i = 0; i < points.Length; i++)
		//        for (int j = 0; j < points[i].Length; j++)
		//            Min = System.Math.Min(Min, System.Math.Log10(points[i][j].X));
		//    return Min;
		//}

		//private static float MaxY(System.Drawing.PointF[][] points)
		//{
		//    float Max = (float)System.Math.Log10(points[0][0].Y);
		//    for (int i = 0; i < points.Length; i++)
		//        for (int j = 0; j < points[i].Length; j++)
		//            Max = System.Math.Max(Max, (float)System.Math.Log10(points[i][j].Y));
		//    return Max;
		//}

		//private static float MinY(System.Drawing.PointF[][] points)
		//{
		//    float Min = (float)System.Math.Log10(points[0][0].Y);
		//    for (int i = 0; i < points.Length; i++)
		//        for (int j = 0; j < points[i].Length; j++)
		//            Min = System.Math.Min(Min, (float)System.Math.Log10(points[i][j].Y));
		//    return Min;
		//}

	}
	*/
}
