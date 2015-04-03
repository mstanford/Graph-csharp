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
	public class Tag : IComparable<Tag>
	{
		public readonly string Name;
		public readonly string Value;

		public Tag(string name, string value)
		{
			Name = name;
			Value = value;
		}

		public override bool Equals(object obj)
		{
			Tag tag = (Tag)obj;
			return tag.Name.Equals(Name) && tag.Value.Equals(Value);
		}

		public override int GetHashCode()
		{
			// Rotating hash
			int h = Value.GetHashCode();
			h = (h << 4) ^ (h >> 28) ^ Name.GetHashCode();
			return h;
		}

		public int CompareTo(Tag other)
		{
			int cmp = Name.CompareTo(other.Name);
			if (cmp == 0)
				cmp = Value.CompareTo(other.Value);
			return cmp;
		}

		public static Tag[] Parse(string query)
		{
			if (query.Length == 0)
				return new Tag[0];
			string[] aqs = query.Split('&');
			Tag[] tags = new Tag[aqs.Length];
			for (int i = 0; i < aqs.Length; i++)
			{
				string[] s = aqs[i].Split('=');
				if (s[0].Length == 0 || s[1].Length == 0 || s[1].Contains("{"))
					throw new System.Exception();
				tags[i] = new Tag(s[0], s[1]);
			}
			return tags;
		}

		public static string Generate(Tag[] tags)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			stringBuilder.Append(tags[0].Name + "=" + tags[0].Value);
			for (int i = 1; i < tags.Length; i++)
				stringBuilder.Append("&" + tags[i].Name + "=" + tags[i].Value);
			return stringBuilder.ToString();
		}

	}
}
