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
	public class Grid : Widget
	{
		private readonly GridCell[] _cells;
		private readonly float[] _minimumWidths;
		private readonly float[] _minimumHeights;
		private readonly bool[] _expandableWidths;
		private readonly bool[] _expandableHeights;

		public Grid(params GridCell[] cells)
			: base((Widget[])cells)
		{
			_cells = cells;

			int gridWidth = 0;
			int gridHeight = 0;
			for (int i = 0; i < _cells.Length; i++)
			{
				gridWidth = System.Math.Max(gridWidth, _cells[i].GridCoordinates.X + _cells[i].GridCoordinates.Width);
				gridHeight = System.Math.Max(gridHeight, _cells[i].GridCoordinates.Y + _cells[i].GridCoordinates.Height);
			}
			_minimumWidths = new float[gridWidth];
			_minimumHeights = new float[gridHeight];
			_expandableWidths = new bool[gridWidth];
			_expandableHeights = new bool[gridHeight];
		}

		public override void Initialize(Scripting.Frame frame, Style style)
		{
			Frame = frame;
			Style = style;

			for (int i = 0; i < _cells.Length; i++)
				_cells[i].Initialize(frame, style);

			for (int i = 0; i < _cells.Length; i++)
			{
				if ((_cells[i].MinimumSize.Width > 0))
				{
					float widthIncrement = _cells[i].MinimumSize.Width / _cells[i].GridCoordinates.Width;
					for (int j = 0; j < _cells[i].GridCoordinates.Width; j++)
						_minimumWidths[_cells[i].GridCoordinates.X + j] = System.Math.Max(_minimumWidths[_cells[i].GridCoordinates.X + j], widthIncrement);
				}
				else
				{
					for (int j = 0; j < _cells[i].GridCoordinates.Width; j++)
						_expandableWidths[_cells[i].GridCoordinates.X + j] = true;
				}

				if ((_cells[i].MinimumSize.Height > 0))
				{
					float heightIncrement = _cells[i].MinimumSize.Height / _cells[i].GridCoordinates.Height;
					for (int j = 0; j < _cells[i].GridCoordinates.Height; j++)
						_minimumHeights[_cells[i].GridCoordinates.Y + j] = System.Math.Max(_minimumHeights[_cells[i].GridCoordinates.Y + j], heightIncrement);
				}
				else
				{
					for (int j = 0; j < _cells[i].GridCoordinates.Height; j++)
						_expandableHeights[_cells[i].GridCoordinates.Y + j] = true;
				}
			}

			for (int i = 0; i < _expandableWidths.Length; i++)
				if (_expandableWidths[i])
					ExpandableWidthCount++;

			for (int i = 0; i < _expandableHeights.Length; i++)
				if (_expandableHeights[i])
					ExpandableHeightCount++;

			float minimumWidth = 0;
			for (int i = 0; i < _minimumWidths.Length; i++)
				minimumWidth += _minimumWidths[i];

			float minimumHeight = 0;
			for (int i = 0; i < _minimumHeights.Length; i++)
				minimumHeight += _minimumHeights[i];

			MinimumSize = new System.Drawing.SizeF(minimumWidth, minimumHeight);
		}

		public override void Layout(System.Drawing.RectangleF rectangle)
		{
			float widthIncrement = 0;
			if (ExpandableWidthCount > 0)
			{
				widthIncrement = (rectangle.Width - MinimumSize.Width) / ExpandableWidthCount;

				float widthAdjustment = rectangle.Width;
				for (int i = 0; i < _minimumWidths.Length; i++)
				{
					if (_expandableWidths[i])
						widthAdjustment -= widthIncrement;
					else
						widthAdjustment -= _minimumWidths[i];
				}

				widthIncrement += widthAdjustment / ExpandableWidthCount;
			}

			float heightIncrement = 0;
			if (ExpandableHeightCount > 0)
			{
				heightIncrement = (rectangle.Height - MinimumSize.Height) / ExpandableHeightCount;

				float heightAdjustment = rectangle.Height;
				for (int i = 0; i < _minimumHeights.Length; i++)
				{
					if (_expandableHeights[i])
						heightAdjustment -= heightIncrement;
					else
						heightAdjustment -= _minimumHeights[i];
				}

				heightIncrement += heightAdjustment / ExpandableHeightCount;
			}

			for (int i = 0; i < _cells.Length; i++)
			{
				float x = rectangle.X;
				for (int j = 0; j < _cells[i].GridCoordinates.X; j++)
				{
					if (_expandableWidths[j])
						x += widthIncrement;
					else
						x += _minimumWidths[j];
				}

				float width = 0;
				for (int j = 0; j < _cells[i].GridCoordinates.Width; j++)
				{
					if (_expandableWidths[_cells[i].GridCoordinates.X + j])
						width += widthIncrement;
					else
						width += _minimumWidths[_cells[i].GridCoordinates.X + j];
				}

				float y = rectangle.Y;
				for (int j = 0; j < _cells[i].GridCoordinates.Y; j++)
				{
					if (_expandableHeights[j])
						y += heightIncrement;
					else
						y += _minimumHeights[j];
				}

				float height = 0;
				for (int j = 0; j < _cells[i].GridCoordinates.Height; j++)
				{
					if (_expandableHeights[_cells[i].GridCoordinates.Y + j])
						height += heightIncrement;
					else
						height += _minimumHeights[_cells[i].GridCoordinates.Y + j];
				}

				_cells[i].Layout(new System.Drawing.RectangleF(x, y, width, height));
			}

			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

		public override void Render(System.Drawing.Graphics graphics, System.Drawing.RectangleF rectangle)
		{
			float widthIncrement = 0;
			if (ExpandableWidthCount > 0)
			{
				widthIncrement = (rectangle.Width - MinimumSize.Width) / ExpandableWidthCount;

				float widthAdjustment = rectangle.Width;
				for (int i = 0; i < _minimumWidths.Length; i++)
				{
					if (_expandableWidths[i])
						widthAdjustment -= widthIncrement;
					else
						widthAdjustment -= _minimumWidths[i];
				}

				widthIncrement += widthAdjustment / ExpandableWidthCount;
			}

			float heightIncrement = 0;
			if (ExpandableHeightCount > 0)
			{
				heightIncrement = (rectangle.Height - MinimumSize.Height) / ExpandableHeightCount;

				float heightAdjustment = rectangle.Height;
				for (int i = 0; i < _minimumHeights.Length; i++)
				{
					if (_expandableHeights[i])
						heightAdjustment -= heightIncrement;
					else
						heightAdjustment -= _minimumHeights[i];
				}

				heightIncrement += heightAdjustment / ExpandableHeightCount;
			}

			for (int i = 0; i < _cells.Length; i++)
			{
				float x = rectangle.X;
				for (int j = 0; j < _cells[i].GridCoordinates.X; j++)
				{
					if (_expandableWidths[j])
						x += widthIncrement;
					else
						x += _minimumWidths[j];
				}

				float width = 0;
				for (int j = 0; j < _cells[i].GridCoordinates.Width; j++)
				{
					if (_expandableWidths[_cells[i].GridCoordinates.X + j])
						width += widthIncrement;
					else
						width += _minimumWidths[_cells[i].GridCoordinates.X + j];
				}

				float y = rectangle.Y;
				for (int j = 0; j < _cells[i].GridCoordinates.Y; j++)
				{
					if (_expandableHeights[j])
						y += heightIncrement;
					else
						y += _minimumHeights[j];
				}

				float height = 0;
				for (int j = 0; j < _cells[i].GridCoordinates.Height; j++)
				{
					if (_expandableHeights[_cells[i].GridCoordinates.Y + j])
						height += heightIncrement;
					else
						height += _minimumHeights[_cells[i].GridCoordinates.Y + j];
				}

				_cells[i].Render(graphics, new System.Drawing.RectangleF(x, y, width, height));
			}

			RenderedRectangle = new System.Drawing.RectangleF(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}

	}
}
