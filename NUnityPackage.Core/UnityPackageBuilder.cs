using System;
using System.Collections.Generic;
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
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace NUnityPackage.Core
{
	// https://weblog.west-wind.com/posts/2007/jun/29/httpwebrequest-and-gzip-http-responses
	// https://www.radenkozec.com/asp-net-web-api-gzip-compression-actionfilter/

	// https://www.carlrippon.com/zipping-up-files-from-a-memorystream/
	public static class UnityPackageBuilder
	{
		private static readonly byte[] writeBuffer = new byte[32 * 1024];

		public static async Task<byte[]> BuildTgzPackage(string packageName, string packageVersion, string packageId, Caching cache, ILogger logger = null)
		{
			using var package = new NugetPackage(packageName, packageVersion, logger);
			var spec = await package.GetSpecification();
			if (spec == null)
			{
				logger.LogError("Failed getting specification for " + packageId);
				return null;
			}

			var dllStream = await package.GetDllStream(logger);

			if (dllStream == null)
			{
				return null;
			}

			var meta = spec.metadata;
			var unityPackage = new UnityPackage();
			unityPackage.name = spec.ToUnityPackageName(packageName);
			unityPackage.version = meta.version;
			unityPackage.displayName = meta.title ?? packageName;
			unityPackage.description = meta.description;
			unityPackage.author = meta.authors;
			unityPackage.changelog = meta.releaseNotes;
			unityPackage.license = meta.license;
			unityPackage.licensesUrl = meta.licenseUrl;
			unityPackage.documentationUrl = meta.projectUrl;

			logger?.LogInformation("Downloading " + packageName + " as " + packageId);
			var p = await BuildPackageTgz(unityPackage, packageName + ".dll", dllStream, cache, packageId);
			logger?.LogInformation("Return file: " + packageId + ", " + p.Length + " bytes");
			return p;
		}

		private static async Task<byte[]> BuildPackageTgz(UnityPackage package, string dllName, MemoryStream dllStream, Caching cache, string cacheName)
		{
			var packageName = package.name + "-" + package.version + ".tgz";

			var tempDir = "temp/" + packageName + "-" + DateTime.UtcNow.ToFileTime();
			if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);
			var localPackagePath = tempDir + "/" + packageName;

			var zipStream = File.Create(localPackagePath);
			var gzipStream = new GZipStream(zipStream, CompressionLevel.Optimal);
			await using var archive = new TarOutputStream(gzipStream, Encoding.Default);

			var dir = Directory.CreateDirectory(tempDir + "/package");
			var jsonPath = tempDir + "/package/package.json";
			var json = JsonConvert.SerializeObject(package, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
			await WriteFile(archive, jsonPath, json);
			await WriteFile(archive, jsonPath + ".meta", UnityMetaHelper.GetMeta("305c24821ff995c408403969a18e2c79"));

			var dllPath = tempDir + "/package/" + dllName + ".dll";
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

			var bytes = await File.ReadAllBytesAsync(localPackagePath);

			if (cache != null)
			{
				cache.UploadFile(localPackagePath, cacheName);
				Shasum.CreateAndUpload(bytes, cacheName, cache);
			}

			if (File.Exists(localPackagePath))
				File.Delete(localPackagePath);
			if (Directory.Exists(tempDir)) Directory.Delete(tempDir);
			return bytes;
		}

		public static string GetHash(byte[] bytes)
		{
			using var algo = SHA1.Create();
			var data = algo.ComputeHash(bytes);
			var sBuilder = new StringBuilder();
			foreach (var t in data)
			{
				sBuilder.Append(t.ToString("x2"));
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