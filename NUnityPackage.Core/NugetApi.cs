using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace NUnityPackage.Core
{
	public class NugetApi : IDisposable
	{
		public async Task<byte[]> GetDll(string packageName)
		{
			using var archive = await GetArchive(packageName);
			foreach (var entry in archive.Entries)
			{
				if (entry.Name.EndsWith(".dll") && entry.Name.Contains(packageName, StringComparison.OrdinalIgnoreCase))
				{
					await using var stream = entry.Open();
					await using var ms = new MemoryStream();
					await stream.CopyToAsync(ms);
					var bytes = ms.ToArray();
					return bytes;
				}
			}

			return null;
		}

		private async Task<ZipArchive> GetArchive(string name)
		{
			using var client = new WebClient();
			var data = await client.DownloadDataTaskAsync(new Uri("https://www.nuget.org/api/v2/package/" + name));
			var archive = new ZipArchive(new MemoryStream(data));
			return archive;
		}

		public void Dispose()
		{
		}
	}
}