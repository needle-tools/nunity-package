using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NUnityPackage.Core
{
	public static class NugetApi
	{
		// https://docs.microsoft.com/en-gb/nuget/api/search-query-service-resource
		/*
		 * examples:
		 * https://api-v2v3search-0.nuget.org/query?q=system.drawing&prerelease=
		 * https://api-v2v3search-0.nuget.org/query?q=packageid:system.drawing&prerelease=false
		 */

		public const string BaseSearchUrl = "https://api-v2v3search-0.nuget.org/query?";
		public const int MaxResults = 3000;

		public static async Task<string> GetSearchResultsJson(params string[] ids)
		{
			using (var client = new WebClient())
			{
				var idsString = string.Join(" ", ids.Select(id => "id:" + id));
				var url = $"{BaseSearchUrl}q={idsString}&take={MaxResults}";
				var res = await client.DownloadStringTaskAsync(new Uri(url));
				return res;
			}
		}


		public static async Task<NugetSearchResult> GetSearchResults(params string[] ids)
		{
			var json = await GetSearchResultsJson(ids);
			if (json != null)
			{
				var obj = JsonConvert.DeserializeObject<NugetSearchResult>(json);
				return obj;
			}
			return null;
		}

		// https://api.nuget.org/v3/registration5-semver1/system.drawing.common/index.json
		private static string PackageRegistrationUrl(string packageName) => $"https://api.nuget.org/v3/registration5-semver1/{packageName}/index.json";
		
		public static async Task<string> GetPackageRegistrationInfoRaw(string packageName, ILogger logger = null)
		{
			try
			{
				using var client = new WebClient();
				var url = PackageRegistrationUrl(packageName);
				var res = await client.DownloadStringTaskAsync(new Uri(url));
				return res;
			}
			catch (WebException webex)
			{
				logger?.LogWarning(packageName + ": " + webex.Message);
			}

			return null;
		}
		
		
		public static async Task<NugetPackageRegistrationResult> GetPackageRegistrationResult(string packageName)
		{
			var json = await GetPackageRegistrationInfoRaw(packageName);
			if (json != null)
			{
				var obj = JsonConvert.DeserializeObject<NugetPackageRegistrationResult>(json);
				if (obj != null) obj = await ResolvePaginatedResult(obj);
				return obj;
			}
			return null;
		}

		public static async Task<NugetPackageRegistrationResult> ResolvePaginatedResult(NugetPackageRegistrationResult result, ILogger logger = null)
		{
			if (result == null || result.items == null) return result;
			var newItems = new List<NugetCatalogPageItem>();
			for (var index = result.items.Length - 1; index >= 0; index--)
			{
				var it = result.items[index];
				if (it == null) continue;
				if (it.items != null && it.items.Length > 0) continue;
				try
				{
					using var client = new WebClient();
					logger?.LogInformation("Request paginated result: " + it.id);
					var res = await client.DownloadStringTaskAsync(new Uri(it.id));
					if (res != null)
					{
						var obj = JsonConvert.DeserializeObject<NugetCatalogPageItem>(res);
						if (obj != null)
						{
							if (obj.items != null && obj.items.Length > 0)
							{
								logger?.LogInformation("Add paginated results: " + obj.items.Length + " from " + it.id);
								newItems.Add(obj);
							}
						}
					}
				}
				catch (WebException webex)
				{
					logger?.LogWarning(webex.Message);
				}
				catch (Exception ex)
				{
					logger?.LogError(ex.Message);
				}
			}

			if (newItems.Count > 0)
			{
				var newResult = new NugetPackageRegistrationResult();
				newResult.items = newItems.ToArray();
				newResult.count = newResult.items.Length;
				return newResult;
			}

			return result;
		}
	}
}