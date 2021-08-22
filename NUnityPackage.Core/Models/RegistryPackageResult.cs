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
				// TODO: download package once on request and calculate sha1 sum
				public string shasum = "153eda12187d4ec6fd4d983d875f10626b148ff0";
				public string tarball;
			}
		}
	}

	public static class RegistryPackageResultExtensions
	{
		public static RegistryPackageResult ToRegistryPackageResult(this NugetPackageRegistrationResult res, string downloadEndpoint)
		{
			if (res == null) return null;
			var rr = new RegistryPackageResult();
			if (res.count <= 0 || res.items == null) return rr;
			if (!downloadEndpoint.EndsWith("/"))
				downloadEndpoint += "/";
			foreach (var it in res.items)
			{
				if (it.items == null) continue;
				foreach (var p in it.items)
				{
					if (p == null) continue;
					var details = p.catalogEntry;
					var id = details.id.ToLowerInvariant();
					rr.name = id;
					if (!rr.versions.ContainsKey(details.version))
					{
						rr.versions.Add(details.version, new RegistryPackageResult.Version()
						{
							name = id,
							description = details.description,
							displayName =
								!string.IsNullOrWhiteSpace(details.title)
									? details.title
									: !string.IsNullOrWhiteSpace(details.id)
										? details.id
										: id,
							version = details.version,
							dist = new RegistryPackageResult.Version.Dist()
							{
								tarball = downloadEndpoint + id + "/-/" + id + "-" + details.version + ".tgz"
								
							}
						});
					}
				}
			}

			return rr;
		}
	}
}