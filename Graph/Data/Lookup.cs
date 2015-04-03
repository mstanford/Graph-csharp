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
	public class Lookup<T> : IEnumerable<T>
	{
		private readonly Dictionary<T, int> _dictionary = new Dictionary<T, int>();
		private readonly System.Collections.Generic.List<T> _list = new System.Collections.Generic.List<T>();

		public Lookup() { }

		public Lookup(List<T> list)
		{
			foreach (T t in list)
				Add(t);
		}

		public int this[T t]
		{
			get
			{
				if (!_dictionary.ContainsKey(t))
				{
					_dictionary.Add(t, _dictionary.Count);
					_list.Add(t);
				}
				return _dictionary[t];
			}
		}

		public T this[int index]
		{
			get { return _list[index]; }
		}

		public int Count
		{
			get { return _list.Count; }
		}

		public int Add(T t)
		{
			_dictionary.Add(t, _dictionary.Count);
			int index = _list.Count;
			_list.Add(t);
			return index;
		}

		public bool Contains(T t)
		{
			return _dictionary.ContainsKey(t);
		}

		public T[] ToArray()
		{
			return _list.ToArray();
		}

		public System.Collections.Generic.List<T> ToSortedList()
		{
			System.Collections.Generic.List<T> list = new System.Collections.Generic.List<T>(_list);
			list.Sort();
			return list;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new Exception("The method or operation is not implemented.");
		}

	}
}
