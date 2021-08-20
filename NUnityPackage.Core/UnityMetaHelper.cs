namespace NUnityPackage.Core
{
	public class UnityMetaHelper
	{
    public static string GetMeta(string filePath)
    {
      return pluginDllMetaTemplate;
    }
    
		public const string pluginDllMetaTemplate = @"fileFormatVersion: 2
guid: 305c24821ff995c408403969a18e2c77
PluginImporter:
  externalObjects: {}
  serializedVersion: 2
  iconMap: {}
  executionOrder: {}
  defineConstraints: []
  isPreloaded: 0
  isOverridable: 1
  isExplicitlyReferenced: 0
  validateReferences: 1
  platformData:
  - first:
      : 
    second:
      enabled: 0
      settings: {}
  - first:
      Any: 
    second:
      enabled: 1
      settings: {}
  - first:
      Editor: Editor
    second:
      enabled: 0
      settings:
        DefaultValueInitialized: true
  - first:
      Windows Store Apps: WindowsStoreApps
    second:
      enabled: 0
      settings:
        CPU: AnyCPU
  userData: 
  assetBundleName: 
  assetBundleVariant: 
";
	}
}