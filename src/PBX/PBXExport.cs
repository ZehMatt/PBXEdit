using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace PBX
{
    using DictionaryList = List<KeyValuePair<string, dynamic>>;

    public partial class File
    {
        public void Save(string file)
        {
            try
            {
                // Always save as UTF8.
                var encoding = new UTF8Encoding(false);
                using (var stream = new StreamWriter(file, false, encoding))
                {
                    var sw = new Writer(stream);
                    Save(sw);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Save(Writer sw)
        {
            // Save encoding in header
            sw.WriteLine("// !$*UTF8*$!");

            sw.BeginScope();

            SaveField(sw, "archiveVersion", archiveVersion);
            SaveField(sw, "classes", classes);
            SaveField(sw, "objectVersion", objectVersion);

            SaveObjects(sw);

            SaveField(sw, "rootObject", rootObject, GetObjectComment(rootObject));

            sw.EndScope();
            sw.WriteLine("");
        }

        private void SaveObjects(Writer sw)
        {
            sw.WriteIndent();
            sw.Write("objects = ");

            sw.BeginDict();

            SaveObjectSection(sw, AggregateTargets);
            SaveObjectSection(sw, BuildFiles);
            SaveObjectSection(sw, ContainerItemProxies);
            SaveObjectSection(sw, CopyFilesBuildPhases);
            SaveObjectSection(sw, FileReferences);
            SaveObjectSection(sw, FrameworksBuildPhases);
            SaveObjectSection(sw, Groups);
            SaveObjectSection(sw, HeadersBuildPhases);
            SaveObjectSection(sw, NativeTargets);
            SaveObjectSection(sw, Projects);
            SaveObjectSection(sw, ResourcesBuildPhases);
            SaveObjectSection(sw, ShellScriptBuildPhases);
            SaveObjectSection(sw, SourcesBuildPhases);
            SaveObjectSection(sw, TargetDependencies);
            SaveObjectSection(sw, BuildConfigurations);
            SaveObjectSection(sw, ConfigurationLists);

            sw.EndDict();
            sw.WriteLine(";");
        }

        private void SaveStringValue(Writer sw, string val, string comment, bool inArray = false)
        {
            val = val.Replace("\"", "\\\"");
            if (inArray)
            {
                if (val.Contains("\"") ||
                    val.Contains(" ") ||
                    val.Contains("\t") ||
                    val.Contains(",") ||
                    val.Contains(";") ||
                    val.Contains("<") ||
                    val.Contains(">") ||
                    val.Contains("(") ||
                    val.Contains(")") ||
                    val.Contains("-") ||
                    val.Contains("+") ||
                    val.Contains("=") ||
                    val.Contains(".") ||
                    val.Length == 0)
                {
                    val = "\"" + val + "\"";
                }
            }
            else
            {
                if (val.Contains("\"") ||
                    val.Contains(" ") ||
                    val.Contains("<") ||
                    val.Contains(">") ||
                    val.Contains("(") ||
                    val.Contains(")") ||
                    val.Contains("-") ||
                    val.Contains("+") ||
                    val.Contains("=") ||
                    val.Contains("\t") ||
                    val.Length == 0)
                {
                    val = "\"" + val + "\"";
                }
            }

            sw.Write(val);
            if (comment != null && comment.Length > 0)
            {
                sw.Write(" /* {0} */", comment);
            }
        }

        private void SaveDictionary(Writer sw, DictionaryList dict)
        {
            sw.BeginDict();

            foreach (var pair in dict)
            {
                SaveField(sw, pair.Key, pair.Value);
            }

            sw.EndDict();
        }

        private void SaveAssignment(Writer sw, string key, string keyComment = "")
        {
            sw.WriteIndent();
            if (keyComment != null && keyComment.Length > 0)
            {
                sw.Write("{0} /* {1} */ = ", key, keyComment);
            }
            else
            {
                sw.Write("{0} = ", key);
            }
        }

        private void SaveArray(Writer sw, List<dynamic> arr)
        {
            sw.BeginArray();

            foreach (var entry in arr)
            {
                sw.WriteIndent();

                var comment = GetObjectComment(entry);
                SaveFieldValue(sw, entry, comment, true);

                sw.Write(",");
                sw.NewLine();
            }

            sw.EndArray();
        }

        private void SaveFieldValue(Writer sw, dynamic val, string valueComment = "", bool inArray = false)
        {
            if (val is string)
            {
                var str = val as string;
                SaveStringValue(sw, str, valueComment, inArray);
            }
            else if (val is List<dynamic>)
            {
                var list = val as List<dynamic>;
                SaveArray(sw, list);
            }
            else if (val is DictionaryList)
            {
                var list = val as DictionaryList;
                SaveDictionary(sw, list);
            }
        }

        private void SaveField(Writer sw, string key, dynamic val, string valueComment = "")
        {
            if (val == null)
            {
                return;
            }

            SaveAssignment(sw, key);
            SaveFieldValue(sw, val, valueComment);
            sw.Write(";");
            sw.NewLine();
        }

        private bool ShouldCommentField(string name)
        {
            switch (name)
            {
                case "remoteGlobalIDString":
                    return false;
            }
            return true;
        }

        private void SaveFields<T>(Writer sw, T src)
        {
            PropertyInfo[] members = src.GetType().GetProperties();
            SaveField(sw, "isa", src.GetType().Name);
            foreach (var field in members)
            {
                var prop = src.GetType().GetProperty(field.Name);
                var data = prop.GetValue(src, null);

                string valueComment = ShouldCommentField(field.Name) ? GetObjectComment(data) : null;
                SaveField(sw, prop.Name, data, valueComment);
            }
        }

        private bool WriteInline<T>(T src)
        {
            switch (src.GetType().Name)
            {
                case "PBXBuildFile":
                case "PBXFileReference":
                    return true;
            }
            return false;
        }

        private string GetProjectName(PBXProject proj)
        {
            // This is potentially wrong, this is based on guess-work.
            if (proj.targets.Count > 0)
            {
                var target = proj.targets[0];
                var targetObj = FindObject(target);
                if (targetObj != null && targetObj is PBXObject<PBXNativeTarget>)
                {
                    var nativeTarget = targetObj as PBXObject<PBXNativeTarget>;
                    return nativeTarget.Value.name;
                }
            }
            return "";
        }

        private string GetObjectComment(dynamic entry)
        {
            if (entry == null)
            {
                return null;
            }

            string keyComment = null;
            if (entry is PBXObject<PBXAggregateTarget>)
            {
                var elem = entry as PBXObject<PBXAggregateTarget>;
                keyComment = elem.Value.name;
            }
            else if (entry is PBXObject<PBXFileReference>)
            {
                var cur = entry as PBXObject<PBXFileReference>;
                if (cur.Value.name != null)
                {
                    keyComment = cur.Value.name;
                }
                else
                {
                    keyComment = cur.Value.path;
                }
            }
            else if (entry is PBXObject<PBXBuildFile>)
            {
                var cur = entry as PBXObject<PBXBuildFile>;
                var fileId = cur.Value.fileRef;
                var fileObj = FindObject(fileId);
                var fileRef = fileObj as PBXObject<PBXFileReference>;
                if (fileRef.Value.name != null)
                {
                    keyComment = fileRef.Value.name;
                }
                else
                {
                    keyComment = fileRef.Value.path;
                }

                var fileType = fileRef.Value.explicitFileType;
                if (fileType == null)
                {
                    fileType = fileRef.Value.lastKnownFileType;
                }

                keyComment += " in " + GetFileCategory(cur.Key, fileType);
            }
            else if (entry is PBXObject<PBXContainerItemProxy>)
            {
                keyComment = "PBXContainerItemProxy";
            }
            else if (entry is PBXObject<PBXProject>)
            {
                keyComment = "Project object";
            }
            else if (entry is PBXObject<PBXFrameworksBuildPhase>)
            {
                keyComment = "Frameworks";
            }
            else if (entry is PBXObject<PBXHeadersBuildPhase>)
            {
                keyComment = "Headers";
            }
            else if (entry is PBXObject<PBXSourcesBuildPhase>)
            {
                keyComment = "Sources";
            }
            else if (entry is PBXObject<PBXNativeTarget>)
            {
                var cur = entry as PBXObject<PBXNativeTarget>;
                keyComment = cur.Value.name;
            }
            else if (entry is PBXObject<PBXGroup>)
            {
                var cur = entry as PBXObject<PBXGroup>;
                keyComment = cur.Value.name != null ? cur.Value.name : cur.Value.path;
            }
            else if (entry is PBXObject<PBXCopyFilesBuildPhase>)
            {
                var cur = entry as PBXObject<PBXCopyFilesBuildPhase>;
                keyComment = cur.Value.name != null ? cur.Value.name : "CopyFiles";
            }
            else if (entry is PBXObject<PBXFileReference>)
            {
                var fileRef = entry as PBXObject<PBXFileReference>;
                if (fileRef.Value.name != null)
                {
                    keyComment = fileRef.Value.name;
                }
                else
                {
                    keyComment = fileRef.Value.path;
                }
            }
            else if (entry is PBXObject<PBXShellScriptBuildPhase>)
            {
                var cur = entry as PBXObject<PBXShellScriptBuildPhase>;
                keyComment = cur.Value.name;
            }
            else if (entry is PBXObject<PBXTargetDependency>)
            {
                keyComment = "PBXTargetDependency";
            }
            else if (entry is PBXObject<PBXResourcesBuildPhase>)
            {
                keyComment = "Resources";
            }
            else if (entry is PBXObject<XCConfigurationList>)
            {
                var buildRef = FindBuildConfigurationRef(entry.Key);
                if (buildRef != null)
                {
                    if (buildRef is PBXObject<PBXProject>)
                    {
                        var obj = buildRef as PBXObject<PBXProject>;
                        var name = GetProjectName(obj.Value);
                        keyComment = string.Format("Build configuration list for PBXProject \"{0}\"", name);
                    }
                    else if (buildRef is PBXObject<PBXNativeTarget>)
                    {
                        var obj = buildRef as PBXObject<PBXNativeTarget>;
                        keyComment = string.Format("Build configuration list for PBXNativeTarget \"{0}\"", obj.Value.name);
                    }
                    else if (buildRef is PBXObject<PBXAggregateTarget>)
                    {
                        var obj = buildRef as PBXObject<PBXAggregateTarget>;
                        keyComment = string.Format("Build configuration list for PBXAggregateTarget \"{0}\"", obj.Value.name);
                    }
                }
            }
            else if (entry is PBXObject<XCBuildConfiguration>)
            {
                var cur = entry as PBXObject<XCBuildConfiguration>;
                return cur.Value.name;
            }
            else if (entry is string)
            {
                return GetObjectComment(FindObject(entry as string));
            }

            return keyComment;
        }

        private string GetFileCategory(string fileId, string fileType)
        {
            foreach (var entry in this.ResourcesBuildPhases)
            {
                foreach (var elem in entry.Value.files)
                {
                    if (elem == fileId)
                    {
                        return "Resources";
                    }
                }
            }

            foreach (var entry in this.CopyFilesBuildPhases)
            {
                foreach (var elem in entry.Value.files)
                {
                    if (elem == fileId)
                    {
                        return entry.Value.name == null ? "CopyFiles" : entry.Value.name;
                    }
                }
            }

            foreach (var entry in this.FrameworksBuildPhases)
            {
                foreach (var elem in entry.Value.files)
                {
                    if (elem == fileId)
                    {
                        return "Frameworks";
                    }
                }
            }

            foreach (var entry in this.HeadersBuildPhases)
            {
                foreach (var elem in entry.Value.files)
                {
                    if (elem == fileId)
                    {
                        return "Headers";
                    }
                }
            }

            foreach (var entry in this.SourcesBuildPhases)
            {
                foreach (var elem in entry.Value.files)
                {
                    if (elem == fileId)
                    {
                        return "Sources";
                    }
                }
            }

            return "NOPE";
        }

        private void SaveObjectSection<T>(Writer sw, List<PBXObject<T>> list)
        {
            var section = typeof(T).Name;
            sw.WriteLine("\n/* Begin {0} section */", section);

            foreach (var entry in list)
            {
                var key = entry.Key;
                var val = entry.Value;

                string keyComment = GetObjectComment(entry);
                SaveAssignment(sw, key, keyComment);

                bool inlineScope = WriteInline(val);
                sw.SetInline(inlineScope);

                sw.BeginDict();

                SaveFields(sw, val);

                sw.EndDict();

                sw.SetInline(false);

                sw.WriteLine(";");
            }

            sw.WriteLine("/* End {0} section */", section);
        }
    }
}
