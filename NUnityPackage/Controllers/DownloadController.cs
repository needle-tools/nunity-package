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
	[Route("[controller]")]
	public class DownloadController : ControllerBase
	{
		private readonly ILogger<DownloadController> _logger;

		public DownloadController(ILogger<DownloadController> logger)
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
		public async Task<string> Get()
		{
			return "hello nuget";
		}

		[HttpGet("{packageName}")]
		public async Task<ActionResult> Get(string packageName)
		{
			_logger.LogInformation("Download " + packageName);
			
			using (var package = new NugetPackage(packageName))
			{
				var spec = await package.GetSpecification();
				var dllStream = await package.GetDllStream();
				
				var meta = spec.metadata;
				var unityPackage = new UnityPackage();
				unityPackage.name = spec.ToUnityPackageName(packageName);
				unityPackage.version =  meta.version;
				unityPackage.displayName = meta.title ?? packageName;
				unityPackage.description = meta.description;
				unityPackage.author = meta.authors;
				unityPackage.changelog = meta.releaseNotes;
				unityPackage.license = meta.license;
				unityPackage.licensesUrl = meta.licenseUrl;
				unityPackage.documentationUrl = meta.projectUrl;

				var resultName = packageName + "-" + unityPackage.version + ".tgz";
				_logger.LogInformation("Downloading " + packageName + " as " + resultName);
				var p = await UnityPackageBuilder.Package(unityPackage, packageName + ".dll", dllStream);
				return File(p, "application/zip", resultName);
			}
			// var bytes = await _nuget.GetDll(packageName);
			// if (bytes != null)
			// 	return File(bytes, "application/octet-stream", packageName + ".dll");
			// return null;
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