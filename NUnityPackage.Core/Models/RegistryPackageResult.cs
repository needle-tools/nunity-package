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
				public string integrity = "sha512-OksHxW6aKf7KaANm1RGpUhQaCMEgkfjH3IGuU5oXKTvRzfVy5AiD6RNAnjAxYVphnakle8jKLPzoa1ux65yZIQ==";
				public string shasum = "59a5ab572f9a1dfd434f8b8613f3b6205ba63a19";
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
								tarball = downloadEndpoint + id + "/-/" + id + "-" + details.version + ".tgz",
								shasum = "0eaf2b4161171679cc62a2bca9eb1d52e63780a0"
							}
						});
					}
				}
			}

			return rr;
		}
	}
}