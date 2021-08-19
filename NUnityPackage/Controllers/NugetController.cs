using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnityPackage.Core;

namespace NUnityPackage.Controllers
{
	[ApiController]
	[Route("[controller]/{packageName?}")]
	public class NugetController : ControllerBase
	{
		private readonly ILogger<NugetController> _logger;
		private readonly NugetApi _nuget = new NugetApi();

		public NugetController(ILogger<NugetController> logger)
		{
			_logger = logger;
		}

		// [HttpGet]
		// public async Task<string[]> Get(string packageName)
		// {
		// 	var archive = await GetArchive(packageName);
		// 	var entries = new List<string>();
		// 	entries.AddRange(archive.Entries.Select(e => e.FullName));
		// 	return entries.ToArray();
		// }

		[HttpGet]
		public async Task<ActionResult> Get(string packageName)
		{
			var bytes = await _nuget.GetDll(packageName);
			if (bytes != null)
				return File(bytes, "application/octet-stream", packageName + ".dll");
			return null;
		}
	}
}