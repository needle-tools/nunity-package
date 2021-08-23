using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NUnityPackage.Core
{
	// https://api.nuget.org/v3/registration5-semver1/system.drawing.common/index.json
	// https://api.nuget.org/v3/registration5-semver1/system.diagnostics.diagnosticsource/index.json
	public class NugetRegistrationResult<T>
	{
		public string commitId;
		public string commitTimeStamp;
		public int count;
		public T[] items;
	}

	public class NugetPackageRegistrationResult : NugetRegistrationResult<NugetCatalogPageItem>
	{
	}
	
	public class NugetCatalogPageItem : NugetRegistrationResult<NugetPackageItem>
	{
		// e.g. https://api.nuget.org/v3/registration5-semver1/serilog/index.json
		[JsonProperty("@id")]
		public string id;
		public string lower;
		public string upper;
	}
	
	public class NugetPackageItem
	{
		public string commitId;
		public string commitTimeStamp;
		public PackageDetails catalogEntry;
		public string packageContent;
		public string registration;
		
		public class PackageDetails
		{
			public string authors;
			public string description;
			public string iconUrl;
			public string id;
			public string language;
			public string licenseExpression;
			public string licenseUrl;
			public bool listed;
			public string packageContent;
			public string projectUrl;
			public DateTime published;
			public bool requireLicenseAcceptance;
			public string summary;
			public string[] tags;
			public string title;
			public string version;
			public TargetFramework[] dependencyGroups;
		}
	}


}