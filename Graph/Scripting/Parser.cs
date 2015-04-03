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

namespace Graph.Scripting
{
	public class Parser
	{

		public static IEnumerable<Expression> Parse(System.IO.TextReader reader, Dictionary<string, string> infixOperators)
		{
			Stack<ProtoExpression> nestedExpressionStack = new Stack<ProtoExpression>();
			Iterator<Token> iterator = new Iterator<Token>(Tokenize(reader).GetEnumerator());
			int depth = 0;

			Token token;
			while ((token = iterator.Peek()) != null)
			{
				switch (token.Type)
				{
					case TokenType.Tab:
						{
							iterator.Next();
							depth++;
							break;
						}
					case TokenType.Space:
						{
							throw new ParserException(token.Line, token.Column, "Expression starts with a space.");
						}
					default:
						{
							ProtoExpression expression = ParseExpression(iterator, token.Line, token.Column, infixOperators);
							if (expression.Elements.Length > 0)
							{
								if (depth > nestedExpressionStack.Count)
									throw new ParserException(token.Line, token.Column, "Invalid Expression.");

								if (depth == 0)
								{
									while (nestedExpressionStack.Count > 1)
										nestedExpressionStack.Pop();
									if (nestedExpressionStack.Count > 0)
										yield return Expressionize(nestedExpressionStack.Pop(), infixOperators);
								}
								else
								{
									while (nestedExpressionStack.Count > depth)
										nestedExpressionStack.Pop();
									nestedExpressionStack.Peek().NestedExpressions.Add(expression);
								}
								nestedExpressionStack.Push(expression);
							}
							else if (expression.Annotations.Count > 0)
							{
								foreach (KeyValuePair<string, object> annotation in expression.Annotations)
									nestedExpressionStack.Peek().Annotations.Add(annotation.Key, annotation.Value);
							}
							depth = 0;
							break;
						}
				}
			}

			while (nestedExpressionStack.Count > 1)
				nestedExpressionStack.Pop();
			if (nestedExpressionStack.Count > 0)
				yield return Expressionize(nestedExpressionStack.Pop(), infixOperators);
		}

		private static Expression Expressionize(ProtoExpression protoExpression, Dictionary<string, string> infixOperators)
		{
			object[] elements = cons(protoExpression.Elements, protoExpression.NestedExpressions);
			if (elements.Length > 1 && elements[1] is string && infixOperators != null && infixOperators.ContainsKey((string)elements[1]))
			{
				object op = elements[1];
				elements[1] = elements[0];
				elements[0] = op;
			}
			for (int i = 0; i < elements.Length; i++)
				if (elements[i] is ProtoExpression)
					elements[i] = Expressionize((ProtoExpression)elements[i], infixOperators);
			return new Expression(protoExpression.Line, protoExpression.Column, (string)elements[0], new List<object>(cdr(elements)));
			//, new Wind.Collections.ImmutableMap<string, object>(protoExpression.Annotations)
		}

		private static ProtoExpression ParseExpression(Iterator<Token> iterator, int line, int column, Dictionary<string, string> infixOperators)
		{
			List<object> values = new List<object>();
			Dictionary<string, object> annotations = new Dictionary<string, object>();

			Token token;
			while ((token = iterator.Peek()) != null)
			{
				switch (token.Type)
				{
					case TokenType.Newline:
						{
							iterator.Next();
							return new ProtoExpression(line, column, values.ToArray(), annotations);
						}
					case TokenType.Space:
					case TokenType.Tab:
						{
							iterator.Next();
							break;
						}
					default:
						{
							object value = ParseElement(iterator, line, column, infixOperators);
							if (value != null)
							{
								if (value is ProtoExpression && values.Count == 0)
								{
									return (ProtoExpression)value;
								}
								//else if (value is ProtoAnnotation)
								//{
								//    annotations.Add(((ProtoAnnotation)value).Name, ((ProtoAnnotation)value).Value);
								//}
								else
								{
									values.Add(value);
									if (values.Count == 2 && value.Equals("<-"))
									{
										ProtoExpression expression = ParseExpression(iterator, line, column, infixOperators);
										if (expression.Elements.Length == 1)
											values.Add(expression.Elements[0]);
										else
											values.Add(expression);
										return new ProtoExpression(line, column, values.ToArray(), new Dictionary<string, object>());
									}
								}
							}
							break;
						}
				}
			}

			return new ProtoExpression(line, column, values.ToArray(), annotations);
		}

		private static object ParseElement(Iterator<Token> iterator, int line, int column, Dictionary<string, string> infixOperators)
		{
			Token token;
			while ((token = iterator.Peek()) != null)
			{
				switch (token.Type)
				{
					//case TokenType.Comma:
					//case TokenType.Newline:
					//case TokenType.RightBracket:
					//case TokenType.RightParenthesis:
					//    {
					//        return null;
					//    }
					case TokenType.LeftBracket:
						{
							iterator.Next();
							return ParseArray(iterator, infixOperators);
						}
					case TokenType.LeftParenthesis:
						{
							iterator.Next();
							return ParseParentheticalExpression(iterator, token.Line, token.Column, infixOperators);
						}
					case TokenType.QuotedString:
					case TokenType.QuotedString2:
						{
							iterator.Next();
							return token.Value;
						}
					//case TokenType.Space:
					//case TokenType.Tab:
					//    {
					//        iterator.Next();
					//        break;
					//    }
					//case TokenType.Annotation:
					//    {
					//        iterator.Next();
					//        string name = token.Value;

					//        token = iterator.Peek();
					//        if (token == null)
					//            return new ProtoAnnotation(line, column, name, name);

					//        switch (token.Type)
					//        {
					//            case TokenType.LeftParenthesis:
					//                {
					//                    iterator.Next();
					//                    List<object> values = new List<object>();
					//                    values.AddRange(ParseParentheticalExpression(iterator, line, column).Elements);
					//                    return new ProtoAnnotation(line, column, name, new Expression(line, column, name, new Wind.Collections.ImmutableList<object>(values.ToArray()), new Wind.Collections.ImmutableMap<string, object>()));
					//                }
					//            default:
					//                {
					//                    return new ProtoAnnotation(line, column, name, name);
					//                }
					//        }
					//    }
					case TokenType.String:
						{
							iterator.Next();
							string value = token.Value;

							token = iterator.Peek();
							if (token == null)
								return value;

							switch (token.Type)
							{
								case TokenType.LeftBracket:
									{
										iterator.Next();
										return new ProtoExpression(line, column, new object[] { value, "[]", ParseArray(iterator, infixOperators) }, new Dictionary<string, object>());
									}
								case TokenType.LeftParenthesis:
									{
										iterator.Next();
										List<object> values = new List<object>();
										values.Add(value);
										values.AddRange(ParseParentheticalExpression(iterator, token.Line, token.Column, infixOperators).Elements);
										return new ProtoExpression(line, column, values.ToArray(), new Dictionary<string, object>());
									}
								default:
									{
										return value;
									}
							}
						}
					default:
						{
							throw new ParserException(line, column, "Invalid TokenType: " + token.Type.ToString());
						}
				}
			}

			throw new System.Exception("Invalid expression.");
		}

		private static ProtoExpression ParseParentheticalExpression(Iterator<Token> iterator, int line, int column, Dictionary<string, string> infixOperators)
		{
			List<object> values = new List<object>();
			List<object> commaSeparatedvalues = new List<object>();

			Token token;
			while ((token = iterator.Peek()) != null)
			{
				switch (token.Type)
				{
					case TokenType.Comma:
						{
							iterator.Next();
							switch (values.Count)
							{
								case 0: commaSeparatedvalues.Add(""); break;
								case 1: commaSeparatedvalues.Add(values[0]); values.Clear(); break;
								default: commaSeparatedvalues.Add(new ProtoExpression(line, column, values.ToArray(), new Dictionary<string, object>())); values.Clear(); break;
							}
							break;
						}
					case TokenType.Newline:
					case TokenType.Space:
					case TokenType.Tab:
						{
							iterator.Next();
							break;
						}
					case TokenType.RightParenthesis:
						{
							iterator.Next();
							if (commaSeparatedvalues.Count > 0)
							{
								switch (values.Count)
								{
									case 0: commaSeparatedvalues.Add(""); break;
									case 1: commaSeparatedvalues.Add(values[0]); values.Clear(); break;
									default: commaSeparatedvalues.Add(new ProtoExpression(line, column, values.ToArray(), new Dictionary<string, object>())); values.Clear(); break;
								}
								return new ProtoExpression(line, column, commaSeparatedvalues.ToArray(), new Dictionary<string, object>());
							}
							return new ProtoExpression(line, column, values.ToArray(), new Dictionary<string, object>());
						}
					default:
						{
							object value = ParseElement(iterator, line, column, infixOperators);
							if (value != null)
								values.Add(value);
							break;
						}
				}
			}

			throw new System.Exception("Invalid expression.");
		}

		private static object[] ParseArray(Iterator<Token> iterator, Dictionary<string, string> infixOperators)
		{
			List<object> values = new List<object>();
			List<object> commaSeparatedvalues = new List<object>();

			Token token;
			while ((token = iterator.Peek()) != null)
			{
				switch (token.Type)
				{
					case TokenType.Comma:
						{
							iterator.Next();
							switch (values.Count)
							{
								case 0: commaSeparatedvalues.Add(""); break;
								case 1: commaSeparatedvalues.Add(values[0]); values.Clear(); break;
								default: commaSeparatedvalues.Add(values.ToArray()); values.Clear(); break;
							}
							break;
						}
					case TokenType.RightBracket:
						{
							iterator.Next();
							if (commaSeparatedvalues.Count > 0)
							{
								switch (values.Count)
								{
									case 0: commaSeparatedvalues.Add(""); break;
									case 1: commaSeparatedvalues.Add(values[0]); values.Clear(); break;
									default: commaSeparatedvalues.Add(values.ToArray()); values.Clear(); break;
								}
								return commaSeparatedvalues.ToArray();
							}
							return values.ToArray();
						}
					case TokenType.Space:
					case TokenType.Tab:
						{
							iterator.Next();
							break;
						}
					default:
						{
							object value = ParseElement(iterator, token.Line, token.Column, infixOperators);
							if (value != null)
							{
								if (value is ProtoExpression)
								{
									value = Expressionize((ProtoExpression)value, infixOperators);
								}
								values.Add(value);
							}
							break;
						}
				}
			}

			throw new System.Exception("Invalid array.");
		}

		private static IEnumerable<Token> Tokenize(System.IO.TextReader reader)
		{
			int line = 1;
			int column = 1;
			int startColumn = column;
			StringBuilder stringBuilder = new StringBuilder();

			while (reader.Peek() != -1)
			{
				char ch = (char)reader.Read();
				column++;

				switch (ch)
				{
					case '#': // Comment
						{
							string ignore = reader.ReadLine();
							line++;
							column = 1;
							startColumn = column;
							break;
						}
					case '\r':
						{
							break;
						}
					case '\n':
						{
							yield return new Token(TokenType.Newline, line, startColumn);
							line++;
							column = 1;
							startColumn = column;
							break;
						}
					case ' ':
						{
							yield return new Token(TokenType.Space, line, startColumn);
							startColumn = column;
							break;
						}
					case '\t':
						{
							yield return new Token(TokenType.Tab, line, startColumn);
							startColumn = column;
							break;
						}
					case '(':
						{
							yield return new Token(TokenType.LeftParenthesis, line, startColumn);
							startColumn = column;
							break;
						}
					case ')':
						{
							yield return new Token(TokenType.RightParenthesis, line, startColumn);
							startColumn = column;
							break;
						}
					case '[':
						{
							yield return new Token(TokenType.LeftBracket, line, startColumn);
							startColumn = column;
							break;
						}
					case ']':
						{
							yield return new Token(TokenType.RightBracket, line, startColumn);
							startColumn = column;
							break;
						}
					case ',':
						{
							yield return new Token(TokenType.Comma, line, startColumn);
							startColumn = column;
							break;
						}
					case '@':
						{
							if (reader.Peek() == (int)'\"')
							{
								ReadQuotedString2(reader, stringBuilder);
								yield return new Token(TokenType.QuotedString2, line, startColumn, stringBuilder.ToString());
								stringBuilder.Length = 0;
							}
							else
							{
								stringBuilder.Append(ch);
								foreach (Token token in ReadString(reader, stringBuilder, line, startColumn))
									yield return token;
								stringBuilder.Length = 0;
							}
							break;
						}
					case '\"':
						{
							ReadQuotedString(reader, stringBuilder);
							yield return new Token(TokenType.QuotedString, line, startColumn, stringBuilder.ToString());
							stringBuilder.Length = 0;
							break;
						}
					//case ':':
					//    {
					//        List<Token> tokens = new List<Token>(ReadString(reader, stringBuilder, line, startColumn));
					//        if (tokens.Count != 1)
					//            throw new System.Exception("Invalid annotation.");
					//        yield return new Token(TokenType.Annotation, line, startColumn, tokens[0].Value);
					//        stringBuilder.Length = 0;
					//        break;
					//    }
					default:
						{
							stringBuilder.Append(ch);
							foreach (Token token in ReadString(reader, stringBuilder, line, startColumn))
								yield return token;
							stringBuilder.Length = 0;
							break;
						}
				}
			}
		}

		private static void ReadQuotedString2(System.IO.TextReader reader, StringBuilder stringBuilder)
		{
			reader.Read(); // Read '\"'

			int n;
			while ((n = reader.Peek()) != -1)
			{
				switch (n)
				{
					case '\"':
						reader.Read();
						char ch = (char)reader.Read();
						switch (ch)
						{
							case '\"':
								stringBuilder.Append(ch);
								break;
							default:
								return;
						}
						break;
					default:
						stringBuilder.Append((char)reader.Read());
						break;
				}
			}
			throw new System.Exception("Unterminated expression.");
		}

		private static void ReadQuotedString(System.IO.TextReader reader, StringBuilder stringBuilder)
		{
			int n;
			while ((n = reader.Peek()) != -1)
			{
				switch (n)
				{
					case '\"':
						reader.Read();
						return;
					case '\\':
						reader.Read();
						char ch = (char)reader.Read();
						switch (ch)
						{
							case 'n':
								stringBuilder.Append('\n');
								break;
							case 'r':
								stringBuilder.Append('\r');
								break;
							case 't':
								stringBuilder.Append('\t');
								break;
							case '\'':
							case '\"':
							case '\\':
								stringBuilder.Append(ch);
								break;
							default:
								throw new System.Exception("Invalid escape sequence: \\" + ch);
						}
						break;
					default:
						stringBuilder.Append((char)reader.Read());
						break;
				}
			}
			throw new System.Exception("Unterminated expression.");
		}

		private static IEnumerable<Token> ReadString(System.IO.TextReader reader, StringBuilder stringBuilder, int line, int column)
		{
			bool isRange = false;
			int n;
			while ((n = reader.Peek()) != -1)
			{
				switch (n)
				{
					case '.':
						{
							char ch = (char)reader.Read();
							if (reader.Peek() == (int)'.')
							{
								reader.Read();
								isRange = true;
								yield return new Token(TokenType.LeftParenthesis, line, column);
								if (stringBuilder.Length > 0)
								{
									yield return new Token(TokenType.String, line, column, stringBuilder.ToString());
									stringBuilder.Length = 0;
								}
								yield return new Token(TokenType.String, line, column, "..");
							}
							else
							{
								stringBuilder.Append(ch);
							}
							break;
						}
					case '#':
					case ' ':
					case '\t':
					case '\r':
					case '\n':
					case '(':
					case ')':
					case '[':
					case ']':
					case ',':
						{
							goto EXIT;
						}
					default:
						{
							stringBuilder.Append((char)reader.Read());
							break;
						}
				}
			}

		EXIT:
			if (isRange)
			{
				if (stringBuilder.Length == 0)
					throw new ParserException(line, column, "Invalid Range.");
				yield return new Token(TokenType.String, line, column, stringBuilder.ToString());
				yield return new Token(TokenType.RightParenthesis, line, column);
			}
			else
			{
				if (stringBuilder.Length > 0)
					yield return new Token(TokenType.String, line, column, stringBuilder.ToString());
			}
		}

		private static object[] cdr(object[] A)
		{
			object[] B = new object[A.Length - 1];
			Array.Copy(A, 1, B, 0, B.Length);
			return B;
		}

		private static object[] cons(object[] A, List<ProtoExpression> B)
		{
			object[] C = new object[A.Length + B.Count];
			Array.Copy(A, 0, C, 0, A.Length);
			for (int i = 0; i < B.Count; i++)
				C[i + A.Length] = B[i];
			return C;
		}

		//TODO go over these with a fine tooth comb.
		private class ParserException : System.Exception
		{
			public readonly int Line;
			public readonly int Column;

			public ParserException(int line, int column, string message)
				: base(message)
			{
				Line = line;
				Column = column;
			}

		}

		private class ProtoExpression
		{
			public readonly int Line;
			public readonly int Column;
			public readonly object[] Elements;
			public readonly List<ProtoExpression> NestedExpressions;
			public readonly Dictionary<string, object> Annotations;

			public ProtoExpression(int line, int column, object[] elements, Dictionary<string, object> annotations)
			{
				Line = line;
				Column = column;
				Elements = elements;
				NestedExpressions = new List<ProtoExpression>();
				Annotations = annotations;
			}
		}

		//private class ProtoAnnotation
		//{
		//    public readonly int Line;
		//    public readonly int Column;
		//    public readonly string Name;
		//    public readonly object Value;

		//    public ProtoAnnotation(int line, int column, string name, object value)
		//    {
		//        Line = line;
		//        Column = column;
		//        Name = name;
		//        Value = value;
		//    }
		//}

		private class Token
		{
			public readonly TokenType Type;
			public readonly int Line;
			public readonly int Column;
			public readonly string Value;

			public Token(TokenType type, int line, int column) : this(type, line, column, null) { }

			public Token(TokenType type, int line, int column, string value)
			{
				Type = type;
				Value = value;
				Line = line;
				Column = column;
			}
		}

		private enum TokenType
		{
			//Annotation,
			Comma,
			LeftBracket,
			LeftParenthesis,
			Newline,
			QuotedString,
			QuotedString2,
			RightBracket,
			RightParenthesis,
			Space,
			String,
			Tab,
		}

		public class Iterator<T>
		{
			private IEnumerator<T> _enumerator;
			private T _value;
			private bool _eof;

			public Iterator(IEnumerator<T> enumerator)
			{
				_enumerator = enumerator;
				_eof = false;
			}

			public T Peek()
			{
				if (_eof)
					return default(T);
				if (_value == null)
				{
					if (_enumerator.MoveNext())
						_value = _enumerator.Current;
					else
						_eof = true;
				}
				return _value;
			}

			public T Next()
			{
				T value = (_value != null) ? _value : Peek();
				_value = default(T); //TODO : Nullable<T>
				return value;
			}

		}

	}
}
