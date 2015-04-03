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
	public class HorizontalSlider : Widget
	{
		private static readonly int LEFT_MARGIN = 8;
		private static readonly int RIGHT_MARGIN = 8;
		private static readonly int TOP_MARGIN = 8;
		private static readonly int BOTTOM_MARGIN = 8;
		private static readonly int SLIDER_WIDTH = 5;
		private static readonly int SLIDER_HEIGHT = 13;
		private static readonly int TICK_SPACE = 2;
		private static readonly int TICK_HEIGHT = 2;

		private readonly string[] _values;
		private int _index = 0;
		private bool _isMouseDown;

		public HorizontalSlider(string name, string[] values)
			: base(name)
		{
			_values = values;

			ExpandableWidthCount = 1;
			ExpandableHeightCount = 0;
			MinimumSize = new System.Drawing.SizeF(0, TOP_MARGIN + SLIDER_HEIGHT + TICK_SPACE + TICK_HEIGHT + BOTTOM_MARGIN);
		}

		public override void Initialize(Scripting.Frame frame, Style style)
		{
			Frame = frame;
			Style = style;

			if (Frame.Values.ContainsKey(Name))
				_index = System.Array.IndexOf(_values, Frame.Values[Name]);
			else
				Frame.Update(Name, _values[_index]);
		}

		public override void MouseDown(System.Drawing.Point location) { _isMouseDown = true; SetValue(location); }
		public override void MouseUp(System.Drawing.Point location) { _isMouseDown = false; }
		public override void MouseMove(System.Drawing.Point location) { if (_isMouseDown) { SetValue(location); } }
		//public override void MouseWheel(int delta) { if (delta > 0) { _index = System.Math.Max(0, _index - 1); } else { _index = System.Math.Min(_values.Length - 1, _index + 1); } }
		//public override void MouseLeave() { _isMouseDown = false; }

		private void SetValue(System.Drawing.Point location)
		{
			float increment = (float)(RenderedRectangle.Width - LEFT_MARGIN - RIGHT_MARGIN) / (float)(_values.Length - 1);
			_index = (int)System.Math.Round((location.X - RenderedRectangle.X - LEFT_MARGIN) / increment);
			_index = System.Math.Max(0, _index);
			_index = System.Math.Min(_values.Length - 1, _index);
			Frame.Update(Name, _values[_index]);
		}

		public override void KeyDown(string key)
		{
			switch (key)
			{
				case "Left":
					_index = System.Math.Max(0, _index - 1);
					Frame.Update(Name, _values[_index]);
					break;
				case "Right":
					_index = System.Math.Min(_values.Length - 1, _index + 1);
					Frame.Update(Name, _values[_index]);
					break;
				case "Home":
					_index = 0;
					Frame.Update(Name, _values[_index]);
					break;
				case "End":
					_index = _values.Length - 1;
					Frame.Update(Name, _values[_index]);
					break;
			}
		}

		public override void Layout(System.Drawing.RectangleF rectangle)
		{
			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, rectangle.Width, MinimumSize.Height);
		}

		public override void Render(System.Drawing.Graphics graphics, System.Drawing.RectangleF rectangle)
		{
			float increment = (float)(rectangle.Width - LEFT_MARGIN - RIGHT_MARGIN) / (float)(_values.Length - 1);
			float x = rectangle.X + LEFT_MARGIN;
			for (int i = 0; i < _values.Length; i++)
			{
				graphics.DrawLine(Style.ForePen, x, rectangle.Y + TOP_MARGIN + SLIDER_HEIGHT + TICK_SPACE, x, rectangle.Y + TOP_MARGIN + SLIDER_HEIGHT + TICK_SPACE + TICK_HEIGHT);
				if (i == _index)
					graphics.FillRectangle(Style.ForeBrush, x - (int)(SLIDER_WIDTH / 2.0), rectangle.Y + TOP_MARGIN, SLIDER_WIDTH, SLIDER_HEIGHT);
				x += increment;
			}
			graphics.DrawLine(Style.ForePen, rectangle.X + LEFT_MARGIN, rectangle.Y + TOP_MARGIN + (int)(SLIDER_HEIGHT / 2.0), x - increment, rectangle.Y + TOP_MARGIN + (int)(SLIDER_HEIGHT / 2.0));

			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, rectangle.Width, MinimumSize.Height);
		}

	}
}
