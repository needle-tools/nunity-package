using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace NUnityPackage.Core
{
	[Serializable]
	public class RegistrySearchResult
	{
		public List<RegistryObject> objects = new List<RegistryObject>();

	}

	[Serializable]
	public class RegistryObject
	{
		public Package package;
		
		public class Package
		{
			public string name;
			[JsonProperty("dist-tags")]
			public Dictionary<string, string> distTags = new Dictionary<string, string>();
			public Dictionary<string, string> versions = new Dictionary<string, string>();
		}
	}

	public static class RegistryExtensions
	{
		public static RegistrySearchResult ToRegistryResult(this NugetSearchResult nugetResult)
		{
			if (nugetResult == null) return null;
			var rr = new RegistrySearchResult();
			if (nugetResult.data == null) return rr;
			foreach (var res in nugetResult.data)
			{
				var obj = new RegistryObject();
				var package = obj.package = new RegistryObject.Package();
				rr.objects.Add(obj);
				package.name = res.id.ToLowerInvariant();
				var ver = res.versions.LastOrDefault();
				if (ver != null)
				{
					var version = ver.version;
					package.versions.Add(version, "latest");
					package.distTags.Add("latest", version);
				}
			}
			return rr;
		}
	}
}