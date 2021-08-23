using System;

namespace NUnityPackage.Core
{
	public static class UnityAllowedDependencies
	{
		public static bool IsAllowed(string name, string version)
		{
			if (name.Equals("system.runtime", StringComparison.OrdinalIgnoreCase)) return false;
			return true;
		}
	}
}