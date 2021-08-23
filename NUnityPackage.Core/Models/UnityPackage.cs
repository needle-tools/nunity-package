using System;
using System.Collections.Generic;
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
				name += $".{nuget.metadata.owners.Replace(",", "-")}";
			name += $".{packageName.Replace('.', '-')}";
			name = name.ToLowerInvariant();
			return name;
		}
	}
}