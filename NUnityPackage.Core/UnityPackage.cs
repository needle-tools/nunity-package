using System;

namespace NUnityPackage.Core
{
	[Serializable]
	public class UnityPackage
	{
		public string name;
		public string version;
		public string displayName;
		public string description;
		public string author;
		public string changelog;
		public string documentationUrl;
		public string license;
		public string licensesUrl;
	}
}