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
	public class Row : Widget
	{

		public Row(params Widget[] widgets)
			: base(widgets)
		{
		}

		public override void Initialize(Scripting.Frame frame, Style style)
		{
			Frame = frame;
			Style = style;

			for (int i = 0; i < Widgets.Length; i++)
				Widgets[i].Initialize(frame, style);

			float width = 0;
			float height = 0;
			for (int i = 0; i < Widgets.Length; i++)
			{
				width += Widgets[i].MinimumSize.Width;
				height = System.Math.Max(height, Widgets[i].MinimumSize.Height);

				ExpandableWidthCount += Widgets[i].ExpandableWidthCount;
				ExpandableHeightCount += Widgets[i].ExpandableHeightCount;
			}
			MinimumSize = new System.Drawing.SizeF(width, height);
		}

		public override void Layout(System.Drawing.RectangleF rectangle)
		{
			float widthIncrement = 0;
			if (ExpandableWidthCount > 0)
				widthIncrement = (rectangle.Width - MinimumSize.Width) / ExpandableWidthCount;

			float x = rectangle.X;
			float y = rectangle.Y;
			for (int i = 0; i < Widgets.Length; i++)
			{
				float width = Widgets[i].MinimumSize.Width + (widthIncrement * Widgets[i].ExpandableWidthCount);
				Widgets[i].Layout(new System.Drawing.RectangleF(x, y, width, rectangle.Height));
				x += width;
			}

			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, x - rectangle.X, rectangle.Height);
		}

		public override void Render(System.Drawing.Graphics graphics, System.Drawing.RectangleF rectangle)
		{
			float widthIncrement = 0;
			if (ExpandableWidthCount > 0)
				widthIncrement = (rectangle.Width - MinimumSize.Width) / ExpandableWidthCount;

			float x = rectangle.X;
			float y = rectangle.Y;
			for (int i = 0; i < Widgets.Length; i++)
			{
				float width = Widgets[i].MinimumSize.Width + (widthIncrement * Widgets[i].ExpandableWidthCount);
				Widgets[i].Render(graphics, new System.Drawing.RectangleF(x, y, width, rectangle.Height));
				x += width;
			}

			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, x - rectangle.X, rectangle.Height);
		}

	}
}
