using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUnityPackage.Core;

namespace NUnityPackage.Controllers
{
	[ApiController]
	[Route("")]
	public class RegistryController : ControllerBase
	{
		private readonly ILogger<RegistryController> _logger;

		public RegistryController(ILogger<RegistryController> logger)
		{
			_logger = logger;
		}
		
		// /-/v1/search
		// -/v1/search?text=com.needle

		[HttpGet("-/v1/search")]
		public async Task<IActionResult> Search(string text)
		{
			_logger.LogInformation("Search: " + text);
			var nuget = await NugetApi.GetSearchResults(text);
			var rr = nuget.ToRegistryResult();
			
			_logger.LogInformation($"Search for " + text + " returned " + rr?.objects?.Count + " results\n" 
			                       // + string.Join("\n", rr?.objects?.Select(o => o.package.name) ?? ArraySegment<string>.Empty)
			                       );
			
			
			var json = JsonConvert.SerializeObject(rr, Formatting.Indented);
			Response.ContentType = "application/json";
			return Content(json);
			// var res = new RegistrySearchResult();
			// return JsonConvert.SerializeObject(res, Formatting.Indented);
		}


		[HttpGet("{packageName}")]
		public async Task<IActionResult> GetPackage(string packageName)
		{
			var url = Request.Scheme + "://" + Request.Host.Host + ":" + Request.Host.Port;
			
			
			_logger.LogInformation("Get " + packageName);
			var packageRes = await NugetApi.GetPackageRegistrationResult(packageName);
			var res = packageRes?.ToRegistryPackageResult(url);
			var json = string.Empty;
			if (res != null)
			{
				_logger.LogInformation("Received package result, building json");
				var settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
				json = JsonConvert.SerializeObject(res, Formatting.Indented, settings);
			}
			else _logger.LogError("Failed getting package result for " + packageName);
			
			Response.ContentType = "application/json";
			return Content(json);
		}

	}
}