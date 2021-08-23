using System.Collections.Generic;

namespace NUnityPackage.Core.Interfaces
{
	public interface IHaveUnityDependencies
	{
		public Dictionary<string, string> dependencies { get; set; }
	}
}