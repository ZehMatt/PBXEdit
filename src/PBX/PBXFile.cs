using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace PBX
{
    using DictionaryList = List<KeyValuePair<string, dynamic>>;

    public partial class File
    {
        private string archiveVersion = "1";
        private DictionaryList classes = new DictionaryList();
        private string objectVersion = "51";

        // Objects
        private List<PBXObject<PBXAggregateTarget>> AggregateTargets = new List<PBXObject<PBXAggregateTarget>>();
        private List<PBXObject<PBXBuildFile>> BuildFiles = new List<PBXObject<PBXBuildFile>>();
        private List<PBXObject<PBXContainerItemProxy>> ContainerItemProxies = new List<PBXObject<PBXContainerItemProxy>>();
        private List<PBXObject<PBXCopyFilesBuildPhase>> CopyFilesBuildPhases = new List<PBXObject<PBXCopyFilesBuildPhase>>();
        private List<PBXObject<PBXFileReference>> FileReferences = new List<PBXObject<PBXFileReference>>();
        private List<PBXObject<PBXFrameworksBuildPhase>> FrameworksBuildPhases = new List<PBXObject<PBXFrameworksBuildPhase>>();
        private List<PBXObject<PBXGroup>> Groups = new List<PBXObject<PBXGroup>>();
        private List<PBXObject<PBXHeadersBuildPhase>> HeadersBuildPhases = new List<PBXObject<PBXHeadersBuildPhase>>();
        private List<PBXObject<PBXNativeTarget>> NativeTargets = new List<PBXObject<PBXNativeTarget>>();
        private List<PBXObject<PBXProject>> Projects = new List<PBXObject<PBXProject>>();
        private List<PBXObject<PBXResourcesBuildPhase>> ResourcesBuildPhases = new List<PBXObject<PBXResourcesBuildPhase>>();
        private List<PBXObject<PBXShellScriptBuildPhase>> ShellScriptBuildPhases = new List<PBXObject<PBXShellScriptBuildPhase>>();
        private List<PBXObject<PBXSourcesBuildPhase>> SourcesBuildPhases = new List<PBXObject<PBXSourcesBuildPhase>>();
        private List<PBXObject<PBXTargetDependency>> TargetDependencies = new List<PBXObject<PBXTargetDependency>>();
        private List<PBXObject<XCBuildConfiguration>> BuildConfigurations = new List<PBXObject<XCBuildConfiguration>>();
        private List<PBXObject<XCConfigurationList>> ConfigurationLists = new List<PBXObject<XCConfigurationList>>();
        private string rootObject;
        private string sourceRoot;

        // Internal
        private Dictionary<string, dynamic> ObjectMap = new Dictionary<string, dynamic>();

        public File(string file)
        {
            try
            {
                Load(file);

                var projectFilePath = Path.GetDirectoryName(Path.GetFullPath(file));
                sourceRoot = Path.GetFullPath(Path.Combine(projectFilePath, ".."));
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private static string ByteToHexBitFiddle(byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];
            int b;
            for (int i = 0; i < bytes.Length; i++)
            {
                b = bytes[i] >> 4;
                c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
                b = bytes[i] & 0xF;
                c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
            }
            return new string(c);
        }

        public static string GenerateUUID()
        {
            var prng = new RNGCryptoServiceProvider();

            var data = new byte[16];
            prng.GetBytes(data);

            // RFC 4122, section 4.4 requirement.
            data[6] = (byte)(0x40 | (data[6] & 0xf));
            data[8] = (byte)(0x80 | (data[8] & 0x3f));

            return ByteToHexBitFiddle(data).Substring(0, 24);
        }

        public string GetSourceRoot()
        {
            return sourceRoot;
        }

        public List<PBXObject<T>> GetObjectList<T>()
        {
            switch (typeof(T).Name)
            {
                case "PBXAggregateTarget":
                    return AggregateTargets as List<PBXObject<T>>;
                case "PBXBuildFile":
                    return BuildFiles as List<PBXObject<T>>;
                case "PBXContainerItemProxy":
                    return ContainerItemProxies as List<PBXObject<T>>;
                case "PBXCopyFilesBuildPhase":
                    return CopyFilesBuildPhases as List<PBXObject<T>>;
                case "PBXFileReference":
                    return FileReferences as List<PBXObject<T>>;
                case "PBXFrameworksBuildPhase":
                    return FrameworksBuildPhases as List<PBXObject<T>>;
                case "PBXGroup":
                    return Groups as List<PBXObject<T>>;
                case "PBXHeadersBuildPhase":
                    return HeadersBuildPhases as List<PBXObject<T>>;
                case "PBXNativeTarget":
                    return NativeTargets as List<PBXObject<T>>;
                case "PBXProject":
                    return Projects as List<PBXObject<T>>;
                case "PBXResourcesBuildPhase":
                    return ResourcesBuildPhases as List<PBXObject<T>>;
                case "PBXShellScriptBuildPhase":
                    return ShellScriptBuildPhases as List<PBXObject<T>>;
                case "PBXSourcesBuildPhase":
                    return SourcesBuildPhases as List<PBXObject<T>>;
                case "PBXTargetDependency":
                    return TargetDependencies as List<PBXObject<T>>;
                case "XCBuildConfiguration":
                    return BuildConfigurations as List<PBXObject<T>>;
                case "XCConfigurationList":
                    return ConfigurationLists as List<PBXObject<T>>;
            }
            return null;
        }

        public void RemoveObjectAndDependencies(dynamic obj)
        {
            if (obj is PBXObject<PBXFileReference> fileRef)
            {
                foreach (var buildFile in BuildFiles)
                {
                    if (buildFile.Value.fileRef == fileRef.Key)
                    {
                        RemoveObjectAndDependencies(buildFile);
                        break;
                    }
                }

                FileReferences.Remove(fileRef);
            }
            else if (obj is PBXObject<PBXBuildFile> buildFile)
            {
                // Go over all things that contain references to PBXBuildFile
                foreach (var sourcePhase in SourcesBuildPhases)
                {
                    sourcePhase.Value.files.Remove(buildFile.Key);
                }
                foreach (var headerPhase in HeadersBuildPhases)
                {
                    headerPhase.Value.files.Remove(buildFile.Key);
                }

                BuildFiles.Remove(buildFile);
            }
            else if (obj is PBXObject<PBXGroup> group)
            {
                // NOTE: Unsure if groups can have other references other than other groups.
                foreach (var group2 in Groups)
                {
                    group2.Value.children.Remove(group.Key);
                }

                Groups.Remove(obj);
            }
        }

        public PBXObject<PBXFileReference> AddFileReference()
        {
            var fileObj = new PBXObject<PBXFileReference>
            {
                Key = GenerateUUID(),
                Value = new PBXFileReference()
            };

            FileReferences.Add(fileObj);
            ObjectMap.Add(fileObj.Key, fileObj);

            return fileObj;
        }

        public PBXObject<PBXGroup> AddGroup()
        {
            var groupObj = new PBXObject<PBXGroup>
            {
                Key = GenerateUUID(),
                Value = new PBXGroup()
            };

            Groups.Add(groupObj);
            ObjectMap.Add(groupObj.Key, groupObj);

            return groupObj;
        }

        public PBXObject<PBXBuildFile> AddBuildFile(PBXObject<PBXFileReference> file)
        {
            var res = new PBXObject<PBXBuildFile>
            {
                Key = GenerateUUID(),
                Value = new PBXBuildFile()
            };

            res.Value.fileRef = file.Key;
            BuildFiles.Add(res);
            ObjectMap.Add(res.Key, res);

            return res;
        }

        public PBXFileReference FindFile(string id)
        {
            dynamic obj;
            if (ObjectMap.TryGetValue(id, out obj))
            {
                if (obj is PBXObject<PBXFileReference>)
                {
                    return (obj as PBXObject<PBXFileReference>).Value;
                }
            }
            return null;
        }

        public List<PBXObject<PBXGroup>> GetGroups()
        {
            return Groups;
        }

        public dynamic GetRoot()
        {
            return FindObject(rootObject);
        }

        public PBXObject<T> FindObject<T>(dynamic objectId)
        {
            if (!(objectId is string))
            {
                return null;
            }

            dynamic obj;
            if (ObjectMap.TryGetValue(objectId as string, out obj))
            {
                if (obj is PBXObject<T>)
                {
                    return obj as PBXObject<T>;
                }
            }
            return null;
        }

        public dynamic FindObject(dynamic objectId)
        {
            if (!(objectId is string))
            {
                return null;
            }

            dynamic obj;
            if (ObjectMap.TryGetValue(objectId as string, out obj))
            {
                return obj;
            }

            return null;
        }

        private dynamic FindBuildConfigurationRef(string id)
        {
            foreach (var obj in AggregateTargets)
            {
                if (obj.Value.buildConfigurationList == id)
                {
                    return obj;
                }
            }
            foreach (var obj in NativeTargets)
            {
                if (obj.Value.buildConfigurationList == id)
                {
                    return obj;
                }
            }
            foreach (var obj in Projects)
            {
                if (obj.Value.buildConfigurationList == id)
                {
                    return obj;
                }
            }
            return null;
        }

    }
}
