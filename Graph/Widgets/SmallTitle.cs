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
	public class SmallTitle : Widget
	{
		private static readonly int LEFT_MARGIN = 8;
		private static readonly int RIGHT_MARGIN = 8;
		private static readonly int TOP_MARGIN = 8;
		private static readonly int BOTTOM_MARGIN = 8;

		private string _title;

		public SmallTitle(string title)
			: base()
		{
			_title = title;

			ExpandableWidthCount = 0;
			ExpandableHeightCount = 0;
		}

		public override void Initialize(Scripting.Frame frame, Style style)
		{
			Frame = frame;
			Style = style;

			System.Drawing.SizeF size = Style.Graphics.MeasureString(_title, Style.SmallTitleFont);

			MinimumSize = new System.Drawing.SizeF(LEFT_MARGIN + (int)System.Math.Ceiling(size.Width) + RIGHT_MARGIN, TOP_MARGIN + (int)System.Math.Ceiling(size.Height) + BOTTOM_MARGIN);
		}

		public override void Layout(System.Drawing.RectangleF rectangle)
		{
			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, MinimumSize.Width, MinimumSize.Height);
		}

		public override void Render(System.Drawing.Graphics graphics, System.Drawing.RectangleF rectangle)
		{
			System.Drawing.StringFormat stringFormat = new System.Drawing.StringFormat();
			stringFormat.Alignment = System.Drawing.StringAlignment.Center;
			graphics.DrawString(_title, Style.SmallTitleFont, Style.TitleBrush, new System.Drawing.RectangleF(rectangle.X + LEFT_MARGIN, rectangle.Y + TOP_MARGIN, rectangle.Width - LEFT_MARGIN - RIGHT_MARGIN, Style.SmallTitleFont.Height), stringFormat);

			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, MinimumSize.Width, MinimumSize.Height);
		}

	}
}
