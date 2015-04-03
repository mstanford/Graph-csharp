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

namespace Graph.Collections
{
	public class Pool<T, U>
	{
		private readonly System.Collections.Generic.Dictionary<T, System.Collections.Generic.Stack<U[]>> _stacks = new System.Collections.Generic.Dictionary<T, System.Collections.Generic.Stack<U[]>>();

		public int Count(T t)
		{
			if (!_stacks.ContainsKey(t))
				return 0;
			return _stacks[t].Count;
		}

		public void Push(T t, U[] u)
		{
			if (!_stacks.ContainsKey(t))
				_stacks.Add(t, new System.Collections.Generic.Stack<U[]>());
			System.Array.Clear(u, 0, u.Length);
			_stacks[t].Push(u);
		}

		public U[] Pop(T t)
		{
			if (!_stacks.ContainsKey(t))
				throw new System.Exception();
			if (_stacks[t].Count == 0)
				throw new System.Exception();
			return _stacks[t].Pop();
		}

	}
}
