using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
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

		public static async Task<byte[]> Package(UnityPackage package, string dllName, MemoryStream dllStream)
		{
			var packageName = package.name + "-" + package.version + ".tgz";
			var packagePath = packageName;
			if(File.Exists(packagePath))
				return await File.ReadAllBytesAsync(packageName);
			
			var zipStream = File.Create(packageName);
			var gzipStream = new GZipStream(zipStream, CompressionLevel.Optimal);
			await using var archive = new TarOutputStream(gzipStream, Encoding.Default);

			var dir = Directory.CreateDirectory("package");
			var jsonPath = "package/package.json";
			var json = JsonConvert.SerializeObject(package, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
			await WriteFile(archive, jsonPath, json);
			await WriteFile(archive, jsonPath + ".meta", UnityMetaHelper.GetMeta("305c24821ff995c408403969a18e2c79"));

			var dllPath = "package/" + dllName + ".dll";
			var entry = TarEntry.CreateTarEntry(dllPath);
			entry.Size = dllStream.Length;
			archive.PutNextEntry(entry);
			dllStream.Position = 0;
			StreamUtils.Copy(dllStream, archive, writeBuffer);
			archive.CloseEntry();

			await WriteFile(archive, dllPath + ".meta", UnityMetaHelper.GetMeta());

			dir.Delete(true);

			archive.IsStreamOwner = true;
			archive.Close();

			var bytes = await File.ReadAllBytesAsync(packageName);
			// File.Delete(packageName);
			return bytes;
		}

		public static string GetHash(byte[] bytes)
		{
			using var algo = SHA1.Create();
			var data = algo.ComputeHash(bytes);
			var sBuilder = new StringBuilder();
			for (var i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}
			return sBuilder.ToString();
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