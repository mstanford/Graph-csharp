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

namespace Graph.Widgets
{
	public class ColumnChart : Chart
	{
		private readonly int _i;
		private readonly int _n;
		private readonly string _positiveColor;
		private readonly string _negativeColor;
		private readonly System.Drawing.Brush _positiveBrush;
		private readonly System.Drawing.Brush _negativeBrush;
		private readonly System.Drawing.PointF[] _points;

		public ColumnChart(
			int i,
			int n, 
			string positiveColor,
			string negativeColor,
			System.Drawing.PointF[] points,
			Scales.Scale xScale,
			Scales.Scale yScale)
			: base(xScale, yScale)
		{
			_i = i;
			_n = n;
			_positiveColor = positiveColor;
			_negativeColor = negativeColor;
			_positiveBrush = Style.Palette.Brush(_positiveColor);
			_negativeBrush = Style.Palette.Brush(_negativeColor);
			_points = points;
		}

		public override void Initialize(Scripting.Frame frame, Style style)
		{
			Frame = frame;
			Style = style;

			ExpandableWidthCount = 1;
			ExpandableHeightCount = 1;

			MinimumSize = new System.Drawing.SizeF(0, 0);
		}

		public override void Layout(System.Drawing.RectangleF rectangle)
		{
			RenderedRectangle = rectangle;
		}

		public override void Render(System.Drawing.Graphics graphics, System.Drawing.RectangleF rectangle)
		{
			float spacing = 2f;
			float x_unit = (rectangle.Width / (_points.Length - 1)) - spacing;
			x_unit /= _n;

			float y0 = System.Math.Min(rectangle.Y + rectangle.Height, YScale.AdjustY(rectangle.Y, rectangle.Height, 0));
			System.Drawing.PointF[] points = Adjust(_points, rectangle);
			for (int i = 0; i < points.Length; i++)
			{
				if (points[i].Y < y0)
					graphics.FillRectangle(_positiveBrush, points[i].X - (x_unit / 2.0f) + (x_unit * _i), points[i].Y + 1, x_unit, y0 - points[i].Y - 1);
				if (points[i].Y > y0)
					graphics.FillRectangle(_negativeBrush, points[i].X - (x_unit / 2.0f) + (x_unit * _i), y0 + 1, x_unit, points[i].Y - y0 - 1);
			}

			RenderedRectangle = rectangle;
		}

	}
}
