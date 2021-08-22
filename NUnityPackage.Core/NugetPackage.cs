using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;

namespace NUnityPackage.Core
{
	public class NugetPackage : IDisposable
	{
		public readonly string name;
		public readonly string version;
		private ZipArchive _archive;
		private readonly ILogger logger;

		public NugetPackage(string name, string version, ILogger logger = null)
		{
			this.name = name;
			this.version = version;
			this.logger = logger;
		}
		
		public async Task<NugetSpecification> GetSpecification()
		{
			var archive = await GetArchive();
			foreach (var entry in archive.Entries)
			{
				if (entry.Name.EndsWith(".nuspec"))
				{
					try
					{
						XmlSerializer serializer = new XmlSerializer(typeof(NugetSpecificationNew));
						await using var stream = entry.Open();
						// var content = new MemoryStream();
						// await entry.Open().CopyToAsync(content);
						// var xml = Encoding.UTF8.GetString(content.ToArray());
						var reader = new NamespaceIgnorantXmlTextReader(new StreamReader(stream));
						reader.Namespaces = false;
						var spec = (NugetSpecificationNew)serializer.Deserialize(reader);

						// var reader = new StreamReader(entry.Open());
						// var text = await reader.ReadToEndAsync();
						return spec;
					}
					catch (InvalidOperationException e)
					{
						if (logger != null)
						{
							// var content = new MemoryStream();
							// await entry.Open().CopyToAsync(content);
							// var text = Encoding.UTF8.GetString(content.ToArray());
							logger?.LogError(e.Message); // + "\nXML:\n" + text);
						}
					}
				}
			}

			return null;
		}
		
		
		public async Task<MemoryStream> GetDllStream(ILogger logger = null)
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
			
			// fallback
			foreach (var entry in archive.Entries)
			{
				if (entry.Name.EndsWith(".dll") && name.StartsWith(entry.Name.Substring(0, entry.Name.Length-4).ToLowerInvariant()))
				{
					logger?.LogInformation("Fallback matched for " + name + " version " + version + " with " + entry.FullName);
					await using var stream = entry.Open();
					var ms = new MemoryStream();
					await stream.CopyToAsync(ms);
					return ms;
				}
			}

			logger?.LogWarning("Failed finding dll for " + name + " version " + version);
			
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