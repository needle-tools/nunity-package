using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Apis.Logging;
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
				logger?.Warning(packageName + ": " + webex.Message);
			}

			return null;
		}
		
		
		public static async Task<NugetPackageRegistrationResult> GetPackageRegistrationResult(string packageName)
		{
			var json = await GetPackageRegistrationInfoRaw(packageName);
			if (json != null)
			{
				var obj = JsonConvert.DeserializeObject<NugetPackageRegistrationResult>(json);
				return obj;
			}
			return null;
		}
	}
}