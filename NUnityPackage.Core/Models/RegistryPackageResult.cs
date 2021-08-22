using System.Collections.Generic;

namespace NUnityPackage.Core
{
	// https://packages.glitch.me/com.needle.dummypackage
	public class RegistryPackageResult
	{
		public string name;
		public Dictionary<string, Version> versions = new Dictionary<string, Version>();
		
		public class Version
		{
			public string name;
			public string displayName;
			public string version;
			public string readme;
			public string readmeFilename;
			public string description;
			public Dist dist;

			public class Dist
			{
				public string tarball;
			}
		}

	}

	public static class RegistryPackageResultExtensions
	{
		public static RegistryPackageResult ToRegistryPackageResult(this NugetPackageRegistrationResult res)
		{
			if (res == null) return null;
			var rr = new RegistryPackageResult();
			if (res.count <= 0) return rr;
			foreach (var it in res.items)
			{
				foreach (var p in it.items)
				{
					var details = p.catalogEntry;
					var id = details.id.ToLowerInvariant();
					rr.name = id;
					if (!rr.versions.ContainsKey(details.version))
					{
						rr.versions.Add(details.version, new RegistryPackageResult.Version()
						{
							name = id,
							description = details.description,
							displayName = details.title,
							version = details.version,
							dist = new RegistryPackageResult.Version.Dist()
							{
								tarball = "http://localhost:8080/nuget/" + id
							}
						});
					}
				}
			}

			return rr;
		}
	}
}