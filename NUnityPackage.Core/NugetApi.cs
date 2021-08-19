using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NUnityPackage.Core
{
	public class NugetApi : IDisposable
	{
		public async Task<NugetSpecification> GetSpecification(string packageName)
		{
			using var archive = await GetArchive(packageName);
			foreach (var entry in archive.Entries)
			{
				if (entry.Name.EndsWith(".nuspec"))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(NugetSpecification));
					await using var stream = entry.Open();
					var spec = (NugetSpecification)serializer.Deserialize(stream);
					
					// var reader = new StreamReader(entry.Open());
					// var text = await reader.ReadToEndAsync();
					return spec;
				}
			}

			return null;
		}
		
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

		private async Task<ZipArchive> GetArchive(string packageName, string packageVersion = null)
		{
			using var client = new WebClient();
			var url = "https://www.nuget.org/api/v2/package/" + packageName;
			if (packageVersion != null)
				url += "/" + packageVersion;
			var data = await client.DownloadDataTaskAsync(new Uri(url));
			var archive = new ZipArchive(new MemoryStream(data));
			return archive;
		}

		public void Dispose()
		{
		}
	}
}