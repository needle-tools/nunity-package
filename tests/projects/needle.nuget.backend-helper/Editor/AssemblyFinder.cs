using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Needle
{
	internal static class AssemblyFinder 
	{
		[MenuItem("Tools/Find Valid Assemblies")]
		private static void ValidAssemblies()
		{
			var sys = CompilationPipeline.GetSystemAssemblyDirectories(ApiCompatibilityLevel.NET_4_6);
			var assemblyPaths = new List<string>();
			foreach (var s in sys)
			{
				var assemblies = Directory.GetFiles(s, "*.dll");
				foreach (var asm in assemblies)
				{
					var fn = new FileInfo(asm).Name.Replace(".dll", "");
					if (!assemblyPaths.Contains(fn))
					{
						assemblyPaths.Add(fn);
					}
				}
			}
			

			// var prec = CompilationPipeline.GetAssemblies(AssembliesType.Editor);
			Debug.Log(string.Join(",\n", assemblyPaths.Select(s => "\"" + s + "\"")));
		}
	}
}
