using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Logging;
using Google.Cloud.Storage.V1;

namespace NUnityPackage.Core
{
	public class Caching
	{
		private readonly string defaultBucketName;
		private readonly StorageClient client;
		
		public Caching(string defaultBucketName = "needle-nuget.appspot.com")
		{
			this.defaultBucketName = defaultBucketName;
			/*
			 * Cloud Storage access:
				1) Generate a access key here: https://console.cloud.google.com/iam-admin/serviceaccounts/details/100019773737101810166;edit=true/keys?folder=&organizationId=&project=needle-nuget (IAM & Admin / AppEngine default service account / Keys)
				2) Place the key file as ``development-gcloud-credentials.json`` at the root of this project (next to .sln file)
				3) Done, you should now be able to access cloud storage objects locally
			 */
			this.client = StorageClient.Create();
		}
		
		public void UploadFile(Stream stream ,string objectName, string bucketName = null)
		{
			bucketName ??= defaultBucketName;
			client.UploadObject(bucketName, objectName, null, stream);
		}

		public void UploadFile(string localPath ,string objectName, string bucketName = null)
		{
			using var fileStream = File.OpenRead(localPath);
			bucketName ??= defaultBucketName;
			client.UploadObject(bucketName, objectName, null, fileStream);
		}

		public async Task DownloadFile(Stream destination, string objectName, string bucketName = null)
		{
			bucketName ??= defaultBucketName;
			await client.DownloadObjectAsync(bucketName, objectName, destination);
		}

		public async Task<byte[]> DownloadFile(string objectName, string bucketName = null)
		{
			bucketName ??= defaultBucketName;
			await using var memoryStream = new MemoryStream();
			await client.DownloadObjectAsync(bucketName, objectName, memoryStream);
			var res = memoryStream.ToArray();
			return res;
		}

		public async Task<byte[]> TryDownloadFile(string objectName, string bucketName = null, ILogger logger = null)
		{
			try
			{
				bucketName ??= defaultBucketName;
				await using var memoryStream = new MemoryStream();
				await client.DownloadObjectAsync(bucketName, objectName, memoryStream);
				var res = memoryStream.ToArray();
				return res;
			}
			catch (GoogleApiException e)
			{
				logger?.Error(e.Message);
				return null;
			}
		}
		
		public IEnumerable<Google.Apis.Storage.v1.Data.Object> ListFiles(string bucketName = null)
		{
			bucketName ??= defaultBucketName;
			var storageObjects = client.ListObjects(bucketName);
			foreach (var storageObject in storageObjects)
			{
				yield return storageObject;
			}
		}

		public async Task ClearCachedFiles(string bucketName = null, ILogger logger= null)
		{
			bucketName ??= defaultBucketName;
			foreach (var file in ListFiles(bucketName))
			{
				logger?.Info("Delete " + file.Name + " in " + file.Bucket);
				await client.DeleteObjectAsync(file.Bucket, file.Name);
			}
		}
	}
}