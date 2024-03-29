﻿using System;
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
			return GetHash(bytes, new SHA1Managed(), true);
		}

		public static string GetHash(byte[] bytes, HashAlgorithm algo = null, bool isOwned = true)
		{
			try
			{
				algo ??= SHA1.Create();
				var data = algo.ComputeHash(bytes);
				return BitConverter.ToString(data).Replace("-", string.Empty).ToLower();
			}
			finally
			{
				if(isOwned) 
					algo?.Dispose();
			}
		}
	}
}