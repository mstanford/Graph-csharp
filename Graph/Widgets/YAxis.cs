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
	public class YAxis : Widget
	{
		private readonly float _axisTickSize;
		private readonly float _axisTickPading;
		private readonly Scales.Scale _scale;
		private readonly bool _primary;

		public YAxis(Scales.Scale scale, bool primary)
		{
			_axisTickSize = 8;
			_axisTickPading = 0;
			_scale = scale;
			_primary = primary;
		}

		public override void Initialize(Scripting.Frame frame, Style style)
		{
			Frame = frame;
			Style = style;

			ExpandableWidthCount = 0;
			ExpandableHeightCount = 1;

			float yAxisWidth = 0.0f;
			if (!double.IsNaN(_scale.Max))
			{
				foreach (Scales.Label label in _scale.Labels)
					yAxisWidth = (float)System.Math.Max(yAxisWidth, Style.Graphics.MeasureString(label.Text, Style.AxisFont).Width);
				yAxisWidth += _axisTickSize + _axisTickPading;
			}

			MinimumSize = new System.Drawing.SizeF(yAxisWidth, 0);
		}

		public override void Layout(System.Drawing.RectangleF rectangle)
		{
			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, MinimumSize.Width, rectangle.Height);
		}

		public override void Render(System.Drawing.Graphics graphics, System.Drawing.RectangleF rectangle)
		{
			if (!double.IsNaN(_scale.Max))
			{
				if (_primary)
				{
					graphics.DrawLine(Style.AxisTickPen, rectangle.X + MinimumSize.Width, rectangle.Y, rectangle.X + MinimumSize.Width, rectangle.Y + rectangle.Height);
					foreach (Scales.Label label in _scale.Labels)
					{
						float yy = _scale.AdjustY(rectangle.Y, rectangle.Height, label.Value);
						graphics.DrawLine(Style.AxisTickPen, rectangle.X + MinimumSize.Width - _axisTickSize, yy, rectangle.X + MinimumSize.Width, yy);
						System.Drawing.SizeF size = graphics.MeasureString(label.Text, Style.AxisFont);
						System.Drawing.StringFormat stringFormat = new System.Drawing.StringFormat();
						stringFormat.LineAlignment = System.Drawing.StringAlignment.Center;
						graphics.DrawString(label.Text, Style.AxisFont, Style.ForeBrush, rectangle.X + MinimumSize.Width - MinimumSize.Width + (MinimumSize.Width - _axisTickSize - size.Width), yy, stringFormat);
					}
				}
				else
				{
					graphics.DrawLine(Style.AxisTickPen, rectangle.X, rectangle.Y, rectangle.X, rectangle.Y + rectangle.Height);
					foreach (Scales.Label label in _scale.Labels)
					{
						float yy = _scale.AdjustY(rectangle.Y, rectangle.Height, label.Value);
						graphics.DrawLine(Style.AxisTickPen, rectangle.X, yy, rectangle.X + _axisTickSize, yy);
						System.Drawing.StringFormat stringFormat = new System.Drawing.StringFormat();
						stringFormat.LineAlignment = System.Drawing.StringAlignment.Center;
						graphics.DrawString(label.Text, Style.AxisFont, Style.ForeBrush, rectangle.X + _axisTickSize + 1, yy, stringFormat);
					}
				}
			}

			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, MinimumSize.Width, rectangle.Height);
		}

	}
}
