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
	public class Selector : Widget
	{
		private static readonly int LEFT_MARGIN = 8;
		private static readonly int RIGHT_MARGIN = 8;
		private static readonly int TOP_MARGIN = 8;
		private static readonly int BOTTOM_MARGIN = 8;
		private static readonly int INNER_MARGIN = 4;

		private static readonly int CIRCLE_OUTER_WIDTH = 9;
		private static readonly int CIRCLE_OUTER_HEIGHT = 9;
		private static readonly int CIRCLE_INNER_WIDTH = 7;
		private static readonly int CIRCLE_INNER_HEIGHT = 7;

		private readonly string[] _values;
		private int _index = 0;
		private readonly System.Drawing.SizeF[] _sizes;

		public Selector(string name, string[] values)
			: base(name)
		{
			_values = values;
			_sizes = new System.Drawing.SizeF[_values.Length];
		}

		public override void Initialize(Scripting.Frame frame, Style style)
		{
			Frame = frame;
			Style = style;

			if (Frame.Values.ContainsKey(Name))
				_index = System.Array.IndexOf(_values, Frame.Values[Name]);
			else
				Frame.Update(Name, _values[_index]);

			float width = 0;
			float height = TOP_MARGIN;
			for (int i = 0; i < _sizes.Length; i++)
			{
				System.Drawing.SizeF size = style.Graphics.MeasureString(_values[i], style.Font);
				_sizes[i] = size;
				width = (int)System.Math.Max(System.Math.Ceiling(size.Width), width);
				height += (int)System.Math.Max(System.Math.Ceiling(size.Height), CIRCLE_OUTER_HEIGHT) + INNER_MARGIN;
			}
			width += LEFT_MARGIN + CIRCLE_OUTER_WIDTH + INNER_MARGIN + RIGHT_MARGIN;
			height += BOTTOM_MARGIN - INNER_MARGIN;

			ExpandableWidthCount = 0;
			ExpandableHeightCount = 0;
			MinimumSize = new System.Drawing.SizeF(width, height);
		}

		public override void Click(System.Drawing.Point location)
		{
			float y = RenderedRectangle.Y + TOP_MARGIN;
			if (location.Y < y)
				return;
			for (int i = 0; i < _sizes.Length; i++)
			{
				y += (int)System.Math.Max(System.Math.Ceiling(_sizes[i].Height), CIRCLE_OUTER_HEIGHT);
				if (location.Y < y)
				{
					_index = i;
					Frame.Update(Name, _values[_index]);
					return;
				}
				y += INNER_MARGIN;
			}
		}

		//public override void MouseWheel(int delta) { if (delta > 0) { _index = System.Math.Max(0, _index - 1); } else { _index = System.Math.Min(_values.Length - 1, _index + 1); } }

		//public override void KeyDown(string key)
		//{
		//    switch (key)
		//    {
		//        case "Up":
		//            _index = System.Math.Max(0, _index - 1);
		//            break;
		//        case "Down":
		//            _index = System.Math.Min(_values.Length - 1, _index + 1);
		//            break;
		//    }
		//}

		public override void Layout(System.Drawing.RectangleF rectangle)
		{
			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, MinimumSize.Width, MinimumSize.Height);
		}

		public override void Render(System.Drawing.Graphics graphics, System.Drawing.RectangleF rectangle)
		{
			System.Drawing.Drawing2D.SmoothingMode priorSmoothingMode = graphics.SmoothingMode;
			graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			int x = (int)rectangle.X + LEFT_MARGIN;
			int y = (int)rectangle.Y + TOP_MARGIN;
			for (int i = 0; i < _values.Length; i++)
			{
				int rowHeight = (int)System.Math.Max(System.Math.Ceiling(_sizes[i].Height), CIRCLE_OUTER_HEIGHT);

				graphics.FillEllipse(Style.ForeBrush, x, y + ((rowHeight - CIRCLE_OUTER_HEIGHT) / 2), CIRCLE_OUTER_WIDTH, CIRCLE_OUTER_HEIGHT);
				if (i == _index)
					graphics.FillEllipse(Style.BackBrush, x + ((CIRCLE_OUTER_WIDTH - CIRCLE_INNER_WIDTH) / 2), y + ((rowHeight - CIRCLE_OUTER_HEIGHT) / 2) + ((CIRCLE_OUTER_HEIGHT - CIRCLE_INNER_HEIGHT) / 2), CIRCLE_INNER_WIDTH, CIRCLE_INNER_HEIGHT);
				//graphics.DrawString(_values[i], Style.TitleFont, style.ForeBrush, x + CIRCLE_OUTER_WIDTH + INNER_MARGIN, y);

				y += rowHeight + INNER_MARGIN;
			}

			graphics.SmoothingMode = priorSmoothingMode;

			x = (int)rectangle.X + LEFT_MARGIN;
			y = (int)rectangle.Y + TOP_MARGIN;
			for (int i = 0; i < _values.Length; i++)
			{
				int rowHeight = (int)System.Math.Max(System.Math.Ceiling(_sizes[i].Height), CIRCLE_OUTER_HEIGHT);

				graphics.DrawString(_values[i], Style.Font, Style.ForeBrush, x + CIRCLE_OUTER_WIDTH + INNER_MARGIN, y);

				y += rowHeight + INNER_MARGIN;
			}

			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, MinimumSize.Width, MinimumSize.Height);
		}

	}
}
