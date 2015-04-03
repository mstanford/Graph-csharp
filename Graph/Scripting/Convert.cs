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
	public class Convert
	{

		public static object ChangeType(object value, Type conversionType)
		{
			if (conversionType.FullName.Equals("System.Object") || value == null)
				return value;

			Type sourceType = value.GetType();
			if (sourceType.FullName.Equals(conversionType.FullName) || conversionType.IsAssignableFrom(sourceType))
				return value;

			if (sourceType.IsGenericType)
			{
				Type[] genericArguments = sourceType.GetGenericArguments();
				switch (sourceType.Namespace + "." + sourceType.Name + "[[" + genericArguments[0].FullName + "]]" + "::" + conversionType.FullName)
				{
					case "System.Collections.Generic.List`1[[System.Double]]::System.Double[]":
					case "System.Collections.Generic.List`1[[System.Double]]::System.Single[]":
						return ChangeType(ToArray((List<double>)value), conversionType);

					case "System.Collections.Generic.List`1[[System.Int32]]::System.Int32[]":
					case "System.Collections.Generic.List`1[[System.Int32]]::System.Single[]":
						return ChangeType(ToArray((List<int>)value), conversionType);

					case "System.Collections.Generic.List`1[[System.Single]]::System.Single[]":
						return ChangeType(ToArray((List<float>)value), conversionType);

					case "System.Collections.Generic.List`1[[System.String]]::System.String[]":
						return ChangeType(ToArray((List<string>)value), conversionType);

					default:
						throw new System.Exception();
				}
			}

			if (conversionType.IsGenericType)
			{
				Type[] genericArguments = conversionType.GetGenericArguments();
				switch (sourceType.FullName + "::" + conversionType.Namespace + "." + conversionType.Name + "[[" + genericArguments[0].FullName + "]]")
				{
					case "System.String[]::System.Collections.Generic.List`1[[System.String]]":
						return new List<string>((string[])value);

					default:
						throw new System.Exception();
				}
			}

			switch (sourceType.FullName + "::" + conversionType.FullName)
			{
				case "System.Double::System.String": return value.ToString();
				case "System.Double[]::System.Int32[]": return ToInt32Array((double[])value);
				case "System.Double[]::System.Single[]": return ToSingleArray((double[])value);
				case "System.Int32[]::System.Double[]": return ToDoubleArray((int[])value);
				case "System.Int32[]::System.Single[]": return ToSingleArray((int[])value);
				case "System.Int32[]::System.String[]": return ToStringArray((int[])value);
				case "System.Object[]::System.Double[]": return ToDoubleArray((object[])value);
				case "System.Object[]::System.Drawing.Color[]": return ToColorArray((object[])value);
				case "System.Object[]::System.Int32[]": return ToInt32Array((object[])value);
				case "System.Object[]::System.String[]": return ToStringArray((object[])value);
				case "System.Single[]::System.Double[]": return ToDoubleArray((float[])value);
				case "System.Single[]::System.Int32[]": return ToInt32Array((float[])value);
				case "System.String::System.Boolean": return ToBoolean((string)value);
				case "System.String::System.Double": return ToDouble((string)value);
				case "System.String::System.Int32": return ToInt32((string)value);
				case "System.String::System.Single": return ToSingle((string)value);
				case "System.String[]::System.Double[]": return ToDoubleArray((string[])value);
			}

			throw new System.Exception("Unable to convert " + sourceType.FullName + " to " + conversionType.FullName);
		}

		private static double[] ToArray(List<double> A) { double[] B = new double[A.Count]; for (int i = 0; i < A.Count; i++) B[i] = A[i]; return B; }
		private static float[] ToArray(List<float> A) { float[] B = new float[A.Count]; for (int i = 0; i < A.Count; i++) B[i] = A[i]; return B; }
		private static int[] ToArray(List<int> A) { int[] B = new int[A.Count]; for (int i = 0; i < A.Count; i++) B[i] = A[i]; return B; }
		private static string[] ToArray(List<string> A) { string[] B = new string[A.Count]; for (int i = 0; i < A.Count; i++) B[i] = A[i]; return B; }

		private static System.Drawing.Color ToColor(string s) { return System.Drawing.Color.FromName(s); }
		private static System.Drawing.Color[] ToColorArray(object[] A) { System.Drawing.Color[] B = new System.Drawing.Color[A.Length]; for (int i = 0; i < A.Length; i++) B[i] = ToColor((string)A[i]); return B; }

		private static bool ToBoolean(string s) { return bool.Parse(s); }
		private static double ToDouble(string s) { return double.Parse(s); }
		private static int ToInt32(string s) { return int.Parse(s); }
		private static float ToSingle(string s) { return float.Parse(s); }

		private static double[] ToDoubleArray(object[] A) { double[] B = new double[A.Length]; for (int i = 0; i < A.Length; i++) B[i] = System.Convert.ToDouble(A[i]); return B; }
		private static double[] ToDoubleArray(float[] A) { double[] B = new double[A.Length]; for (int i = 0; i < A.Length; i++) B[i] = (double)A[i]; return B; }
		private static double[] ToDoubleArray(int[] A) { double[] B = new double[A.Length]; for (int i = 0; i < A.Length; i++) B[i] = (double)A[i]; return B; }

		private static int[] ToInt32Array(float[] A) { int[] B = new int[A.Length]; for (int i = 0; i < A.Length; i++) B[i] = (int)A[i]; return B; }
		private static int[] ToInt32Array(double[] A) { int[] B = new int[A.Length]; for (int i = 0; i < A.Length; i++) B[i] = (int)A[i]; return B; }
		private static int[] ToInt32Array(object[] A) { int[] B = new int[A.Length]; for (int i = 0; i < A.Length; i++) B[i] = System.Convert.ToInt32(A[i]); return B; }

		private static float[] ToSingleArray(double[] A) { float[] B = new float[A.Length]; for (int i = 0; i < A.Length; i++) B[i] = (float)A[i]; return B; }
		private static float[] ToSingleArray(int[] A) { float[] B = new float[A.Length]; for (int i = 0; i < A.Length; i++) B[i] = (float)A[i]; return B; }

		private static string[] ToStringArray(object[] A) { string[] B = new string[A.Length]; for (int i = 0; i < A.Length; i++) B[i] = A[i].ToString(); return B; }
		private static string[] ToStringArray(int[] A) { string[] B = new string[A.Length]; for (int i = 0; i < A.Length; i++) B[i] = A[i].ToString(); return B; }

	}
}
