using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PBX
{
    using DictionaryList = List<KeyValuePair<string, dynamic>>;

    public partial class File
    {
        private string filePath = null;

        public string FilePath { get { return filePath; } }

        private void Load(string file)
        {
            try
            {
                using (var stream = new StreamReader(file))
                {
                    Reader sr = new Reader(stream);
                    var data = Parse(sr);
                    Import(data);
                }
                filePath = file;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private dynamic GetValue(DictionaryList src, string key)
        {
            foreach (var pair in src)
            {
                if (pair.Key == key)
                {
                    return pair.Value;
                }
            }
            return null;
        }

        private void Import(DictionaryList data)
        {
            archiveVersion = GetValue(data, "archiveVersion");
            classes = GetValue(data, "classes");
            objectVersion = GetValue(data, "objectVersion");
            rootObject = GetValue(data, "rootObject");

            var objects = GetValue(data, "objects");
            foreach (var entry in objects)
            {
                var key = entry.Key;
                var val = entry.Value;
                var isa = GetValue(val, "isa") as string;
                switch (isa)
                {
                    case "PBXAggregateTarget":
                        {
                            var obj = ImportObject<PBXAggregateTarget>(key, val);
                            AggregateTargets.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXBuildFile":
                        {
                            var obj = ImportObject<PBXBuildFile>(key, val);
                            BuildFiles.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXContainerItemProxy":
                        {
                            var obj = ImportObject<PBXContainerItemProxy>(key, val);
                            ContainerItemProxies.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXCopyFilesBuildPhase":
                        {
                            var obj = ImportObject<PBXCopyFilesBuildPhase>(key, val);
                            CopyFilesBuildPhases.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXFileReference":
                        {
                            var obj = ImportObject<PBXFileReference>(key, val);
                            FileReferences.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXFrameworksBuildPhase":
                        {
                            var obj = ImportObject<PBXFrameworksBuildPhase>(key, val);
                            FrameworksBuildPhases.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXGroup":
                        {
                            var obj = ImportObject<PBXGroup>(key, val);
                            Groups.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXHeadersBuildPhase":
                        {
                            var obj = ImportObject<PBXHeadersBuildPhase>(key, val);
                            HeadersBuildPhases.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXNativeTarget":
                        {
                            var obj = ImportObject<PBXNativeTarget>(key, val);
                            NativeTargets.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXProject":
                        {
                            var obj = ImportObject<PBXProject>(key, val);
                            Projects.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXResourcesBuildPhase":
                        {
                            var obj = ImportObject<PBXResourcesBuildPhase>(key, val);
                            ResourcesBuildPhases.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXShellScriptBuildPhase":
                        {
                            var obj = ImportObject<PBXShellScriptBuildPhase>(key, val);
                            ShellScriptBuildPhases.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXSourcesBuildPhase":
                        {
                            var obj = ImportObject<PBXSourcesBuildPhase>(key, val);
                            SourcesBuildPhases.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "PBXTargetDependency":
                        {
                            var obj = ImportObject<PBXTargetDependency>(key, val);
                            TargetDependencies.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "XCBuildConfiguration":
                        {
                            var obj = ImportObject<XCBuildConfiguration>(key, val);
                            BuildConfigurations.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    case "XCConfigurationList":
                        {
                            var obj = ImportObject<XCConfigurationList>(key, val);
                            ConfigurationLists.Add(obj);
                            ObjectMap.Add(obj.Key, obj);
                            break;
                        }
                    default:
                        throw new System.Exception("Unsupported ISA type: " + isa);
                }
            }
        }

        private void ImportFields<T>(ref T dst, DictionaryList src)
        {
            PropertyInfo[] members = dst.GetType().GetProperties();
            foreach (var field in members)
            {
                var data = GetValue(src, field.Name) as object;

                var prop = dst.GetType().GetProperty(field.Name);
                prop.SetValue(dst, data, null);
            }
        }

        private PBXObject<T> ImportObject<T>(string key, DictionaryList src)
        {
            var res = new PBXObject<T>
            {
                Key = key,
                Value = System.Activator.CreateInstance<T>()
            };
            ImportFields(ref res.Value, src);
            return res;
        }

    }
}
