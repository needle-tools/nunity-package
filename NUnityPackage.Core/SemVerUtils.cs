// using System.Text.RegularExpressions;
//
// namespace NUnityPackage.Core
// {
// 	public static class SemVerUtils
// 	{
// 		// https://regex101.com/r/3WMjS5/5
// 		// private static readonly Regex NugetRegex = new Regex(@"\d+\.\d+\.\d+(?<additional>-[a-zA-Z]+)?(?<dot>\.)?(?<patch>\d+)?", RegexOptions.Compiled);
// 		// public static string SanitizeSemver(this string version)
// 		// {
// 		// 	var match = NugetRegex.Match(version);
// 		// 	if (match.Success)
// 		// 	{
// 		// 		var additional = match.Groups["additional"];
// 		// 		if (additional.Success)
// 		// 		{
// 		// 			var sanitized = version.Substring(additional.Index);
// 		// 		}
// 		// 	}
// 		//
// 		// 	return version;
// 		// }
// 	}
// }