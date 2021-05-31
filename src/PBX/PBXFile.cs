using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace PBX
{
    using DictionaryList = List<KeyValuePair<string, dynamic>>;

    public partial class File
    {
        string archiveVersion = "1";
        DictionaryList classes = new DictionaryList();
        string objectVersion = "51";
        // Objects
        List<PBXObject<PBXAggregateTarget>> AggregateTargets = new List<PBXObject<PBXAggregateTarget>>();
        List<PBXObject<PBXBuildFile>> BuildFiles = new List<PBXObject<PBXBuildFile>>();
        List<PBXObject<PBXContainerItemProxy>> ContainerItemProxies = new List<PBXObject<PBXContainerItemProxy>>();
        List<PBXObject<PBXCopyFilesBuildPhase>> CopyFilesBuildPhases = new List<PBXObject<PBXCopyFilesBuildPhase>>();
        List<PBXObject<PBXFileReference>> FileReferences = new List<PBXObject<PBXFileReference>>();
        List<PBXObject<PBXFrameworksBuildPhase>> FrameworksBuildPhases = new List<PBXObject<PBXFrameworksBuildPhase>>();
        List<PBXObject<PBXGroup>> Groups = new List<PBXObject<PBXGroup>>();
        List<PBXObject<PBXHeadersBuildPhase>> HeadersBuildPhases = new List<PBXObject<PBXHeadersBuildPhase>>();
        List<PBXObject<PBXNativeTarget>> NativeTargets = new List<PBXObject<PBXNativeTarget>>();
        List<PBXObject<PBXProject>> Projects = new List<PBXObject<PBXProject>>();
        List<PBXObject<PBXResourcesBuildPhase>> ResourcesBuildPhases = new List<PBXObject<PBXResourcesBuildPhase>>();
        List<PBXObject<PBXShellScriptBuildPhase>> ShellScriptBuildPhases = new List<PBXObject<PBXShellScriptBuildPhase>>();
        List<PBXObject<PBXSourcesBuildPhase>> SourcesBuildPhases = new List<PBXObject<PBXSourcesBuildPhase>>();
        List<PBXObject<PBXTargetDependency>> TargetDependencies = new List<PBXObject<PBXTargetDependency>>();
        List<PBXObject<XCBuildConfiguration>> BuildConfigurations = new List<PBXObject<XCBuildConfiguration>>();
        List<PBXObject<XCConfigurationList>> ConfigurationLists = new List<PBXObject<XCConfigurationList>>();
        string rootObject;
        string sourceRoot;

        // Internal
        Dictionary<string, dynamic> ObjectMap = new Dictionary<string, dynamic>();

        public File(string file)
        {
            try
            {
                load(file);

                var projectFilePath = Path.GetDirectoryName(Path.GetFullPath(file));
                sourceRoot = Path.GetFullPath(Path.Combine(projectFilePath, ".."));
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        static string ByteToHexBitFiddle(byte[] bytes)
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

        public static string generateUUID()
        {
            var prng = new RNGCryptoServiceProvider();

            var data = new byte[16];
            prng.GetBytes(data);

            // RFC 4122, section 4.4 requirement.
            data[6] = (byte)(0x40 | ((int)data[6] & 0xf));
            data[8] = (byte)(0x80 | ((int)data[8] & 0x3f));

            return ByteToHexBitFiddle(data).Substring(0, 24);
        }

        public string getSourceRoot()
        {
            return sourceRoot;
        }

        public List<PBXObject<T>> getObjectList<T>()
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

        public void removeObjectAndDependencies(dynamic obj)
        {
            if (obj is PBXObject<PBXFileReference>)
            {
                var fileRef = obj as PBXObject<PBXFileReference>;

                foreach (var buildFile in BuildFiles)
                {
                    if (buildFile.Value.fileRef == fileRef.Key)
                    {
                        removeObjectAndDependencies(buildFile);
                        break;
                    }
                }

                FileReferences.Remove(fileRef);
            }
            else if (obj is PBXObject<PBXBuildFile>)
            {
                var buildFile = obj as PBXObject<PBXBuildFile>;

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
            else if (obj is PBXObject<PBXGroup>)
            {
                var group = obj as PBXObject<PBXGroup>;

                // NOTE: Unsure if groups can have other references other than other groups.
                foreach (var group2 in Groups)
                {
                    group2.Value.children.Remove(group.Key);
                }

                Groups.Remove(obj);
            }
        }

        public PBXObject<PBXFileReference> addFileReference()
        {
            var fileObj = new PBXObject<PBXFileReference>();
            fileObj.Key = generateUUID();
            fileObj.Value = new PBXFileReference();

            FileReferences.Add(fileObj);
            ObjectMap.Add(fileObj.Key, fileObj);

            return fileObj;
        }

        public PBXObject<PBXGroup> addGroup()
        {
            var groupObj = new PBXObject<PBXGroup>();
            groupObj.Key = generateUUID();
            groupObj.Value = new PBXGroup();

            Groups.Add(groupObj);
            ObjectMap.Add(groupObj.Key, groupObj);

            return groupObj;
        }

        public PBXObject<PBXBuildFile> addBuildFile(PBXObject<PBXFileReference> file)
        {
            var res = new PBXObject<PBXBuildFile>();
            res.Key = generateUUID();
            res.Value = new PBXBuildFile();

            res.Value.fileRef = file.Key;
            BuildFiles.Add(res);
            ObjectMap.Add(res.Key, res);

            return res;
        }

        public PBXFileReference findFile(string id)
        {
            dynamic obj = null;
            if (ObjectMap.TryGetValue(id, out obj))
            {
                if (obj is PBXObject<PBXFileReference>)
                {
                    return (obj as PBXObject<PBXFileReference>).Value;
                }
            }
            return null;
        }

        public List<PBXObject<PBXGroup>> getGroups()
        {
            return Groups;
        }

        public dynamic getRoot()
        {
            return findObject(rootObject);
        }

        public PBXObject<T> findObject<T>(dynamic objectId)
        {
            if (!(objectId is string))
                return null;
            dynamic obj = null;
            if (ObjectMap.TryGetValue(objectId as string, out obj))
            {
                if (obj is PBXObject<T>)
                    return obj as PBXObject<T>;
            }
            return null;
        }

        public dynamic findObject(dynamic objectId)
        {
            if (!(objectId is string))
                return null;
            dynamic obj = null;
            if (ObjectMap.TryGetValue(objectId as string, out obj))
                return obj;
            return null;
        }

        dynamic findBuildConfigurationRef(string id)
        {
            foreach (var obj in AggregateTargets)
            {
                if (obj.Value.buildConfigurationList == id)
                    return obj;
            }
            foreach (var obj in NativeTargets)
            {
                if (obj.Value.buildConfigurationList == id)
                    return obj;
            }
            foreach (var obj in Projects)
            {
                if (obj.Value.buildConfigurationList == id)
                    return obj;
            }
            return null;
        }

    }
}
