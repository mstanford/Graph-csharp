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
	public class Legend : Widget
	{
		private readonly float _legendPadding;
		private readonly float _legendBoxWidth;
		private readonly float _legendBoxOffset;
		private readonly float _legendFontOffset;
		private readonly string[] _titles;
		private readonly string[] _colors;
		private readonly System.Drawing.Brush[] _brushes;

		public Legend(string[] colors, string[] titles)
			: base()
		{
			_legendPadding = 2;
			_legendBoxWidth = 12;
			_legendBoxOffset = 3;
			_legendFontOffset = -1;

			_titles = titles;
			_colors = colors;
			_brushes = new System.Drawing.Brush[colors.Length];
			for (int i = 0; i < _brushes.Length; i++)
				_brushes[i] = Style.Palette.Brush(_colors[i]);

			ExpandableWidthCount = 0;
			ExpandableHeightCount = 0;
		}

		public override void Initialize(Scripting.Frame frame, Style style)
		{
			//Frame = frame;
			Style = style;


			System.Drawing.SizeF[] sizes = new System.Drawing.SizeF[_titles.Length];
			float minimumWidth = 0;
			for (int i = 0; i < _titles.Length; i++)
			{
				sizes[i] = Style.Graphics.MeasureString(_titles[i], Style.Font);
				minimumWidth = System.Math.Max(minimumWidth, _legendPadding + _legendBoxWidth + _legendPadding + sizes[i].Width);
			}

			MinimumSize = new System.Drawing.SizeF(minimumWidth, _legendPadding + (Style.Font.Height * _titles.Length) + _legendPadding);
		}

		public override void Layout(System.Drawing.RectangleF rectangle)
		{
			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, MinimumSize.Width, MinimumSize.Height);
		}

		public override void Render(System.Drawing.Graphics graphics, System.Drawing.RectangleF rectangle)
		{
			float yOffset = _legendPadding;
			for (int i = 0; i < _titles.Length; i++)
			{
				graphics.FillRectangle(_brushes[i], rectangle.X + _legendPadding, rectangle.Y + yOffset + _legendBoxOffset, _legendBoxWidth, _legendBoxWidth);
				graphics.DrawString(_titles[i], Style.Font, Style.ForeBrush, rectangle.X + _legendPadding + _legendBoxWidth + _legendPadding, rectangle.Y + yOffset - _legendFontOffset);
				yOffset += Style.Font.Height;
			}

			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, MinimumSize.Width, MinimumSize.Height);
		}

	}
}
