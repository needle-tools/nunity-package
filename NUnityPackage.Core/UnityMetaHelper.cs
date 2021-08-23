﻿namespace NUnityPackage.Core
{
	public class UnityMetaHelper
	{
    public static string GetMeta(string guid)
    {
      return pluginDllMetaTemplate.Replace("<guid>", guid);
    }
    
		public const string pluginDllMetaTemplate = @"fileFormatVersion: 2
guid: <guid>
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