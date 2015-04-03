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
	public class Style
	{
		private System.Drawing.Graphics _graphics = System.Drawing.Graphics.FromImage(new System.Drawing.Bitmap(1, 1));

		private System.Drawing.Color _backColor = System.Drawing.Color.Black;
		private System.Drawing.Brush _backBrush = System.Drawing.Brushes.Black;

		private System.Drawing.Color _foreColor = System.Drawing.Color.White;
		private System.Drawing.Brush _foreBrush = System.Drawing.Brushes.White;
		private System.Drawing.Pen _forePen = System.Drawing.Pens.White;

		private System.Drawing.Color _titleColor = System.Drawing.Color.White;
		private System.Drawing.Brush _titleBrush = System.Drawing.Brushes.White;
		private System.Drawing.Pen _titlePen = System.Drawing.Pens.White;

		private System.Drawing.Color _borderColor = System.Drawing.Color.DimGray;
		private System.Drawing.Pen _borderPen = System.Drawing.Pens.DimGray;

		private System.Drawing.Color _axisTickColor = System.Drawing.Color.White;
		private System.Drawing.Pen _axisTickPen = System.Drawing.Pens.White;

		private System.Drawing.Pen _selectedPen;

		private System.Drawing.Font _font;
		private System.Drawing.Font _axisFont;
		private System.Drawing.Font _smallTitleFont;
		private System.Drawing.Font _titleFont;

		//TODO Load user style from a file.
		public Style()
		{
			_font = Widgets.Style.Palette.Font("Calibri,8");
			_axisFont = Widgets.Style.Palette.Font("Calibri,8");
			//_axisFont = Widgets.Style.Palette.Font("Calibri,12,Bold");
			_smallTitleFont = Widgets.Style.Palette.Font("Calibri,12");
			_titleFont = Widgets.Style.Palette.Font("Calibri,24,Bold");
			_selectedPen = new System.Drawing.Pen(new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Percent40, System.Drawing.Color.FromArgb(63, _foreColor)));
		}

		public System.Drawing.Graphics Graphics { get { return _graphics; } }

		public System.Drawing.Color BackColor { get { return _backColor; } set { _backColor = value; _backBrush = new System.Drawing.SolidBrush(_backColor); } }
		public System.Drawing.Brush BackBrush { get { return _backBrush; } }

		public System.Drawing.Color ForeColor { get { return _foreColor; } set { _foreColor = value; _foreBrush = new System.Drawing.SolidBrush(_foreColor); _forePen = new System.Drawing.Pen(_foreBrush); _selectedPen = new System.Drawing.Pen(new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Percent40, System.Drawing.Color.FromArgb(63, _foreColor))); } }
		public System.Drawing.Brush ForeBrush { get { return _foreBrush; } }
		public System.Drawing.Pen ForePen { get { return _forePen; } }

		public System.Drawing.Color TitleColor { get { return _titleColor; } set { _titleColor = value; _titleBrush = new System.Drawing.SolidBrush(_titleColor); _titlePen = new System.Drawing.Pen(_titleBrush); _selectedPen = new System.Drawing.Pen(new System.Drawing.Drawing2D.HatchBrush(System.Drawing.Drawing2D.HatchStyle.Percent40, System.Drawing.Color.FromArgb(63, _titleColor))); } }
		public System.Drawing.Brush TitleBrush { get { return _titleBrush; } }
		public System.Drawing.Pen TitlePen { get { return _titlePen; } }

		public System.Drawing.Color BorderColor { get { return _borderColor; } set { _borderColor = value; _borderPen = new System.Drawing.Pen(new System.Drawing.SolidBrush(_borderColor)); } }
		public System.Drawing.Pen BorderPen { get { return _borderPen; } }

		public System.Drawing.Color AxisTickColor { get { return _axisTickColor; } set { _axisTickColor = value; _axisTickPen = new System.Drawing.Pen(new System.Drawing.SolidBrush(_axisTickColor)); } }
		public System.Drawing.Pen AxisTickPen { get { return _axisTickPen; } }

		public System.Drawing.Pen SelectedPen { get { return _selectedPen; } }

		public System.Drawing.Font Font { get { return _font; } set { _font = value; } }
		public System.Drawing.Font AxisFont { get { return _axisFont; } set { _axisFont = value; } }
		public System.Drawing.Font SmallTitleFont { get { return _smallTitleFont; } set { _smallTitleFont = value; } }
		public System.Drawing.Font TitleFont { get { return _titleFont; } set { _titleFont = value; } }

		public System.Drawing.SizeF MeasureString(string s) { return MeasureString(s, _font); }
		public System.Drawing.SizeF MeasureString(string s, System.Drawing.Font font) { return _graphics.MeasureString(s, font, new System.Drawing.PointF(0, 0), new System.Drawing.StringFormat(System.Drawing.StringFormatFlags.MeasureTrailingSpaces | System.Drawing.StringFormatFlags.NoWrap)); }

		public class Palette
		{
			private static Dictionary<string, System.Drawing.Brush> _brushes = new Dictionary<string, System.Drawing.Brush>();
			private static Dictionary<string, System.Drawing.Font> _fonts = new Dictionary<string, System.Drawing.Font>();
			private static Dictionary<string, System.Drawing.Pen> _pens = new Dictionary<string, System.Drawing.Pen>();

			public static System.Drawing.Brush Brush(string color)
			{
				if (!_brushes.ContainsKey(color))
				{
					//TODO remove the locks.
					//lock (_brushes)
					{
						_brushes.Add(color, new System.Drawing.SolidBrush(Parse(color)));
					}
				}
				return _brushes[color];
			}

			public static System.Drawing.Font Font(string key)
			{
				if (!_fonts.ContainsKey(key))
				{
					//lock (_fonts)
					{
						string[] asz = key.Split(',');
						switch (asz.Length)
						{
							case 2: _fonts.Add(key, new System.Drawing.Font(asz[0], float.Parse(asz[1]))); break;
							case 3: _fonts.Add(key, new System.Drawing.Font(asz[0], float.Parse(asz[1]), (System.Drawing.FontStyle)Enum.Parse(typeof(System.Drawing.FontStyle), asz[2]))); break;
							default: throw new System.Exception();
						}
					}
				}
				return _fonts[key];
			}

			public static System.Drawing.Font Font(string fontName, float fontSize)
			{
				string key = fontName + "," + fontSize;
				if (!_fonts.ContainsKey(key))
				{
					//lock (_fonts)
					{
						_fonts.Add(key, new System.Drawing.Font(fontName, fontSize));
					}
				}
				return _fonts[key];
			}

			public static System.Drawing.Pen Pen(string color, float width)
			{
				string key = color + "::" + width;
				if (!_pens.ContainsKey(key))
				{
					//lock (_pens)
					{
						_pens.Add(key, new System.Drawing.Pen(Parse(color), width));
					}
				}
				return _pens[key];
			}

			public static System.Drawing.Pen Pen(string color, float width, System.Drawing.Drawing2D.LineJoin lineJoin)
			{
				string key = color + "::" + width + "::" + lineJoin;
				if (!_pens.ContainsKey(key))
				{
					//lock (_pens)
					{
						System.Drawing.Pen pen = new System.Drawing.Pen(Parse(color), width);
						pen.LineJoin = lineJoin;
						_pens.Add(key, pen);
					}
				}
				return _pens[key];
			}

			public static System.Drawing.Pen Pen(string color, float width, System.Drawing.Drawing2D.DashStyle dashStyle)
			{
				string key = color + "::" + width + "::" + dashStyle;
				if (!_pens.ContainsKey(key))
				{
					//lock (_pens)
					{
						System.Drawing.Pen pen = new System.Drawing.Pen(Parse(color), width);
						pen.DashStyle = dashStyle;
						_pens.Add(key, pen);
					}
				}
				return _pens[key];
			}

			//private static System.Drawing.Color CreateColor(string s)
			//{
			//    System.Drawing.Color color = System.Drawing.Color.FromName(s);
			//    if (color.A == 0 && color.R == 0 && color.G == 0 && color.B == 0)
			//        return System.Drawing.ColorTranslator.FromHtml("#" + s);
			//    return color;
			//}

			public static System.Drawing.Color Parse(string s)
			{
				if (s.IndexOf(",") == -1)
					return System.Drawing.Color.FromName(s);

				string[] asz = s.Split(',');
				return System.Drawing.Color.FromArgb(Int16.Parse(asz[0].Trim()), Int16.Parse(asz[1].Trim()), Int16.Parse(asz[2].Trim()));
			}

			public static System.Drawing.Color NearestNamedColor(int r, int g, int b)
			{
				System.Drawing.Color color = System.Drawing.Color.FromArgb(r, g, b);
				System.Reflection.PropertyInfo[] properties = typeof(System.Drawing.Color).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
				List<System.Drawing.Color> namedColors = new List<System.Drawing.Color>(properties.Length);
				for (int i = 0; i < properties.Length; i++)
					namedColors.Add((System.Drawing.Color)properties[i].GetValue(null, null));
				namedColors.Sort(new NearestNamedColorSorter(color));
				return namedColors[0];
			}

			private class NearestNamedColorSorter : IComparer<System.Drawing.Color>
			{
				private System.Drawing.Color _color;
				public NearestNamedColorSorter(System.Drawing.Color color) { _color = color; }
				public int Compare(System.Drawing.Color x, System.Drawing.Color y) { return Distance(x).CompareTo(Distance(y)); }
				private double Distance(System.Drawing.Color color) { return System.Math.Sqrt(Square(_color.R - color.R) + Square(_color.G - color.G) + Square(_color.B - color.B)); }
				private int Square(int a) { return a * a; }
			}

		}

	}
}
