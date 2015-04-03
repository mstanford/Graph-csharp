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
	public class MultiSelector : Widget
	{
		private static readonly int LEFT_MARGIN = 8;
		private static readonly int RIGHT_MARGIN = 8;
		private static readonly int TOP_MARGIN = 8;
		private static readonly int BOTTOM_MARGIN = 8;
		private static readonly int INNER_MARGIN = 4;

		private static readonly int BOX_OUTER_WIDTH = 9;
		private static readonly int BOX_OUTER_HEIGHT = 9;
		private static readonly int BOX_INNER_WIDTH = 7;
		private static readonly int BOX_INNER_HEIGHT = 7;

		private readonly string[] _values;
		private bool[] _selected;
		private readonly System.Drawing.SizeF[] _sizes;

		public MultiSelector(string name, string[] values)
			: base(name)
		{
			_values = values;
			_selected = new bool[values.Length];
			_sizes = new System.Drawing.SizeF[_values.Length];
		}

		public override void Initialize(Scripting.Frame frame, Style style)
		{
			Frame = frame;
			Style = style;

			if (Frame.Values.ContainsKey(Name))
			{
				string[] selectedValues = (string[])Frame.Values[Name];

				for (int i = 0; i < selectedValues.Length; i++)
				{
					int index = System.Array.IndexOf(_values, selectedValues[i]);
					if (index > -1)
						_selected[index] = true;
				}
			}
			else
			{
				Frame.Update(Name, new string[] { });
			}


			float width = 0;
			float height = TOP_MARGIN;
			for (int i = 0; i < _sizes.Length; i++)
			{
				System.Drawing.SizeF size = style.Graphics.MeasureString(_values[i], style.Font);
				_sizes[i] = size;
				width = (int)System.Math.Max(System.Math.Ceiling(size.Width), width);
				height += (int)System.Math.Max(System.Math.Ceiling(size.Height), BOX_OUTER_HEIGHT) + INNER_MARGIN;
			}
			width += LEFT_MARGIN + BOX_OUTER_WIDTH + INNER_MARGIN + RIGHT_MARGIN;
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
				y += (int)System.Math.Max(System.Math.Ceiling(_sizes[i].Height), BOX_OUTER_HEIGHT);
				if (location.Y < y)
				{
					_selected[i] = !_selected[i];

					//DUP
					List<string> selectedValues = new List<string>();
					for (int j = 0; j < _selected.Length; j++)
						if (_selected[j])
							selectedValues.Add(_values[j]);
					Frame.Update(Name, selectedValues.ToArray());
					return;
				}
				y += INNER_MARGIN;
			}
		}

		public override void Layout(System.Drawing.RectangleF rectangle)
		{
			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, MinimumSize.Width, MinimumSize.Height);
		}

		public override void Render(System.Drawing.Graphics graphics, System.Drawing.RectangleF rectangle)
		{
			int x = (int)rectangle.X + LEFT_MARGIN;
			int y = (int)rectangle.Y + TOP_MARGIN;
			for (int i = 0; i < _values.Length; i++)
			{
				int rowHeight = (int)System.Math.Max(System.Math.Ceiling(_sizes[i].Height), BOX_OUTER_HEIGHT);

				graphics.FillRectangle(Style.ForeBrush, x, y + ((rowHeight - BOX_OUTER_HEIGHT) / 2), BOX_OUTER_WIDTH, BOX_OUTER_HEIGHT);
				if (_selected[i])
				{
					graphics.FillRectangle(Style.BackBrush, x + ((BOX_OUTER_WIDTH - BOX_INNER_WIDTH) / 2), y + ((rowHeight - BOX_OUTER_HEIGHT) / 2) + ((BOX_OUTER_HEIGHT - BOX_INNER_HEIGHT) / 2), BOX_INNER_WIDTH, BOX_INNER_HEIGHT);
					//graphics.DrawRectangle(Style.ForePen, X + (LEFT_MARGIN / 2), y, Width - (LEFT_MARGIN / 2) - (RIGHT_MARGIN / 2), rowHeight);
				}
				graphics.DrawString(_values[i], Style.Font, Style.ForeBrush, x + BOX_OUTER_WIDTH + INNER_MARGIN, y);

				y += rowHeight + INNER_MARGIN;
			}

			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, MinimumSize.Width, MinimumSize.Height);
		}

	}
}
