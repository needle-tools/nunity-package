using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Logging;
using Newtonsoft.Json;

namespace NUnityPackage.Core
{
	[Serializable]
	public class Shasum
	{
		public string shasum;
		
		// for serialization
		public Shasum(){}

		public Shasum(byte[] data)
		{
			shasum = UnityPackageBuilder.GetHash(data);
		}

		public Shasum(string shasum)
		{
			this.shasum = shasum;
		}

		public const string Extension = ".shasum";

		public static string CreateFileContent(byte[] data)
		{
			var instance = new Shasum(data);
			return JsonConvert.SerializeObject(instance, Formatting.Indented);
		}

		public static void CreateAndUpload(byte[] data, string name, Caching cache)
		{
			var json = CreateFileContent(data);
			cache.UploadFile(new MemoryStream(Encoding.UTF8.GetBytes(json)), name + Extension);
		}

		public static async Task<Shasum> TryGet(string name, Caching cache, ILogger logger = null)
		{
			try
			{
				var res = await cache.TryDownloadFile(name + Extension);
				var json = Encoding.UTF8.GetString(res);
				return JsonConvert.DeserializeObject<Shasum>(json);
			}
			catch (Exception e)
			{
				logger?.Error(e.Message);
			}

			return null;
		}
	}
}