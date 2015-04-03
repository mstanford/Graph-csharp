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

namespace Graph.Widgets.Scales
{
	public class TimeSeriesScale : Scale
	{

		private TimeSeriesScale(List<string> values, int ticks) : base(Create(values, ticks), values.Count - 1, 0) { }

		public static TimeSeriesScale X(List<string> values)
		{
			int xTicks = 5;
			if (values.Count < xTicks) return null;
			return new TimeSeriesScale(values, xTicks);
		}

		public static TimeSeriesScale Y(List<string> values)
		{
			int yTicks = 5;
			if (values.Count < yTicks) return null;
			return new TimeSeriesScale(values, yTicks);
		}

		private static Label[] Create(List<string> values, int ticks)
		{
			List<Label> labels = new List<Label>();
			int increment = values.Count / ticks;
			for (int i = (values.Count - 1); i >= 0; i -= increment)
				labels.Add(new Label(i, values[i]));
			return labels.ToArray();
		}

	}
}
