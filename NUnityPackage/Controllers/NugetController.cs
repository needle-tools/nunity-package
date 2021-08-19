using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NUnityPackage.Controllers
{
	[ApiController]
	[Route("[controller]/{packageName?}")]
	public class NugetController : ControllerBase
	{
		private readonly ILogger<NugetController> _logger;

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
			using var archive = await GetArchive(packageName);
			foreach (var entry in archive.Entries)
			{
				if (entry.Name.EndsWith(".dll") && entry.Name.Contains(packageName, StringComparison.OrdinalIgnoreCase))
				{
					await using Stream stream = entry.Open();
					byte[] bytes;
					await using var ms = new MemoryStream();
					await stream.CopyToAsync(ms);
					bytes = ms.ToArray();
					return File(bytes, "application/octet-stream", entry.Name);
				}
			}

			return null;
		}

		private static async Task<ZipArchive> GetArchive(string name)
		{
			using var client = new WebClient();
			var data = await client.DownloadDataTaskAsync(new Uri("https://www.nuget.org/api/v2/package/" + name));
			var archive = new ZipArchive(new MemoryStream(data));
			return archive;
		}
	}
}