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
	public class XAxis : Widget
	{
		private readonly float _axisTickSize;
		private readonly float _axisTickPading;
		private readonly float _xAxisAngle;
		private readonly Scales.Scale _scale;
		private readonly bool _primary;

		public XAxis(Scales.Scale scale, bool primary)
		{
			_axisTickSize = 8;
			_axisTickPading = 0;
			_xAxisAngle = 30;
			_scale = scale;
			_primary = primary;
		}

		public override void Initialize(Scripting.Frame frame, Style style)
		{
			Frame = frame;
			Style = style;

			ExpandableWidthCount = 0;
			ExpandableHeightCount = 1;

			float xAxisHeight = 0.0f;
			foreach (Scales.Label label in _scale.Labels)
				xAxisHeight = (float)System.Math.Max(xAxisHeight, System.Math.Sin(System.Math.PI * _xAxisAngle / 180.0) * Style.Graphics.MeasureString(label.Text, Style.AxisFont).Width);
			xAxisHeight += _axisTickSize + _axisTickPading;

			MinimumSize = new System.Drawing.SizeF(0, xAxisHeight);
		}

		public override void Layout(System.Drawing.RectangleF rectangle)
		{
			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, rectangle.Width, MinimumSize.Height);
		}

		public override void Render(System.Drawing.Graphics graphics, System.Drawing.RectangleF rectangle)
		{
			if (_primary)
			{
				graphics.DrawLine(Style.AxisTickPen, rectangle.X, rectangle.Y, rectangle.X + rectangle.Width, rectangle.Y);

				foreach (Scales.Label label in _scale.Labels)
				{
					float xx = _scale.AdjustX(rectangle.X, rectangle.Width, label.Value);
					graphics.DrawLine(Style.AxisTickPen, xx, rectangle.Y, xx, rectangle.Y + _axisTickSize);
					graphics.TranslateTransform(xx, rectangle.Y);
					graphics.RotateTransform(-_xAxisAngle);
					System.Drawing.SizeF size = graphics.MeasureString(label.Text, Style.AxisFont);
					System.Drawing.StringFormat stringFormat = new System.Drawing.StringFormat();
					stringFormat.LineAlignment = System.Drawing.StringAlignment.Center;
					graphics.DrawString(label.Text, Style.AxisFont, Style.ForeBrush, -size.Width, (float)(_axisTickSize / System.Math.Sin(System.Math.PI * _xAxisAngle / 180.0)), stringFormat);
					graphics.ResetTransform();
				}
			}
			else
			{
				graphics.DrawLine(Style.AxisTickPen, rectangle.X, rectangle.Y + MinimumSize.Height, rectangle.X + rectangle.Width, rectangle.Y + MinimumSize.Height);
				foreach (Scales.Label label in _scale.Labels)
				{
					float xx = _scale.AdjustX(rectangle.X, rectangle.Width, label.Value);
					graphics.DrawLine(Style.AxisTickPen, xx, rectangle.Y + MinimumSize.Height, xx, rectangle.Y + MinimumSize.Height - _axisTickSize);
					graphics.TranslateTransform(xx, rectangle.Y + MinimumSize.Height - _axisTickSize);
					graphics.RotateTransform(_xAxisAngle);
					System.Drawing.SizeF size = graphics.MeasureString(label.Text, Style.AxisFont);
					System.Drawing.StringFormat stringFormat = new System.Drawing.StringFormat();
					stringFormat.LineAlignment = System.Drawing.StringAlignment.Center;
					graphics.DrawString(label.Text, Style.AxisFont, Style.ForeBrush, -size.Width, -size.Height + (float)(_axisTickSize / System.Math.Sin(System.Math.PI * _xAxisAngle / 180.0)), stringFormat);
					graphics.ResetTransform();
				}
			}

			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, rectangle.Width, MinimumSize.Height);
		}

	}
}
