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

namespace Task
{
	public class Ini
	{

		public static Dictionary<string, Dictionary<string, string>> Parse(string path)
		{
			using (System.IO.TextReader reader = System.IO.File.OpenText(path))
			{
				Dictionary<string, Dictionary<string, string>> ini = Parse(reader);
				reader.Close();
				return ini;
			}
		}

		public static Dictionary<string, Dictionary<string, string>> Parse(System.IO.TextReader reader)
		{
			Dictionary<string, Dictionary<string, string>> ini = new Dictionary<string, Dictionary<string, string>>();
			foreach (Section section in ParseList(reader))
				ini.Add(section.Name, section.Entries);
			return ini;
		}

		public static List<Section> ParseList(string path)
		{
			using (System.IO.TextReader reader = System.IO.File.OpenText(path))
			{
				List<Section> ini = ParseList(reader);
				reader.Close();
				return ini;
			}
		}

		public static List<Section> ParseList(System.IO.TextReader reader)
		{
			List<Section> ini = new List<Section>();
			int n;
			while ((n = reader.Peek()) != -1)
			{
				char ch = (char)n;
				switch (ch)
				{
					case '[':
						reader.Read();
						ParseListSection(reader, ini);
						break;
					case '\r':
					case '\n':
					case '\t':
					case ' ':
						reader.Read();
						break;
					case '#':
						reader.ReadLine();
						break;
					default:
						throw new System.Exception("Invalid ini.  Section header expected, but next char was '" + ch + "'.  Remaining text is: \r\n\r\n" + reader.ReadToEnd());
				}
			}
			return ini;
		}

		private static void ParseListSection(System.IO.TextReader reader, List<Section> ini)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int n;
			while ((n = reader.Peek()) != -1)
			{
				char ch = (char)n;
				switch (ch)
				{
					case ']':
						reader.Read();
						ini.Add(new Section(stringBuilder.ToString(), ParseEntries(reader)));
						return;
					case '\r':
					case '\n':
						throw new System.Exception("Invalid ini section header.  Expected ']', but next char was '" + ch + "'.  Remaining text is: \r\n\r\n" + reader.ReadToEnd());
					default:
						reader.Read();
						stringBuilder.Append(ch);
						break;
				}
			}
			throw new System.Exception("Invalid ini section header.  Expected ']', but ran out of text.  Remaining text is: \r\n\r\n" + reader.ReadToEnd());
		}

		private static Dictionary<string, string> ParseEntries(System.IO.TextReader reader)
		{
			Dictionary<string, string> entries = new Dictionary<string, string>();
			StringBuilder stringBuilderName = new StringBuilder();
			StringBuilder stringBuilderValue = new StringBuilder();
			StringBuilder currentStringBuilder = stringBuilderName;
			int n;
			while ((n = reader.Peek()) != -1)
			{
				char ch = (char)n;
				switch (ch)
				{
					case '[':
						if (stringBuilderName.Length > 0)
							entries.Add(stringBuilderName.ToString(), stringBuilderValue.ToString().TrimEnd());
						return entries;
					case '=':
						reader.Read();
						if (currentStringBuilder == stringBuilderValue)
							currentStringBuilder.Append(ch);
						else
							currentStringBuilder = stringBuilderValue;
						break;
					case '\r':
					case '\n':
						reader.Read();
						if (stringBuilderName.Length > 0)
							entries.Add(stringBuilderName.ToString(), stringBuilderValue.ToString().TrimEnd());
						stringBuilderName.Length = 0;
						stringBuilderValue.Length = 0;
						currentStringBuilder = stringBuilderName;
						break;
					case '\t':
					case ' ':
						reader.Read();
						if (currentStringBuilder == stringBuilderValue && currentStringBuilder.Length > 0)
							currentStringBuilder.Append(ch);
						break;
					case '#':
						reader.ReadLine();
						if (stringBuilderName.Length > 0)
							entries.Add(stringBuilderName.ToString(), stringBuilderValue.ToString().TrimEnd());
						stringBuilderName.Length = 0;
						stringBuilderValue.Length = 0;
						currentStringBuilder = stringBuilderName;
						break;
					default:
						reader.Read();
						currentStringBuilder.Append(ch);
						break;
				}
			}
			if (stringBuilderName.Length > 0)
				entries.Add(stringBuilderName.ToString(), stringBuilderValue.ToString().TrimEnd());
			return entries;
		}

		public class Section
		{
			public readonly string Name;
			public readonly Dictionary<string, string> Entries;

			public Section(string name, Dictionary<string, string> entries)
			{
				Name = name;
				Entries = entries;
			}
		}

	}
}
