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

namespace Graph.Data
{
	public class Key
	{
		public readonly object[] Values;

		public Key(params object[] values) { Values = values; }

		public override int GetHashCode()
		{
			int h = 0;
			for (int i = 0; i < Values.Length; i++)
				if (Values[i] == null)
					h = (h << 4) ^ (h >> 28) ^ 0;
				else
					h = (h << 4) ^ (h >> 28) ^ Values[i].GetHashCode();
			return h;
		}

		public override bool Equals(object obj)
		{
			Key key = (Key)obj;
			if (key.Values.Length != Values.Length)
				return false;
			for (int i = 0; i < key.Values.Length; i++)
				if (!Equals(key.Values[i], Values[i]))
					return false;
			return true;
		}

		private static new bool Equals(object a, object b)
		{
			if (a == null && b == null)
				return true;
			if (a != null)
				return a.Equals(b);
			return false;
		}
	}

	public class Key<T>
	{
		public readonly T[] Values;

		public Key(params T[] values) { Values = values; }

		public override int GetHashCode()
		{
			int h = 0;
			for (int i = 0; i < Values.Length; i++)
				if (Values[i] == null)
					h = (h << 4) ^ (h >> 28) ^ 0;
				else
					h = (h << 4) ^ (h >> 28) ^ Values[i].GetHashCode();
			return h;
		}

		public override bool Equals(object obj)
		{
			Key<T> key = (Key<T>)obj;
			if (key.Values.Length != Values.Length)
				return false;
			for (int i = 0; i < key.Values.Length; i++)
				if (!Equals(key.Values[i], Values[i]))
					return false;
			return true;
		}

		private static new bool Equals(object a, object b)
		{
			if (a == null && b == null)
				return true;
			if (a != null)
				return a.Equals(b);
			return false;
		}
	}
}
