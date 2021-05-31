using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PBX
{
    using DictionaryList = List<KeyValuePair<string, dynamic>>;

    public partial class File
    {
        string filePath = null;

        public string FilePath { get { return filePath; } }

        void load(string file)
        {
            try
            {
                using (var stream = new StreamReader(file))
                {
                    Reader sr = new Reader(stream);
                    var data = parse(sr);
                    import(data);
                }
                filePath = file;
            }
            catch (Exception)
            {
                throw;
            }
        }
        dynamic getValue(DictionaryList src, string key)
        {
            foreach (var pair in src)
            {
                if (pair.Key == key)
                    return pair.Value;
            }
            return null;
        }

        void import(DictionaryList data)
        {
            archiveVersion = getValue(data, "archiveVersion");
            classes = getValue(data, "classes");
            objectVersion = getValue(data, "objectVersion");
            rootObject = getValue(data, "rootObject");

            var objects = getValue(data, "objects");
            foreach (var entry in objects)
            {
                var key = entry.Key;
                var val = entry.Value;
                var isa = getValue(val, "isa") as string;
                switch (isa)
                {
                    case "PBXAggregateTarget":
                        {
                            var obj = importObject<PBXAggregateTarget>(key, val);
                            AggregateTargets.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXBuildFile":
                        {
                            var obj = importObject<PBXBuildFile>(key, val);
                            BuildFiles.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXContainerItemProxy":
                        {
                            var obj = importObject<PBXContainerItemProxy>(key, val);
                            ContainerItemProxies.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXCopyFilesBuildPhase":
                        {
                            var obj = importObject<PBXCopyFilesBuildPhase>(key, val);
                            CopyFilesBuildPhases.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXFileReference":
                        {
                            var obj = importObject<PBXFileReference>(key, val);
                            FileReferences.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXFrameworksBuildPhase":
                        {
                            var obj = importObject<PBXFrameworksBuildPhase>(key, val);
                            FrameworksBuildPhases.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXGroup":
                        {
                            var obj = importObject<PBXGroup>(key, val);
                            Groups.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXHeadersBuildPhase":
                        {
                            var obj = importObject<PBXHeadersBuildPhase>(key, val);
                            HeadersBuildPhases.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXNativeTarget":
                        {
                            var obj = importObject<PBXNativeTarget>(key, val);
                            NativeTargets.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXProject":
                        {
                            var obj = importObject<PBXProject>(key, val);
                            Projects.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXResourcesBuildPhase":
                        {
                            var obj = importObject<PBXResourcesBuildPhase>(key, val);
                            ResourcesBuildPhases.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXShellScriptBuildPhase":
                        {
                            var obj = importObject<PBXShellScriptBuildPhase>(key, val);
                            ShellScriptBuildPhases.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXSourcesBuildPhase":
                        {
                            var obj = importObject<PBXSourcesBuildPhase>(key, val);
                            SourcesBuildPhases.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXTargetDependency":
                        {
                            var obj = importObject<PBXTargetDependency>(key, val);
                            TargetDependencies.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "XCBuildConfiguration":
                        {
                            var obj = importObject<XCBuildConfiguration>(key, val);
                            BuildConfigurations.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "XCConfigurationList":
                        {
                            var obj = importObject<XCConfigurationList>(key, val);
                            ConfigurationLists.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    default:
                        throw new System.Exception("Unsupported ISA type: " + isa);
                }
            }
        }

        void importFields<T>(ref T dst, DictionaryList src)
        {
            PropertyInfo[] members = dst.GetType().GetProperties();
            foreach (var field in members)
            {
                var data = getValue(src, field.Name) as object;

                var prop = dst.GetType().GetProperty(field.Name);
                prop.SetValue(dst, data, null);
            }
        }

        PBXObject<T> importObject<T>(string key, DictionaryList src)
        {
            var res = new PBXObject<T>();
            res.Key = key;
            res.Value = System.Activator.CreateInstance<T>();
            importFields(ref res.Value, src);
            return res;
        }

    }
}
