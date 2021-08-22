using System;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using NUnityPackage.Core;

namespace NUnityPackage
{
	public static class Globals
	{
		private static Caching cache;
		public static Caching Cache => cache ??= new Caching();

		public static bool IsDevelopmentEnvironment =>
			string.Equals(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"), "development", StringComparison.CurrentCultureIgnoreCase);
	}
}