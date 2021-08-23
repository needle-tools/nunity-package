using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnityPackage.Core.Interfaces;

namespace NUnityPackage.Core
{
	// https://packages.glitch.me/com.needle.dummypackage
	public class RegistryPackageResult
	{
		public string name;
		public Dictionary<string, Version> versions = new Dictionary<string, Version>();

		public class Version : IHaveUnityDependencies
		{
			public string name;
			public string displayName;
			public string version;
			public string readme;
			public string readmeFilename;
			public string description;
			public Dist dist;
			public Dictionary<string, string> dependencies { get; set; }

			public class Dist
			{
				// TODO: download package once on request and calculate sha1 sum
				public string shasum;
				public string tarball;
			}
		}
	}

	public static class RegistryPackageResultExtensions
	{
		public static async Task<RegistryPackageResult> ToRegistryPackageResult(this NugetPackageRegistrationResult res,
			int maxVersions,
			string downloadEndpoint,
			Caching cache,
			ILogger logger = null)
		{
			if (res == null) return null;
			var rr = new RegistryPackageResult();
			if (res.count <= 0 || res.items == null) return rr;
			if (!downloadEndpoint.EndsWith("/"))
				downloadEndpoint += "/";
			foreach (var it in res.items)
			{
				if (it.items == null) continue;
				if (rr.versions.Count >= maxVersions) break;
				foreach (var p in it.items)
				{
					if (p == null) continue;
					if (rr.versions.Count >= maxVersions) break;
					var details = p.catalogEntry;
					var id = details.id.ToLowerInvariant();
					rr.name = id;
					if (!rr.versions.ContainsKey(details.version))
					{
						if (rr.versions.Count >= maxVersions) break;

						var fileId = $"{id}-{details.version}.tgz";

						Shasum sha = default;
						if (cache != null)
						{
							sha = await Shasum.TryGet(fileId, cache);
							if (sha == null)
							{
								await UnityPackageBuilder.BuildTgzPackage(id, details.version, fileId, cache, logger);
								sha = await Shasum.TryGet(fileId, cache);
								if (sha == null)
								{
									logger?.LogError($"Built package {fileId} but sha is still not found, will skip this");
									continue;
								}
								logger?.LogInformation("Cached sha: " + id + " = " + sha.shasum);
							}
						}

						var dist = new RegistryPackageResult.Version.Dist
						{
							tarball = downloadEndpoint + id + "/-/" + fileId,
							shasum = sha?.shasum
						};
						var inst = new RegistryPackageResult.Version()
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
							dist = dist
						};
						
						if(details.dependencyGroups != null)
							UnityPackageBuilder.AddRelevantDependencies(details.dependencyGroups, inst, logger);


						rr.versions.Add(details.version, inst);
					}
				}
			}

			return rr;
		}
	}
}