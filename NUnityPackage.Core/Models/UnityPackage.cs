using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using NUnityPackage.Core.Interfaces;

namespace NUnityPackage.Core
{
	[Serializable]
	public class UnityPackage : IHaveUnityDependencies
	{
		public string name;
		public string version;
		public string displayName;
		public string description;
		public string author;
		public string changelog;
		public string documentationUrl;
		public string license;
		public string licensesUrl;
		public Dictionary<string, string> dependencies { get; set; }

		[JsonIgnore]
		public string id => name + "-" + version;
	}

	public static class UnityPackageExtensions
	{
		public static string ToUnityPackageName(this NugetSpecification nuget, string packageName)
		{
			var name = "com";
			if (nuget?.metadata?.owners != null && nuget.metadata.owners.Length > 0)
			{
				var mainOwner = nuget.metadata.owners.Split(',').First().Trim();
				// remove brackets, some owners have some company url in brackets e.g. serilog.exceptions
				var splitIndex = mainOwner.IndexOf("(", StringComparison.Ordinal);
				if (splitIndex > 0) mainOwner = mainOwner.Substring(0, splitIndex);
				name += $".{mainOwner.RemoveSpecialCharacters()}";
			}
			name += $".{packageName.RemoveSpecialCharacters()}";
			name = name.ToLowerInvariant();
			return name;
		}
		
		private static string RemoveSpecialCharacters(this string str)
		{
			return Regex.Replace(str, @"[^a-zA-Z0-9_\-]+", "-", RegexOptions.Compiled);
		}
	}
}