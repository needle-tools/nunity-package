using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NUnityPackage.Core
{
	public class NugetPackage : IDisposable
	{
		public readonly string name;
		public readonly string version;
		private ZipArchive _archive;

		public NugetPackage(string name, string version = null)
		{
			this.name = name;
			this.version = version;
		}
		
		public async Task<NugetSpecification> GetSpecification()
		{
			var archive = await GetArchive();
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
		
		
		public async Task<MemoryStream> GetDllStream()
		{
			var archive = await GetArchive();
			foreach (var entry in archive.Entries)
			{
				if (entry.Name.EndsWith(".dll") && entry.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
				{
					await using var stream = entry.Open();
					var ms = new MemoryStream();
					await stream.CopyToAsync(ms);
					return ms;
				}
			}

			return null;
		}
		
		public async Task<byte[]> GetDll()
		{
			var stream = await GetDllStream();
			if (stream != null)
			{
				await using (stream)
				{
					var bytes = stream.ToArray();
					return bytes;
				}
			}

			return null;
		}

		private async Task<ZipArchive> GetArchive()
		{
			if (_archive != null) return _archive;
			using var client = new WebClient();
			var url = "https://www.nuget.org/api/v2/package/" + name;
			if (version != null)
				url += "/" + version;
			var data = await client.DownloadDataTaskAsync(new Uri(url));
			_archive = new ZipArchive(new MemoryStream(data));
			return _archive;
		}

		public void Dispose()
		{
			_archive?.Dispose();
		}
	}
}