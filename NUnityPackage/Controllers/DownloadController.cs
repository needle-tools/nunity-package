using System;
using System.Threading.Tasks;
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


		[HttpGet("internal/clear/all")]
		public async Task<ActionResult> ClearAll()
		{
			await Globals.Cache.ClearCachedFiles();
			return Ok("done");
		}

		[HttpGet("{packageName}/-/{packageId}")]
		public async Task<ActionResult> DownloadPackage(string packageName, string packageId)
		{
			_logger.LogInformation("Request download: " + packageId);

			byte[] bytes;
			bytes = await Globals.Cache.TryDownloadFile(packageId);
			if (bytes != null)
			{
				_logger.LogInformation("Resolved from cache: " + packageId);
			}
			else
			{
				var version = packageId.Substring(packageName.Length+1, packageId.Length - packageName.Length - 5);
				bytes = await UnityPackageBuilder.BuildTgzPackage(packageName, version, packageId, Globals.Cache, _logger);
			}

			if (bytes == null)
			{
				_logger.LogError("Failed downloading " + packageId);
				return Problem("could not find " + packageId);
			}

			return File(bytes, "application/zip", packageId);
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