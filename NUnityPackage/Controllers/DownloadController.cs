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
	[Route("")]
	public class DownloadController : ControllerBase
	{
		private readonly ILogger<DownloadController> _logger;

		public DownloadController(ILogger<DownloadController> logger)
		{
			_logger = logger;
		}

		[HttpGet("{packageName}/-/{packageId}")]
		public async Task<ActionResult> Get(string packageName, string packageId)
		{
			_logger.LogInformation("Download " + packageId);
			
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

				_logger.LogInformation("Downloading " + packageName + " as " + packageId);
				var p = await UnityPackageBuilder.Package(unityPackage, packageName + ".dll", dllStream);
				_logger.LogInformation("Return file: " + packageId + ", " + p.Length + " bytes");
				return File(p, "application/zip", packageId);
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