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
	public class Margin : Widget
	{
		private readonly float _margin;

		public Margin(float margin, Widget widget)
			: base(widget)
		{
			_margin = margin;
		}

		public override void Initialize(Scripting.Frame frame, Style style)
		{
			Frame = frame;
			Style = style;

			Widgets[0].Initialize(frame, style);

			ExpandableWidthCount += Widgets[0].ExpandableWidthCount;
			ExpandableHeightCount += Widgets[0].ExpandableHeightCount;

			MinimumSize = new System.Drawing.SizeF(Widgets[0].MinimumSize.Width + _margin + _margin, Widgets[0].MinimumSize.Height + _margin + _margin);
		}

		public override void Layout(System.Drawing.RectangleF rectangle)
		{
			Widgets[0].Layout(new System.Drawing.RectangleF(rectangle.X + _margin, rectangle.Y + _margin, rectangle.Width - _margin - _margin, rectangle.Height - _margin - _margin));

			RenderedRectangle = Widgets[0].RenderedRectangle;
		}

		public override void Render(System.Drawing.Graphics graphics, System.Drawing.RectangleF rectangle)
		{
			Widgets[0].Render(graphics, new System.Drawing.RectangleF(rectangle.X + _margin, rectangle.Y + _margin, rectangle.Width - _margin - _margin, rectangle.Height - _margin - _margin));

			RenderedRectangle = Widgets[0].RenderedRectangle;
		}

	}
}
