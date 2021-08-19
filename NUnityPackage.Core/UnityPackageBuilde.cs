using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace NUnityPackage.Core
{
	// https://weblog.west-wind.com/posts/2007/jun/29/httpwebrequest-and-gzip-http-responses
	// https://www.radenkozec.com/asp-net-web-api-gzip-compression-actionfilter/
	
	// https://www.carlrippon.com/zipping-up-files-from-a-memorystream/
	public static class UnityPackageBuilder
	{
		public static async Task<Stream> Package()
		{
			var test = "HelloWorld";
			var bytes = Encoding.UTF8.GetBytes(test);
			var compressedStream = new MemoryStream();
			// await compressedStream.WriteAsync(bytes);
			var zipStream = new GZipStream(compressedStream, CompressionMode.Compress);
			await zipStream.WriteAsync(bytes, 0, bytes.Length);
			return compressedStream;
		}
	}
}