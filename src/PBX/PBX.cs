using System.Collections.Generic;

namespace PBX
{
    using DictionaryList = List<KeyValuePair<string, dynamic>>;

    public class PBXAggregateTarget
    {
        public string buildConfigurationList { get; set; }
        public List<dynamic> buildPhases { get; set; }
        public List<dynamic> dependencies { get; set; }
        public string name { get; set; }
        public string productName { get; set; }
    }

    public class PBXBuildFile
    {
        public string fileRef { get; set; }
        public DictionaryList settings { get; set; }
    }
    public class PBXContainerItemProxy
    {
        public string containerPortal { get; set; }
        public string proxyType { get; set; }
        public string remoteGlobalIDString { get; set; }
        public string remoteInfo { get; set; }
    }

    public class PBXCopyFilesBuildPhase
    {
        public string buildActionMask { get; set; }
        public string dstPath { get; set; }
        public string dstSubfolderSpec { get; set; }
        public List<dynamic> files { get; set; }
        public string name { get; set; }
        public string runOnlyForDeploymentPostprocessing { get; set; }
    }

    static class PBXFileEncoding
    {
        public const string UTF8 = "4";
    }

    public class PBXFileReference
    {
        public string explicitFileType { get; set; }
        public string includeInIndex { get; set; }
        // PBXFileEncoding
        public string fileEncoding { get; set; }
        public string lastKnownFileType { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string sourceTree { get; set; }
        public string usesTabs { get; set; }
    }

    public class PBXFrameworksBuildPhase
    {
        public string buildActionMask { get; set; }
        public List<dynamic> files { get; set; }
        public string runOnlyForDeploymentPostprocessing { get; set; }
    }

    public class PBXHeadersBuildPhase
    {
        public string buildActionMask { get; set; }
        public List<dynamic> files { get; set; }
        public string runOnlyForDeploymentPostprocessing { get; set; }
    }

    public class PBXNativeTarget
    {
        public string buildConfigurationList { get; set; }
        public List<dynamic> buildPhases { get; set; }
        public List<dynamic> buildRules { get; set; }
        public List<dynamic> dependencies { get; set; }
        public string name { get; set; }
        public string productName { get; set; }
        public string productReference { get; set; }
        public string productType { get; set; }
    }

    public class PBXProject
    {
        public DictionaryList attributes { get; set; }
        public string buildConfigurationList { get; set; }
        public string compatibilityVersion { get; set; }
        public string developmentRegion { get; set; }
        public string hasScannedForEncodings { get; set; }
        public List<dynamic> knownRegions { get; set; }
        public string mainGroup { get; set; }
        public string productRefGroup { get; set; }
        public string projectDirPath { get; set; }
        public string projectRoot { get; set; }
        public List<dynamic> targets { get; set; }
    }

    public class PBXResourcesBuildPhase
    {
        public string buildActionMask { get; set; }
        public List<dynamic> files { get; set; }
        public string runOnlyForDeploymentPostprocessing { get; set; }
    }

    public class PBXShellScriptBuildPhase
    {
        public string buildActionMask { get; set; }
        public List<dynamic> files { get; set; }
        public List<dynamic> inputFileListPaths { get; set; }
        public List<dynamic> inputPaths { get; set; }
        public string name { get; set; }
        public List<dynamic> outputFileListPaths { get; set; }
        public List<dynamic> outputPaths { get; set; }
        public string runOnlyForDeploymentPostprocessing { get; set; }
        public string shellPath { get; set; }
        public string shellScript { get; set; }
        public string showEnvVarsInLog { get; set; }
    }

    public class PBXGroup
    {
        public List<dynamic> children { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public string sourceTree { get; set; }
        public string usesTabs { get; set; }
    }

    public class PBXSourcesBuildPhase
    {
        public string buildActionMask { get; set; }
        public List<dynamic> files { get; set; }
        public string runOnlyForDeploymentPostprocessing { get; set; }
    }

    public class PBXTargetDependency
    {
        public string target { get; set; }
        public string targetProxy { get; set; }
    }

    public class XCBuildConfiguration
    {
        public List<KeyValuePair<string, dynamic>> buildSettings { get; set; }
        public string name { get; set; }
    }

    public class XCConfigurationList
    {
        public List<dynamic> buildConfigurations { get; set; }
        public string defaultConfigurationIsVisible { get; set; }
        public string defaultConfigurationName { get; set; }
    }

    public class PBXObject<T>
    {
        public string Key;
        public T Value;
    }
}
