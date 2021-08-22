using System.Security.Cryptography;
using System.Text;

namespace NUnityPackage.Core
{
	public class HashUtils
	{
		public static string GetMd5Hash(byte[] bytes)
		{
			return GetHash(bytes, MD5.Create(), true);
		}
		
		public static string GetSha1Hash(byte[] bytes)
		{
			return GetHash(bytes, SHA1.Create(), true);
		}

		public static string GetHash(byte[] bytes, HashAlgorithm algo = null, bool isOwned = true)
		{
			try
			{
				algo ??= SHA1.Create();
				
				var data = algo.ComputeHash(bytes);
				var sBuilder = new StringBuilder();
				foreach (var t in data)
				{
					sBuilder.Append(t.ToString("x2"));
				}

				if (isOwned) algo.Dispose();
				return sBuilder.ToString();
			}
			finally
			{
				if(isOwned) 
					algo?.Dispose();
			}
		}
	}
}