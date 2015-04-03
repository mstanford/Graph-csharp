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
	public class ScatterChart : Chart
	{
		private readonly string _color;
		private readonly int _pointWidth;
		private readonly System.Drawing.Pen _pointPen;
		private readonly System.Drawing.PointF[] _points;

		public ScatterChart(
			string color,
			System.Drawing.PointF[] points,
			Scales.Scale xScale,
			Scales.Scale yScale)
			: base(xScale, yScale)
		{
			_color = color;
			_pointWidth = 2;
			_pointPen = Style.Palette.Pen(_color, _pointWidth);
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
			System.Drawing.PointF[] points = Adjust(_points, rectangle);
			for (int i = 0; i < points.Length; i++)
				graphics.DrawEllipse(_pointPen, points[i].X - (_pointWidth / 2.0f), points[i].Y - (_pointWidth / 2.0f), _pointWidth, _pointWidth);

			RenderedRectangle = rectangle;
		}

	}
}
