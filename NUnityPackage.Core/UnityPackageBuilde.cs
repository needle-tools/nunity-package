using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;

namespace NUnityPackage.Core
{
	// https://weblog.west-wind.com/posts/2007/jun/29/httpwebrequest-and-gzip-http-responses
	// https://www.radenkozec.com/asp-net-web-api-gzip-compression-actionfilter/
	
	// https://www.carlrippon.com/zipping-up-files-from-a-memorystream/
	public static class UnityPackageBuilder
	{
		private static readonly byte[] writeBuffer = new byte[32 * 1024];
		
		public static async Task<Stream> Package()
		{
			var compressedStream = new MemoryStream();
			var gzipStream = new GZipStream(compressedStream, CompressionLevel.Optimal);
			await using var archive = new TarOutputStream(gzipStream, Encoding.Default);
			
			await WriteFile(archive, "package.json", "{}");
			
			// var path = "package.json";
			// var file = File.Create(path);
			//
			// // file.
			// // await file.WriteAsync();
			// // var content = Encoding.UTF8.GetBytes("TEST");
			// var entry = TarEntry.CreateTarEntry("test.txt");
			// entry.Size = file.Length;
			// entry.TarHeader.Name = "MyFile";
			//
			// archive.PutNextEntry(entry);
			// StreamUtils.Copy(file, archive, writeBuffer);
			// archive.CloseEntry();
			//
			
			archive.Finish();
			archive.IsStreamOwner = false;
			archive.Close();
			archive.Flush();
			
			// var entry = TarEntry.CreateTarEntry("test");
			// // TarEntry.NameTarHeader(entry.TarHeader, "TEST");
			// // entry.TarHeader.Version = "0";
			// // entry.TarHeader.LinkName
			// tar.WriteEntry(entry, false);
			// var bytes = Encoding.UTF8.GetBytes("Test");
			// var compressedStream = new MemoryStream();
			// // await compressedStream.WriteAsync(bytes);
			// var zipStream = new GZipStream(compressedStream, CompressionMode.Compress, true);
			// await zipStream.WriteAsync(bytes, 0, bytes.Length);
			
			compressedStream.Position = 0;
			// gzipStream.Close();
			return compressedStream;
		}

		private static async Task WriteFile(TarOutputStream archive, string path, string content)
		{
			var file = File.Create(path);
			await using var sr = new StreamWriter(file) { AutoFlush = true };
			await sr.WriteAsync(content);
			
			var entry = TarEntry.CreateTarEntry(path);
			entry.Size = file.Length;
			Console.WriteLine(entry.Size);
			
			archive.PutNextEntry(entry);
			file.Position = 0;
			StreamUtils.Copy(file, archive, writeBuffer);
			archive.CloseEntry();

			sr.Close();
			File.Delete(path);
		}
	}
}