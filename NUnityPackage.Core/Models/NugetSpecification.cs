using System.Xml.Serialization;

namespace NUnityPackage.Core
{
	[XmlRoot("package")]//, Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd")]
	public class NugetSpecification
	{
		public Metadata metadata;
	}
	
	// [XmlRoot("package")]
	// public class NugetSpecificationW3 : NugetSpecification
	// {
	// }

	// [XmlType("metadata", Namespace = "http://schemas.microsoft.com/packaging/2013/05/nuspec.xsd")]
	public class Metadata
	{
		public string id;
		public string version;
		public string title;
		public string authors;
		public string owners;
		public string requireLicenseAcceptance;
		public string license;
		public string licenseUrl;
		public string projectUrl;
		public string iconUrl;
		public string description;
		public string releaseNotes;
		public string copyright;
		public string serviceable;
		public Repository repository;
		public TargetFramework[] dependencies;
		public FrameworkAssembly[] frameworkAssemblies;
	}

	[XmlType("group")]
	public class TargetFramework
	{
		[XmlAttribute]
		public string targetFramework;
		[XmlElement("dependency")]
		public Dependency[] dependency;
	}

	public class Dependency
	{
		[XmlAttribute]
		public string id;

		[XmlAttribute]
		public string version;
	}

	[XmlType("frameworkAssembly")]
	public class FrameworkAssembly
	{
		[XmlAttribute]
		public string assemblyName;
		[XmlAttribute]
		public string targetFramework;
	}

	public class Repository
	{
		[XmlAttribute]
		public string type;
		[XmlAttribute]
		public string url;
		[XmlAttribute]
		public string commit;
	}
}