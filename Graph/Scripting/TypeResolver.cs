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
	public sealed class TypeResolver
	{
		private readonly string[] _paths;
		private readonly Dictionary<string, System.Reflection.Assembly> _assemblies = new Dictionary<string, System.Reflection.Assembly>();
		private readonly Dictionary<string, System.Type> _types = new Dictionary<string, System.Type>();

		public TypeResolver()
		{
            System.Reflection.Assembly assembly = typeof(TypeResolver).Assembly;

            //_path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _paths = new string[] { System.Environment.CurrentDirectory, System.IO.Path.GetDirectoryName(assembly.Location) };
            _assemblies.Add(assembly.GetName().Name, assembly);
		}

		public System.IO.Stream ResolveResource(string fullyQualifiedResourceName)
		{
			int index = fullyQualifiedResourceName.LastIndexOf(',');
			if (index == -1)
				throw new System.Exception("Bad resource definition: " + fullyQualifiedResourceName);
			string resourceName = fullyQualifiedResourceName.Substring(0, index);
			string assemblyName = fullyQualifiedResourceName.Substring(index + 1);
			return ResolveAssembly(assemblyName).GetManifestResourceStream(resourceName);
			//throw new System.Exception("Cannot find resource: " + resourceName);
		}

		public System.Reflection.MethodBase ResolveFunction(string fullyQualifiedFunctionName)
		{
			int index = fullyQualifiedFunctionName.LastIndexOf(',');
			if (index == -1)
				throw new System.Exception("Bad function definition: " + fullyQualifiedFunctionName);
			string functionAndtypeName = fullyQualifiedFunctionName.Substring(0, index);
			string assemblyName = fullyQualifiedFunctionName.Substring(index + 1);
			index = functionAndtypeName.LastIndexOf('.');
			if (index == -1)
				throw new System.Exception("Bad function definition: " + fullyQualifiedFunctionName);
			System.Type type = ResolveType(functionAndtypeName, assemblyName);
			if (type != null)
			{
				//It's a constructor.
				return (System.Reflection.MethodBase)type.GetConstructors()[0];
			}
			string typeName = functionAndtypeName.Substring(0, index);
			string functionName = functionAndtypeName.Substring(index + 1);
			type = ResolveType(typeName, assemblyName);
			if (type != null)
			{
				return (System.Reflection.MethodBase)type.GetMember(functionName)[0];
			}
			throw new System.Exception("Cannot find type: " + typeName);
		}

		private System.Type ResolveType(string typeName, string assemblyName)
		{
			if (_types.ContainsKey(typeName + "," + assemblyName))
			{
				return _types[typeName + "," + assemblyName];
			}
			else if (_assemblies.ContainsKey(assemblyName))
			{
				System.Type type = Resolve(_assemblies[assemblyName], typeName);
				if (type != null)
				{
					_types.Add(typeName + "," + assemblyName, type);
					return type;
				}
			}
			else
			{
				System.Reflection.Assembly assembly = ResolveAssembly(assemblyName);
				System.Type type = Resolve(assembly, typeName);
				if (type != null)
				{
					_types.Add(typeName + "," + assemblyName, type);
					return type;
				}
			}
			return null;
		}

		private System.Type Resolve(System.Reflection.Assembly assembly, string typeName) { return assembly.GetType(typeName); }

		private System.Reflection.Assembly ResolveAssembly(string assemblyName)
		{
            if (_assemblies.ContainsKey(assemblyName))
                return _assemblies[assemblyName];

			//string filename = System.IO.Path.Combine(_path, assemblyName + ".dll");
			//if (!System.IO.File.Exists(filename))
			//    filename = System.IO.Path.Combine(_path, assemblyName + ".exe");
			//if (!System.IO.File.Exists(filename))
			//    throw new System.Exception("Cannot find assembly: " + assemblyName);
			//System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(filename);
			//_assemblies.Add(assemblyName, assembly);

			for (int i = 0; i < _paths.Length; i++)
			{
				string filename = System.IO.Path.Combine(_paths[i], assemblyName + ".dll");
				if (!System.IO.File.Exists(filename))
					filename = System.IO.Path.Combine(_paths[i], assemblyName + ".exe");
				if (System.IO.File.Exists(filename))
				{
					System.Reflection.Assembly assembly = System.Reflection.Assembly.LoadFile(filename);
					_assemblies.Add(assemblyName, assembly);
					break;
				}
			}
			return _assemblies[assemblyName];
		}

	}
}
