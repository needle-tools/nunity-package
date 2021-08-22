using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NUnityPackage.Core
{
	// https://api-v2v3search-0.nuget.org/query?q=packageid:system.drawing.common
	[Serializable]
	public class NugetSearchResult
	{
		public int totalHits;
		public Entry[] data;
		
		[Serializable]
		public class Entry
		{
			public string registration;
			public string id;
			public string version;
			public string description;
			public string summary;
			public string title;
			public string iconUrl;
			public string licenseUrl;
			public string projectUrl;
			public string[] tags;
			public string[] authors;
			public string[] owners;
			public int totalDownloads;
			public bool verified;
			public object[] packageTypes;
			public Version[] versions;

			[Serializable]
			public class Version
			{
				public string version;
				public int downloads;
				[JsonProperty("@id")]
				public string id;
			}
		}
	}

}