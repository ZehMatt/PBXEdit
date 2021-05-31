using PBX;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PBXEdit
{
    public partial class DlgAddFile : DarkUI.Forms.DarkForm
    {
        private PBX.File Project;
        private string[] Files;
        private PBXObject<PBXGroup> TargetGroup;

        public DlgAddFile(PBX.File project, string[] files, PBXObject<PBXGroup> group)
        {
            Project = project;
            Files = files;
            TargetGroup = group;

            InitializeComponent();
            Populate(Project, files);
        }

        private void Populate(PBX.File projectFile, string[] files)
        {
            foreach (var file in files)
            {
                fileList.Items.Add(new DarkUI.Controls.DarkListItem(file));
            }

            if (projectFile.GetRoot() is PBXObject<PBXProject> proj)
            {
                PopulateTargets(projectFile, proj);
            }
        }

        private void PopulateTargets(PBX.File projectFile, PBXObject<PBXProject> root)
        {
            foreach (var targetId in root.Value.targets)
            {
                if (projectFile.FindObject(targetId) is PBXObject<PBXNativeTarget> buildTarget)
                {
                    AddBuildTarget(buildTarget);
                }
            }
        }

        private void AddBuildTarget(PBXObject<PBXNativeTarget> target)
        {
            var name = target.Value.name;

            var targetEntry = new DarkUI.Controls.DarkListItem(name)
            {
                Tag = target
            };

            listTargets.Items.Add(targetEntry);
        }

        private static string ToPBXPath(string path)
        {
            string res = path.Replace("\\", "/");
            if (res.StartsWith("/"))
            {
                return res.Substring(1);
            }

            return res;
        }

        private static string GetFileType(string fileName)
        {
            if (fileName.EndsWith(".cpp"))
            {
                return "sourcecode.cpp.cpp";
            }
            else if (fileName.EndsWith(".c"))
            {
                return "sourcecode.c.c";
            }
            else if (fileName.EndsWith(".hpp"))
            {
                return "sourcecode.cpp.h";
            }
            else if (fileName.EndsWith(".h"))
            {
                return "sourcecode.c.h";
            }
            else if (fileName.EndsWith(".txt"))
            {
                return "text";
            }
            // Probably a bad idea.
            return "sourcecode.c.c";
        }

        private bool IsSourceFile(string file)
        {
            return file.EndsWith(".cpp") || file.EndsWith(".c");
        }

        private void AddFileToTarget(PBXObject<PBXFileReference> file, PBXObject<PBXBuildFile> buildFile, PBXObject<PBXNativeTarget> target)
        {
            if (IsSourceFile(file.Value.name))
            {
                foreach (var phaseKey in target.Value.buildPhases)
                {
                    if (Project.FindObject(phaseKey) is PBXObject<PBXSourcesBuildPhase> phase)
                    {
                        var sources = phase as PBXObject<PBXSourcesBuildPhase>;
                        sources.Value.files.Add(buildFile.Key);
                        break;
                    }
                }
            }
            else
            {
                foreach (var phaseKey in target.Value.buildPhases)
                {
                    if (Project.FindObject(phaseKey) is PBXObject<PBXHeadersBuildPhase> phase)
                    {
                        var sources = phase as PBXObject<PBXHeadersBuildPhase>;
                        sources.Value.files.Add(buildFile.Key);
                        break;
                    }
                }
            }
        }

        private void AddFileEntry(string filePath, List<PBXObject<PBXNativeTarget>> targets)
        {
            var sourceRoot = Project.GetSourceRoot();

            if (!filePath.StartsWith(sourceRoot))
            {
                // Unsupported
                return;
            }

            var sourcePath = filePath.Substring(sourceRoot.Length);
            sourcePath = ToPBXPath(sourcePath);

            var fileRefObj = Project.AddFileReference();
            var fileRef = fileRefObj.Value;
            fileRef.path = sourcePath;
            fileRef.sourceTree = "SOURCE_ROOT";
            fileRef.name = System.IO.Path.GetFileName(filePath);
            fileRef.fileEncoding = PBX.PBXFileEncoding.UTF8;
            fileRef.lastKnownFileType = GetFileType(fileRef.name);

            TargetGroup.Value.children.Add(fileRefObj.Key);

            var buildFile = Project.AddBuildFile(fileRefObj);

            foreach (var target in targets)
            {
                AddFileToTarget(fileRefObj, buildFile, target);
            }
        }

        private void OnClickAddFiles(object sender, System.EventArgs e)
        {
            var targets = new List<PBXObject<PBXNativeTarget>>();
            foreach (var selIdx in listTargets.SelectedIndices)
            {
                var item = listTargets.Items[selIdx];
                targets.Add(item.Tag as PBXObject<PBXNativeTarget>);
            }

            if (targets.Count == 0)
            {
                MessageBox.Show("ERROR: Must have one or more targets selected");
                return;
            }

            foreach (var file in Files)
            {
                AddFileEntry(file, targets);
            }

            Close();
        }
    }
}
