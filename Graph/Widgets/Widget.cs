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
	public abstract class Widget
	{
		public readonly string Name;
		public readonly Widget[] Widgets;
		public Scripting.Frame Frame;
		public Style Style;
		public System.Drawing.SizeF MinimumSize;
		public System.Drawing.RectangleF RenderedRectangle;
		public int ExpandableWidthCount;
		public int ExpandableHeightCount;
		public bool Selected = false;

		public Widget() { }

		public Widget(string name) { Name = name; }

		public Widget(params Widget[] widgets) { Widgets = widgets; }

		//TODO DELETE
		//public void Initialize()
		//{
		//    Graph.Scripting.Frame frame = new Graph.Scripting.Frame();
		//    Graph.Widgets.Style style = new Graph.Widgets.Style();
		//    Initialize(frame, style);
		//}

		public abstract void Initialize(Scripting.Frame frame, Style style);

		public virtual void KeyDown(string key)
		{
			if (Widgets != null)
			{
				for (int i = 0; i < Widgets.Length; i++)
				{
					if (Widgets[i].Selected)
					{
						Widgets[i].KeyDown(key);
						return;
					}
				}
			}
		}

		public virtual void Click(System.Drawing.Point location)
		{
			if (Widgets != null)
			{
				for (int i = 0; i < Widgets.Length; i++)
				{
					if (Widgets[i].Selected)
					{
						Widgets[i].Click(location);
						return;
					}
				}
			}
		}

		public virtual void MouseDown(System.Drawing.Point location)
		{
			if (Widgets != null)
			{
				for (int i = 0; i < Widgets.Length; i++)
				{
					if (Widgets[i].RenderedRectangle.Contains(location))
					{
						for (int j = 0; j < Widgets.Length; j++)
							Widgets[j].Selected = (i == j);
						Widgets[i].MouseDown(location);
						return;
					}
				}
			}
		}

		public virtual void MouseUp(System.Drawing.Point location)
		{
			if (Widgets != null)
			{
				for (int i = 0; i < Widgets.Length; i++)
				{
					if (Widgets[i].Selected)
					{
						Widgets[i].MouseUp(location);
						return;
					}
				}
			}
		}

		public virtual void MouseMove(System.Drawing.Point location)
		{
			if (Widgets != null)
			{
				for (int i = 0; i < Widgets.Length; i++)
				{
					if (Widgets[i].Selected)
					{
						Widgets[i].MouseMove(location);
						return;
					}
				}
			}
		}

		//public virtual void MouseWheel(int delta)
		//{
		//    if (Widgets != null)
		//    {
		//        for (int i = 0; i < Widgets.Length; i++)
		//        {
		//            if (Widgets[i].Selected)
		//            {
		//                Widgets[i].MouseWheel(delta);
		//                return;
		//            }
		//        }
		//    }
		//}

		//public virtual void MouseLeave()
		//{
		//    if (Widgets != null)
		//    {
		//        for (int i = 0; i < Widgets.Length; i++)
		//        {
		//            if (Widgets[i].Selected)
		//            {
		//                Widgets[i].MouseLeave();
		//                return;
		//            }
		//        }
		//    }
		//}

		public abstract void Layout(System.Drawing.RectangleF rectangle);

		public abstract void Render(System.Drawing.Graphics graphics, System.Drawing.RectangleF rectangle);

		//protected void DrawString(System.Drawing.Graphics graphics, string s, float x, float y)
		//{
		//    graphics.DrawString(s, Style.Font, Style.ForeBrush, x, y, new System.Drawing.StringFormat(System.Drawing.StringFormatFlags.MeasureTrailingSpaces | System.Drawing.StringFormatFlags.NoWrap));
		//}

		public void Render(string path, int chartWidth, int chartHeight)
		{
			System.Drawing.RectangleF rectangle = new System.Drawing.RectangleF(0, 0, chartWidth, chartHeight);

			Layout(rectangle);

			string directory = System.IO.Path.GetDirectoryName(path);
			if (!System.IO.Directory.Exists(directory))
				System.IO.Directory.CreateDirectory(directory);
			System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(chartWidth, chartHeight);
			System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
			graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			graphics.FillRectangle(System.Drawing.Brushes.Black, rectangle);
			Render(graphics, rectangle);

			System.Console.WriteLine(System.IO.Path.Combine(directory, System.IO.Path.GetFileName(path).Replace("*", "[STAR]").Replace("\\", "[BACKSLASH]").Replace("/", "[FORWARDSLASH]")));
			bitmap.Save(System.IO.Path.Combine(directory, System.IO.Path.GetFileName(path).Replace("*", "[STAR]").Replace("\\", "[BACKSLASH]").Replace("/", "[FORWARDSLASH]")), System.Drawing.Imaging.ImageFormat.Png);
			bitmap.Dispose();
		}

	}
}
