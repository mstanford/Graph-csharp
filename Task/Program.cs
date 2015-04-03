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

namespace Task
{
	class Program
	{

		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

			Log log = new Log(System.Console.Out);

			try
			{
				List<Ini.Section> configuration = Ini.ParseList(args[0]);

				if (configuration.Count == 0)
					throw new System.Exception("Configuration file is empty.");

				if (configuration.Count == 1)
					throw new System.Exception("No tasks are defined.");

				Dictionary<string, object> state = new Dictionary<string, object>();

				Graph.Widgets.Style style = new Graph.Widgets.Style();

				System.IO.Stream definitionStream = typeof(Graph.Scripting.Evaluator).Assembly.GetManifestResourceStream(typeof(Graph.Scripting.Evaluator).Namespace + ".Evaluator.def");
				byte[] buffer = new byte[definitionStream.Length];
				if (definitionStream.Read(buffer, 0, buffer.Length) != buffer.Length)
					throw new System.Exception();
				definitionStream.Close();

				Run(args[0], configuration, state, style, buffer, log);
			}
			catch (System.Exception exception)
			{
				log.Indentation = 0;
				log.Write(exception);

				if (System.Diagnostics.Debugger.IsAttached)
					System.Console.ReadLine();
			}
		}

		static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			string filename;
			int index = args.Name.IndexOf(',');
			if (index == -1)
				filename = System.IO.Path.Combine(Environment.CurrentDirectory, args.Name);
			else
				filename = System.IO.Path.Combine(Environment.CurrentDirectory, args.Name.Substring(0, index));
			if (System.IO.File.Exists(filename + ".dll"))
				return System.Reflection.Assembly.LoadFile(filename + ".dll");
			if (System.IO.File.Exists(filename + ".exe"))
				return System.Reflection.Assembly.LoadFile(filename + ".exe");
			return null;
		}

		public static void Run(string file, List<Ini.Section> configuration, Dictionary<string, object> state, Graph.Widgets.Style style, byte[] buffer, Log log)
		{
			Ini.Section GLOBAL = configuration[0];

			log.WriteLine("BEGIN " + GLOBAL.Name + " " + file);
			log.Indentation++;

			Dictionary<string, string> globalConfiguration = new Dictionary<string, string>();
			foreach (KeyValuePair<string, string> kvp in GLOBAL.Entries)
				globalConfiguration[kvp.Key] = kvp.Value;

			for (int i = 1; i < configuration.Count; i++)
			{
				log.WriteLine("");
				log.WriteLine("BEGIN TASK " + configuration[i].Name);
				log.Indentation++;

				Dictionary<string, string> taskConfiguration2 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				foreach (KeyValuePair<string, string> kvp in GLOBAL.Entries)
					taskConfiguration2[kvp.Key] = kvp.Value;
				foreach (KeyValuePair<string, string> kvp in configuration[i].Entries)
					taskConfiguration2[kvp.Key] = kvp.Value;

				Dictionary<string, string> taskConfiguration = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				foreach (KeyValuePair<string, string> kvp in GLOBAL.Entries)
					taskConfiguration[kvp.Key] = ReplaceVariableTags(kvp.Value, taskConfiguration2, false);
				foreach (KeyValuePair<string, string> kvp in configuration[i].Entries)
					taskConfiguration[kvp.Key] = ReplaceVariableTags(kvp.Value, taskConfiguration2, false);

				if (taskConfiguration.ContainsKey("Task"))
				{
					System.Reflection.MethodInfo method = GetMethod(taskConfiguration["Task"]);
					if (method == null)
						throw new System.Exception("Method does not exist: " + taskConfiguration["Task"]);
					Invoke(method, taskConfiguration, log, state);
				}
				else if (taskConfiguration.ContainsKey("Graph"))
				{
					System.IO.MemoryStream definitionStream = new System.IO.MemoryStream(buffer);
					System.IO.StreamReader definitionReader = new System.IO.StreamReader(definitionStream);
					Graph.Scripting.Evaluator evaluator = new Graph.Scripting.Evaluator(definitionReader);

					System.IO.TextReader graphReader = System.IO.File.OpenText(taskConfiguration["Graph"]);
					List<Graph.Scripting.Expression> expressions = new List<Graph.Scripting.Expression>(Graph.Scripting.Parser.Parse(graphReader, evaluator.InfixOperators));
					graphReader.Close();

					EvaluateGraph(evaluator, expressions, taskConfiguration, state, style);
				}
				else if (taskConfiguration.ContainsKey("MultiGraph"))
				{
					string keyName = taskConfiguration["Key"];
					List<string> keys = (List<string>)state["Keys." + keyName];
					foreach (string key in keys)
					{
						taskConfiguration[keyName] = key;

						Dictionary<string, string> taskConfiguration3 = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
						foreach (KeyValuePair<string, string> kvp in GLOBAL.Entries)
							taskConfiguration3[kvp.Key] = ReplaceVariableTags(kvp.Value, taskConfiguration, true);
						foreach (KeyValuePair<string, string> kvp in configuration[i].Entries)
							taskConfiguration3[kvp.Key] = ReplaceVariableTags(kvp.Value, taskConfiguration, true);

						taskConfiguration3[keyName] = key;

						System.IO.MemoryStream definitionStream = new System.IO.MemoryStream(buffer);
						System.IO.StreamReader definitionReader = new System.IO.StreamReader(definitionStream);
						Graph.Scripting.Evaluator evaluator = new Graph.Scripting.Evaluator(definitionReader);

						System.IO.TextReader graphReader = System.IO.File.OpenText(taskConfiguration["MultiGraph"]);
						List<Graph.Scripting.Expression> expressions = new List<Graph.Scripting.Expression>(Graph.Scripting.Parser.Parse(graphReader, evaluator.InfixOperators));
						graphReader.Close();

						EvaluateGraph(evaluator, expressions, taskConfiguration3, state, style);
					}
				}
				else if (taskConfiguration.ContainsKey("Script"))
				{
					List<Ini.Section> scriptConfiguration = Ini.ParseList(taskConfiguration["Script"]);
					scriptConfiguration.Insert(0, new Ini.Section(GLOBAL.Name, new Dictionary<string,string>()));
					foreach (KeyValuePair<string, string> kvp in GLOBAL.Entries)
						scriptConfiguration[0].Entries[kvp.Key] = kvp.Value;
					foreach (KeyValuePair<string, string> kvp in taskConfiguration)
						scriptConfiguration[0].Entries[kvp.Key] = kvp.Value;
					scriptConfiguration[0].Entries.Remove("Script");

					Dictionary<string, object> scriptState = new Dictionary<string, object>();
					foreach (KeyValuePair<string, object> kvp in state)
						scriptState.Add(kvp.Key, kvp.Value);

					log.Indentation++;
					Run(taskConfiguration["Script"], scriptConfiguration, scriptState, style, buffer, log);
					log.Indentation--;
				}
				else if (taskConfiguration.ContainsKey("MultiScript"))
				{
					List<Ini.Section> scriptConfiguration = Ini.ParseList(taskConfiguration["MultiScript"]);
					scriptConfiguration.Insert(0, new Ini.Section(GLOBAL.Name, new Dictionary<string, string>()));
					foreach (KeyValuePair<string, string> kvp in GLOBAL.Entries)
						scriptConfiguration[0].Entries[kvp.Key] = kvp.Value;
					foreach (KeyValuePair<string, string> kvp in taskConfiguration)
						scriptConfiguration[0].Entries[kvp.Key] = kvp.Value;

					string keyName = taskConfiguration["Key"];
					List<string> keys = (List<string>)state["Keys." + keyName];
					foreach (string key in keys)
					{
						scriptConfiguration[0].Entries[keyName] = key;

						Dictionary<string, object> scriptState = new Dictionary<string, object>();
						foreach (KeyValuePair<string, object> kvp in state)
							scriptState.Add(kvp.Key, kvp.Value);

						log.Indentation++;
						Run(taskConfiguration["MultiScript"], scriptConfiguration, scriptState, style, buffer, log);
						log.Indentation--;
					}
				}
				else
				{
					throw new System.Exception("Task is undefined.");
				}

				log.Indentation--;
				log.WriteLine("TASK " + configuration[i].Name + " COMPLETE");
			}

			log.Indentation--;
			log.WriteLine("");
			log.WriteLine(GLOBAL.Name + " COMPLETE");
		}

		static void EvaluateGraph(
			Graph.Scripting.Evaluator evaluator, 
			List<Graph.Scripting.Expression> expressions, 
			Dictionary<string, string> taskConfiguration,
			Dictionary<string, object> state, 
			Graph.Widgets.Style style)
		{
			Graph.Scripting.Frame frame = new Graph.Scripting.Frame();
			foreach (KeyValuePair<string, string> kvp in taskConfiguration)
				frame.Update("$" + kvp.Key, kvp.Value);
			foreach (KeyValuePair<string, object> kvp in state)
				frame.Update("$" + kvp.Key, kvp.Value);

			List<Graph.Widgets.Widget> widgets = new List<Graph.Widgets.Widget>();
			evaluator.EvaluateScript(expressions, frame, widgets);

			Graph.Widgets.Column column = new Graph.Widgets.Column(widgets.ToArray());
			column.Initialize(frame, style);

			System.Drawing.Size size = new System.Drawing.Size(1024, 768);
			if (frame.Values.ContainsKey("$$width"))
				size.Width = Graph.Convert.ToInt32(frame.Values["$$width"]);
			if (frame.Values.ContainsKey("$$height"))
				size.Height = Graph.Convert.ToInt32(frame.Values["$$height"]);

			column.Render(taskConfiguration["Path"], size.Width, size.Height);
		}

		static string ReplaceVariableTags(string s, Dictionary<string, string> variables, bool throwException)
		{
			int index = s.IndexOf('{');
			if (index > -1)
			{
				string s1 = s.Substring(0, index);
				string tag = s.Substring(index + 1);

				int index2 = tag.IndexOf('}');
				string s2 = tag.Substring(index2 + 1);
				tag = tag.Substring(0, index2);
				if (!variables.ContainsKey(tag))
				{
					if (throwException)
						throw new System.Exception("Variable is undefined: " + tag);
					else
						return s;
				}
				return ReplaceVariableTags(s1 + variables[tag] + s2, variables, throwException);
			}
			return s;
		}

		static object Invoke(System.Reflection.MethodInfo method, Dictionary<string, string> configuration, System.IO.TextWriter log, Dictionary<string, object> state)
		{
			System.Reflection.ParameterInfo[] parameterInfos = method.GetParameters();
			object[] values = new object[parameterInfos.Length];
			for (int i = 0, j = 0; i < parameterInfos.Length; i++)
			{
				switch (parameterInfos[i].Name)
				{
					case "CONFIGURATION":
						values[i] = configuration;
						break;
					case "LOG":
						values[i] = log;
						break;
					case "STATE":
						values[i] = state;
						break;
					default:
						if (!configuration.ContainsKey(parameterInfos[i].Name))
							throw new System.Exception("Parameter is undefined: " + parameterInfos[i].Name);
						values[i] = Convert(configuration[parameterInfos[i].Name], parameterInfos[i].ParameterType);
						j++;
						break;
				}
			}
			if (method.IsStatic)
				return method.Invoke(null, values);
			return method.Invoke(Create(method.ReflectedType, configuration, log, state), values);
		}

		static object Create(System.Type type, Dictionary<string, string> configuration, System.IO.TextWriter log, Dictionary<string, object> state)
		{
			System.Reflection.ConstructorInfo[] constructors = type.GetConstructors();
			if (constructors.Length != 1)
				throw new System.Exception();
			System.Reflection.ParameterInfo[] parameterInfos = constructors[0].GetParameters();
			object[] values = new object[parameterInfos.Length];
			for (int i = 0, j = 0; i < parameterInfos.Length; i++)
			{
				switch (parameterInfos[i].Name)
				{
					//TODO DELETE
					//case "CONFIGURATION":
					//    values[i] = configuration;
					//    break;
					//case "LOG":
					//    values[i] = log;
					//    break;
					//case "STATE":
					//    values[i] = state;
					//    break;
					default:
						if (!configuration.ContainsKey(parameterInfos[i].Name))
							throw new System.Exception("Parameter is undefined: " + parameterInfos[i].Name);
						values[i] = Convert(configuration[parameterInfos[i].Name], parameterInfos[i].ParameterType);
						j++;
						break;
				}
			}
			return Activator.CreateInstance(type, values);
		}

		static object Convert(string value, Type conversionType)
		{
			switch (conversionType.FullName)
			{
				case "System.Double":
					return double.Parse(value);
				case "System.Int32":
					return int.Parse(value);
				case "System.String":
					return value;
				case "System.String[]":
					return value.Split(';');
				default:
					throw new System.Exception("Unable to convert to " + conversionType.FullName);
			}
		}

		static System.Reflection.MethodInfo GetMethod(string s)
		{
			try
			{
				string[] asz = s.Split(',');
				System.Type type = GetTypeFromMethod(asz);
				System.Reflection.MethodInfo method = type.GetMethod(asz[0].Substring(asz[0].LastIndexOf('.') + 1));
				if (method == null)
					throw new System.Exception("Method is undefined: " + s);
				return method;
			}
			catch (System.Exception exception)
			{
				throw new System.Exception(exception.Message + "  " + s);
			}
		}

		static System.Type GetTypeFromMethod(params string[] asz)
		{
			return GetType(asz[0].Substring(0, asz[0].LastIndexOf('.')), asz[1]);
		}

		static System.Type GetType(string typeName, string assemblyName)
		{
			System.Type type2 = System.Type.GetType(typeName + "," + assemblyName);
			if (type2 != null)
				return type2;
			if (System.IO.File.Exists(assemblyName + ".dll"))
			{
				System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(System.IO.Path.GetFullPath(assemblyName + ".dll"));
				foreach (System.Type type in assembly.GetTypes())
					if (type.FullName.Equals(typeName))
						return type;
			}
			else if (System.IO.File.Exists(assemblyName + ".exe"))
			{
				System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(System.IO.Path.GetFullPath(assemblyName + ".exe"));
				foreach (System.Type type in assembly.GetTypes())
					if (type.FullName.Equals(typeName))
						return type;
			}
			throw new System.Exception("Unknown type: " + typeName);
		}

	}
}
