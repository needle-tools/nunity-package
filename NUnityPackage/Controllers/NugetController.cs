using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
			var p = await UnityPackageBuilder.Package();
			return File(p, "application/zip", "test.gzip");
			var bytes = await _nuget.GetDll(packageName);
			if (bytes != null)
				return File(bytes, "application/octet-stream", packageName + ".dll");
			return null;
		}

		// [HttpGet]
		// public async void Get(string packageName)
		// {
		// 	HttpContext.Response.ContentType = "application/x-gzip";
		// 	var xml = "<xml/>";
		// 	using (var gzipStream = new GZipStream(HttpContext.Response.Body, CompressionMode.Compress))
		// 	{
		// 		HttpContext.Response.ContentType = "application/zip";
		// 		// HttpContext.Response.Headers.Add();
		// 		var buffer = Encoding.UTF8.GetBytes(xml);
		// 		await gzipStream.WriteAsync(buffer, 0, buffer.Length);
		// 	}
		// }
	}
}