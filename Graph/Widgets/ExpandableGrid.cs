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
	public class ExpandableGrid : Widget
	{
		private readonly GridCell[] _cells;
		private readonly int _gridWidth = 0;
		private readonly int _gridHeight = 0;

		public ExpandableGrid(params GridCell[] cells)
			: base((Widget[])cells)
		{
			_cells = cells;

			_gridWidth = 0;
			_gridHeight = 0;
			for (int i = 0; i < _cells.Length; i++)
			{
				_gridWidth = System.Math.Max(_gridWidth, _cells[i].GridCoordinates.X + _cells[i].GridCoordinates.Width);
				_gridHeight = System.Math.Max(_gridHeight, _cells[i].GridCoordinates.Y + _cells[i].GridCoordinates.Height);
			}
		}

		public override void Initialize(Scripting.Frame frame, Style style)
		{
			Frame = frame;
			Style = style;

			for (int i = 0; i < _cells.Length; i++)
				_cells[i].Initialize(frame, style);

			ExpandableWidthCount++;
			ExpandableHeightCount++;

			MinimumSize = new System.Drawing.SizeF(0, 0);
		}

		public override void Layout(System.Drawing.RectangleF rectangle)
		{
			float widthIncrement = rectangle.Width / _gridWidth;

			float heightIncrement = rectangle.Height / _gridHeight;

			for (int i = 0; i < _cells.Length; i++)
			{
				float x = rectangle.X;
				for (int j = 0; j < _cells[i].GridCoordinates.X; j++)
					x += widthIncrement;

				float width = 0;
				for (int j = 0; j < _cells[i].GridCoordinates.Width; j++)
					width += widthIncrement;

				float y = rectangle.Y;
				for (int j = 0; j < _cells[i].GridCoordinates.Y; j++)
					y += heightIncrement;

				float height = 0;
				for (int j = 0; j < _cells[i].GridCoordinates.Height; j++)
					height += heightIncrement;

				_cells[i].Layout(new System.Drawing.RectangleF(x, y, width, height));
			}

			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		public override void Render(System.Drawing.Graphics graphics, System.Drawing.RectangleF rectangle)
		{
			float widthIncrement = rectangle.Width / _gridWidth;

			float heightIncrement = rectangle.Height / _gridHeight;

			for (int i = 0; i < _cells.Length; i++)
			{
				float x = rectangle.X;
				for (int j = 0; j < _cells[i].GridCoordinates.X; j++)
					x += widthIncrement;

				float width = 0;
				for (int j = 0; j < _cells[i].GridCoordinates.Width; j++)
					width += widthIncrement;

				float y = rectangle.Y;
				for (int j = 0; j < _cells[i].GridCoordinates.Y; j++)
					y += heightIncrement;

				float height = 0;
				for (int j = 0; j < _cells[i].GridCoordinates.Height; j++)
					height += heightIncrement;

				_cells[i].Render(graphics, new System.Drawing.RectangleF(x, y, width, height));
			}

			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

	}
}
