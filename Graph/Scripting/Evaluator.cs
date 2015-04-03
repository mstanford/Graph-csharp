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
	public sealed class Evaluator
	{
		private readonly Dictionary<string, Operator> _operators = new Dictionary<string, Operator>();
		private readonly Dictionary<string, string> AssignmentOperators = new Dictionary<string, string>();
		private readonly TypeResolver _typeResolver = new TypeResolver();

		public Evaluator(System.IO.TextReader definition)
		{
			InfixOperators.Add("<-", "<-");
			AssignmentOperators.Add("<-", "<-");

			Define(definition);
		}

		public void Define(System.IO.TextReader definition)
		{
			foreach (Expression expression in Parser.Parse(definition, null))
			{
				switch (expression.OperatorOrValue)
				{
					case "infix":
						if (expression.Operands.Count != 2)
							throw new System.Exception();
						InfixOperators.Add((string)expression.Operands[0], (string)expression.Operands[0]);
						_operators.Add((string)expression.Operands[0], new Operator(_typeResolver, (string)expression.Operands[1]));
						break;
					case "function":
						_operators[(string)expression.Operands[0]] = new Operator(_typeResolver, (string)expression.Operands[1]);
						break;
					case "type":
						Operator op = new Operator(_typeResolver, (string)expression.Operands[0]);
						_operators.Add(op.Name, op);
						break;
					default:
						throw new System.Exception("Invalid operator: " + expression.OperatorOrValue + " (" + expression.Line + ", " + expression.Column + ").");
				}
			}
		}

		public readonly Dictionary<string, string> InfixOperators = new Dictionary<string, string>();

		// Evaluate an entire script.
		public void EvaluateScript(List<Expression> expressions, Scripting.Frame frame, List<Widgets.Widget> widgets)
		{
			// To evaluate a script first get all of the variables that will be defined and the controls that will be created.
			Dictionary<string, string> variables = GetVariables(expressions, this);

			List<Expression> expressionsToEvaluate = new List<Expression>(expressions);

			//TODO you probably want to do a topological sort in case some old variables are hanging around from a prior session.

			// Keep evaluating until there are no more expressions that can be evaluated.
			int evaluatedExpressionCount = -1;
			while (evaluatedExpressionCount != 0)
			{
				evaluatedExpressionCount = 0;

				for (int i = 0; i < expressionsToEvaluate.Count; i++)
				{
					if (CanEvaluate(expressionsToEvaluate[i], this, variables, frame.Values))
					{
						object result = EvaluateExpression(expressionsToEvaluate[i], frame);
                        if (result is Graph.Widgets.Widget)
                            widgets.Add((Graph.Widgets.Widget)result);
						expressionsToEvaluate.RemoveAt(i);
						evaluatedExpressionCount++;
						break;
					}
				}
			}

			// If all of the expressions haven't been evaluated throw an exception.
			if (expressionsToEvaluate.Count > 0)
				throw new System.Exception();
		}

		//// Evaluate an entire script.
		//public Frame Evaluate(List<Expression> expressions, Frame priorFrame, Wind.Graphics.Grid grid, Wind.Graphics.Style style)
		//{
		//    // To evaluate a script first get all of the variables that will be defined and the controls that will be created.
		//    Wind.Collections.ImmutableSet<string> variables = GetVariables(expressions, this);

		//    Frame frame = new Frame();

		//    List<Expression> expressionsToEvaluate = new List<Expression>(expressions);

		//    // Keep evaluating until there are no more expressions that can be evaluated.
		//    int evaluatedExpressionCount = -1;
		//    while (evaluatedExpressionCount != 0)
		//    {
		//        evaluatedExpressionCount = 0;

		//        for (int i = 0; i < expressionsToEvaluate.Count; i++)
		//        {
		//            if (CanEvaluate(expressionsToEvaluate[i], this, variables, frame, priorFrame))
		//            {
		//                object result = Evaluate(expressionsToEvaluate[i], ref frame, priorFrame, style);
		//                if (result is Wind.Graphics.Element)
		//                    grid.Add((Wind.Graphics.Element)result, style, 0, grid.Height, 1, 1);
		//                expressionsToEvaluate.RemoveAt(i);
		//                evaluatedExpressionCount++;
		//                break;
		//            }
		//        }
		//    }

		//    // If all of the expressions haven't been evaluated throw an exception.
		//    if (expressionsToEvaluate.Count > 0)
		//        throw new System.Exception();

		//    return frame;
		//}

		// Evaluate a single expression.
		public object EvaluateExpression(object element, Scripting.Frame frame)
		{
			if (element is Expression)
			{
				//
				// In computer science, a function is a portion of code within a larger program that performs a 
				// specific task and is relatively independent of the remaining code.
				// 
				// A function may be written so that it expects to obtain one or more data values from the calling 
				// program (its parameters or arguments).  It may also return a computed value to its caller (its 
				// return value), or provide various result values or out(put) parameters.  Indeed, a common use of
				// functions is to implement mathematical functions, in which the purpose of the function is purely 
				// to compute one or more results whose values are entirely determined by the parameters passed to the 
				// function.  (Examples might include computing the logarithm of a number of the determinant of a 
				// matrix.
				//
				Expression expression = (Expression)element;

				switch (expression.OperatorOrValue)
				{
					case "<-":
						{
							if (expression.Operands.Count != 2)
								throw new System.Exception();
							//frame.Values.Add((string)expression.Operands[0], EvaluateExpression(expression.Operands[1], frame));
							frame.Values[(string)expression.Operands[0]] = EvaluateExpression(expression.Operands[1], frame);
							return null;
						}
					case "def":
						{
							if (expression.Operands.Count != 1)
								throw new System.Exception();
							System.IO.TextReader reader = new System.IO.StreamReader(_typeResolver.ResolveResource((string)expression.Operands[0]));
							Define(reader);
							reader.Close();
							return null;
						}
				}

				if (!_operators.ContainsKey(expression.OperatorOrValue))
					throw new System.Exception("Invalid operator: " + expression.OperatorOrValue);

				Operator @operator = _operators[expression.OperatorOrValue];

				object[] constructorParameters = new object[0];

				if (expression.Operands.Count != @operator.ParameterTypes.Length && !@operator.IsParams)
				{
					if (@operator.ConstructorTypes != null)
					{
						if (@operator.ConstructorIsParams)
							throw new System.Exception();

						constructorParameters = LoadParameters(@operator.ConstructorNames, @operator.ConstructorTypes, @operator.ConstructorIsParams, frame, expression.Operands, 0);
					}
					else
						throw new System.Exception("Parameter mismatch.");
				}

				object[] parameters = LoadParameters(@operator.ParameterNames, @operator.ParameterTypes, @operator.IsParams, frame, expression.Operands, constructorParameters.Length);

				object result = @operator.Evaluate(parameters, constructorParameters);

				//if (result is Widgets.Widget)
				//    ((Widgets.Widget)result).Initialize(frame, style);

				return result;
			}

			if (element is string)
			{
				string s = (string)element;
				if (frame.Values.ContainsKey(s))
					return frame.Values[s];
			}

			if (element is object[])
			{
				object[] array = (object[])element;
				for (int i = 0; i < array.Length; i++)
					array[i] = EvaluateExpression(array[i], frame);
			}

			return element;
		}

		private object[] LoadParameters(string[] parameterNames, System.Type[] parameterTypes, bool isParams, Scripting.Frame frame, List<object> operands, int start)
		{
			object[] parameters = new object[parameterTypes.Length];
			for (int i = 0; i < parameterTypes.Length; i++)
			{
				if (i == 0 && parameterNames[i].Equals("name"))
				{
					parameters[i] = Scripting.Convert.ChangeType(operands[i + start], parameterTypes[i]);
				}
				else if (isParams && i == parameterTypes.Length - 1)
				{
					System.Array array = System.Array.CreateInstance(parameterTypes[i].GetElementType(), operands.Count - (parameterTypes.Length - 1) - start);
					for (int j = parameterTypes.Length - 1 + start; j < operands.Count; j++)
						array.SetValue(Scripting.Convert.ChangeType(EvaluateExpression(operands[j + start], frame), parameterTypes[i].GetElementType()), j - (parameterTypes.Length - 1));
					parameters[i] = array;
				}
				else
				{
					parameters[i] = Scripting.Convert.ChangeType(EvaluateExpression(operands[i + start], frame), parameterTypes[i]);
				}
			}
			return parameters;
		}

		private static bool CanEvaluate(Expression expression, Evaluator evaluator, Dictionary<string, string> variables, Dictionary<string, object> frame)
		{
			//for (int i = (evaluator.AssignmentOperators.ContainsKey(expression.OperatorOrValue) ? 1 : 0); i < expression.Operands.Count; i++)
			for (int i = (evaluator.InfixOperators.ContainsKey(expression.OperatorOrValue) ? 1 : 0); i < expression.Operands.Count; i++)
			{
				object operand = expression.Operands[i];
				if (operand is string)
				{
					string s = (string)operand;
					if (variables.ContainsKey(s) && !frame.ContainsKey(s))
						return false;
				}
				else if (operand is Expression)
				{
					Expression e = (Expression)operand;
					if (!CanEvaluate(e, evaluator, variables, frame))
						return false;
				}
			}
			return true;
		}

		private static Dictionary<string, string> GetVariables(List<Expression> expressions, Evaluator evaluator)
		{
			Dictionary<string, string> variables = new Dictionary<string, string>();
			foreach (Expression expression in expressions)
				GetVariables(expression, evaluator, variables);
			return new Dictionary<string, string>(variables);
		}

		private static void GetVariables(Expression expression, Evaluator evaluator, Dictionary<string, string> variables)
		{
			if (evaluator.AssignmentOperators.ContainsKey(expression.OperatorOrValue))
				variables.Add((string)expression.Operands[0], (string)expression.Operands[0]);
			foreach (object operand in expression.Operands)
				if (operand is Expression)
					GetVariables((Expression)operand, evaluator, variables);
		}

		private class Operator
		{
			private readonly TypeResolver _typeResolver;
			private readonly string _fullyQualifiedFunctionName;
			private System.Reflection.MethodBase _methodBase;
			private string[] _constructorNames;
			private Type[] _constructorTypes;
			private bool _constructorIsParams;
			private string[] _parameterNames;
			private Type[] _parameterTypes;
			private bool _isParams;

			public Operator(TypeResolver typeResolver, string fullyQualifiedFunctionName)
			{
				_typeResolver = typeResolver;
				_fullyQualifiedFunctionName = fullyQualifiedFunctionName;
			}

			public string Name
			{
				get
				{
					if (_methodBase == null)
						Load();
					if (_methodBase.Name.Equals(".ctor"))
						return _methodBase.ReflectedType.Name;
					return _methodBase.Name;
				}
			}

			public string[] ConstructorNames
			{
				get
				{
					if (_constructorNames == null)
						Load();
					return _constructorNames;
				}
			}

			public Type[] ConstructorTypes
			{
				get
				{
					if (_constructorTypes == null)
						Load();
					return _constructorTypes;
				}
			}

			public bool ConstructorIsParams
			{
				get
				{
					if (_methodBase == null)
						Load();
					return _constructorIsParams;
				}
			}

			public string[] ParameterNames
			{
				get
				{
					if (_parameterNames == null)
						Load();
					return _parameterNames;
				}
			}

			public Type[] ParameterTypes
			{
				get
				{
					if (_parameterTypes == null)
						Load();
					return _parameterTypes;
				}
			}

			public bool IsParams
			{
				get
				{
					if (_methodBase == null)
						Load();
					return _isParams;
				}
			}

			public object Evaluate(object[] args, object[] constructorArgs)
			{
				if (_methodBase == null)
					Load();
				if (_methodBase is System.Reflection.ConstructorInfo)
					return ((System.Reflection.ConstructorInfo)_methodBase).Invoke(args);
				if (_methodBase.IsStatic)
				{
					if (constructorArgs.Length > 0)
						throw new System.Exception();
					return _methodBase.Invoke(null, args);
				}
				return _methodBase.Invoke(Activator.CreateInstance(_methodBase.ReflectedType, constructorArgs), args);
			}

			private void Load()
			{
				_methodBase = _typeResolver.ResolveFunction(_fullyQualifiedFunctionName);

				System.Reflection.ParameterInfo[] parameters = _methodBase.GetParameters();

				_parameterNames = new string[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
					_parameterNames[i] = parameters[i].Name;

				_parameterTypes = new Type[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
					_parameterTypes[i] = parameters[i].ParameterType;

				if (_methodBase is System.Reflection.MethodInfo)
				{
					System.Reflection.ConstructorInfo[] constructors = _methodBase.ReflectedType.GetConstructors();
					if (constructors.Length == 1)
					{
						System.Reflection.ParameterInfo[] constructorParameters = constructors[0].GetParameters();

						_constructorNames = new string[constructorParameters.Length];
						for (int i = 0; i < constructorParameters.Length; i++)
							_constructorNames[i] = constructorParameters[i].Name;

						_constructorTypes = new Type[constructorParameters.Length];
						for (int i = 0; i < constructorParameters.Length; i++)
							_constructorTypes[i] = constructorParameters[i].ParameterType;

						if (_constructorTypes.Length > 0)
							_constructorIsParams = constructorParameters[constructorParameters.Length - 1].GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0;
					}
				}

				if (_parameterTypes.Length > 0)
					_isParams = parameters[parameters.Length - 1].GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0;
			}
		}

	}
}
