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
	public class Cube<T> where T : Message
	{
		private readonly Dictionary<Key<string>, Index> _indexes = new Dictionary<Key<string>, Index>();

		public Cube() { }
		private Cube(Tag[] tags) { Tags = tags; }

		public readonly System.Collections.Generic.List<T> Messages = new System.Collections.Generic.List<T>();
		public readonly Tag[] Tags;

		public void Add(T message)
		{
			Messages.Add(message);
			foreach (Index index in _indexes.Values)
				index.Add(message);
		}

		public Dictionary<Key<Tag>, Cube<T>> Split(Key<string> key)
		{
			if (!_indexes.ContainsKey(key))
				_indexes.Add(key, new Index(key, Messages));
			return _indexes[key].Cubes;
		}

		public Cube<T> Slice(params Tag[] tags)
		{
			System.Collections.Generic.List<Tag[]> expandedTags = Union(tags);
			if (expandedTags.Count > 1)
			{
				//TODO this sucks.
				Cube<T> cube = new Cube<T>();
				foreach (Tag[] tags2 in expandedTags)
					foreach (T t in Slice(tags2).Messages)
						cube.Add(t);
				return cube;
			}
			else
			{
				string[] names = new string[tags.Length];
				for (int i = 0; i < tags.Length; i++)
					names[i] = tags[i].Name;
				Dictionary<Key<Tag>, Cube<T>> split = Split(new Key<string>(names));
				Key<Tag> key = new Key<Tag>(tags);
				if (split.ContainsKey(key))
					return split[key];
				return new Cube<T>();
			}
		}

		//TODO this sucks.
		private static System.Collections.Generic.List<Tag[]> Union(Tag[] tags)
		{
			System.Collections.Generic.List<string> names = new System.Collections.Generic.List<string>();
			Dictionary<string, System.Collections.Generic.List<Tag>> groups = new Dictionary<string, System.Collections.Generic.List<Tag>>();
			for (int i = 0; i < tags.Length; i++)
			{
				if (!groups.ContainsKey(tags[i].Name))
				{
					names.Add(tags[i].Name);
					groups.Add(tags[i].Name, new System.Collections.Generic.List<Tag>());
				}
				groups[tags[i].Name].Add(tags[i]);
			}

			System.Collections.Generic.List<Tag[]> expandedTags = new System.Collections.Generic.List<Tag[]>();
			expandedTags.Add(new Tag[0]);
			for (int i = 0; i < names.Count; i++)
			{
				System.Collections.Generic.List<Tag[]> expandedTags2 = new System.Collections.Generic.List<Tag[]>();
				System.Collections.Generic.List<Tag> group = groups[names[i]];
				foreach (Tag[] expandedTagArray in expandedTags)
					for (int j = 0; j < group.Count; j++)
						expandedTags2.Add(cons(expandedTagArray, group[j]));
				expandedTags = expandedTags2;
			}
			return expandedTags;
		}

		private static Tag[] cons(Tag[] A, Tag b)
		{
			Tag[] C = new Tag[A.Length + 1];
			System.Array.Copy(A, 0, C, 0, A.Length);
			C[C.Length - 1] = b;
			return C;
		}

		public System.Collections.Generic.List<Record> Shape(string by, Key<string> key)
		{
			Dictionary<Key<Tag>, Cube<T>> split = Split(key);
			System.Collections.Generic.List<Record> shape = new System.Collections.Generic.List<Record>();
			foreach (KeyValuePair<Key<Tag>, Cube<T>> kvp in split)
			{
				Dictionary<string, T> record = new Dictionary<string, T>();
				foreach (T t in kvp.Value.Messages)
					record.Add(t[by], t);
				shape.Add(new Record(Tag.Generate(kvp.Key.Values), kvp.Key.Values, record));
			}
			return shape;
		}

		private class Index
		{
			private readonly Key<string> _key;

			public Index(Key<string> key, System.Collections.Generic.List<T> messages)
			{
				_key = key;
				foreach (T message in messages)
					Add(message);
			}

			public readonly Dictionary<Key<Tag>, Cube<T>> Cubes = new Dictionary<Key<Tag>, Cube<T>>();

			public void Add(T message)
			{
				Tag[] tags = new Tag[_key.Values.Length];
				for (int i = 0; i < _key.Values.Length; i++)
				{
					if (!message.IsDefined(_key.Values[i]))
						return; // Philosophically, if a dimension is not defined for this record then it does not exist in that particular key space.
					tags[i] = new Tag(_key.Values[i], message[_key.Values[i]]);
				}

				Key<Tag> key = new Key<Tag>(tags);
				if (!Cubes.ContainsKey(key))
					Cubes.Add(key, new Cube<T>(tags));
				Cubes[key].Messages.Add(message);
			}
		}

		public class Record : Message
		{
			public readonly Dictionary<string, T> Fields;

			public Record(string query, Tag[] tags, Dictionary<string, T> fields)
				: base(query, tags)
			{
				Fields = fields;
			}

		}

	}
}

//TODO DELETE
//public void Sort(string[] columns, bool ascending, bool[] numeric)
//{
//    Messages.Sort(delegate (Dictionary<string, object> A, Dictionary<string, object> B) {
//        for (int i = 0; i < columns.Length; i++)
//        {
//            int cmp;
//            if (numeric[i])
//                cmp = Genesis.Convert.ToDouble(A[columns[i]]).CompareTo(Genesis.Convert.ToDouble(B[columns[i]]));
//            else
//                cmp = A[columns[i]].ToString().CompareTo(B[columns[i]].ToString());
//            if (!ascending)
//                cmp = -cmp;
//            if (cmp != 0)
//                return cmp;
//        }
//        return 0;
//    });
//}
