using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json;

namespace NUnityPackage.Core
{
	// https://weblog.west-wind.com/posts/2007/jun/29/httpwebrequest-and-gzip-http-responses
	// https://www.radenkozec.com/asp-net-web-api-gzip-compression-actionfilter/
	
	// https://www.carlrippon.com/zipping-up-files-from-a-memorystream/
	public static class UnityPackageBuilder
	{
		private static readonly byte[] writeBuffer = new byte[32 * 1024];
		
		public static async Task<byte[]> Package(MemoryStream dllStream)
		{
			var zipStream = File.Create("package.tgz");
			var gzipStream = new GZipStream(zipStream, CompressionLevel.Optimal);
			await using var archive = new TarOutputStream(gzipStream, Encoding.Default);
			
			var package = new UnityPackage();
			package.name = "com.needle.test";
			package.version = "0.0.1";
			
			var dir = Directory.CreateDirectory("package");
			await WriteFile(archive, "package/package.json", JsonConvert.SerializeObject(package));
			
			var entry = TarEntry.CreateTarEntry("package/plugin.dll");
			entry.Size = dllStream.Length;
			archive.PutNextEntry(entry);
			dllStream.Position = 0;
			StreamUtils.Copy(dllStream, archive, writeBuffer);
			archive.CloseEntry();
			
			await WriteFile(archive, "package/plugin.dll.meta", UnityMetaHelper.GetMeta("package/plugin.dll"));
			
			
			
			dir.Delete();

			archive.IsStreamOwner = true;
			archive.Close();
			
			var bytes = await File.ReadAllBytesAsync("package.tgz");
			File.Delete("package.tgz");
			return bytes;
		}

		private static async Task WriteFile(TarOutputStream archive, string path, string content)
		{
			var file = File.Create(path);
			await using var sr = new StreamWriter(file) { AutoFlush = true };
			await sr.WriteAsync(content);
			
			var entry = TarEntry.CreateTarEntry(path);
			entry.Size = file.Length;
			archive.PutNextEntry(entry);
			file.Position = 0;
			StreamUtils.Copy(file, archive, writeBuffer);
			archive.CloseEntry();

			sr.Close();
			File.Delete(path);
		}
	}
}