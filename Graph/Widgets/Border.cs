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
	public class Border : Widget
	{
		private readonly string _color;
		private readonly System.Drawing.Pen _pen;

		public Border(string color, Widget widget)
			: base(widget)
		{
			_color = color;
			_pen = Style.Palette.Pen(_color, 1.0f);
		}

		public override void Initialize(Scripting.Frame frame, Style style)
		{
			Frame = frame;
			Style = style;

			Widgets[0].Initialize(frame, style);

			ExpandableWidthCount += Widgets[0].ExpandableWidthCount;
			ExpandableHeightCount += Widgets[0].ExpandableHeightCount;

			MinimumSize = Widgets[0].MinimumSize;
		}

		public override void Layout(System.Drawing.RectangleF rectangle)
		{
			Widgets[0].Layout(rectangle);

			RenderedRectangle = Widgets[0].RenderedRectangle;
		}

		public override void Render(System.Drawing.Graphics graphics, System.Drawing.RectangleF rectangle)
		{
			graphics.DrawRectangle(_pen, new System.Drawing.Rectangle((int)rectangle.X, (int)rectangle.Y, (int)rectangle.Width, (int)rectangle.Height));

			Widgets[0].Render(graphics, rectangle);

			RenderedRectangle = Widgets[0].RenderedRectangle;
		}

	}
}
