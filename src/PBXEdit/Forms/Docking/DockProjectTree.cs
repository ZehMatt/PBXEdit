using DarkUI.Controls;
using PBX;
using System;
using System.Collections.Generic;

namespace PBXEdit
{
    public partial class DockProjectTree : DockProject
    {
        private class FileNode
        {
            public string Name;
            public string FilePath;
            public PBXObject<PBXFileReference> File;
            public PBXObject<PBXGroup> Group;
        }

        private class GroupNode
        {
            public string Name;
            public string FilePath;
            public PBXObject<PBXGroup> Group;
        }

        public DockProjectTree()
        {
            InitializeComponent();
        }
        public override void OnProjectChanged()
        {
            base.OnProjectChanged();
            Populate();
        }

        private void Populate()
        {
            treeView.Nodes.Clear();

            if (Project == null)
            {
                return;
            }

            if (Project.GetRoot() is PBXObject<PBXProject> proj)
            {
                PopulateProject(proj);
            }
        }

        private void PopulateProject(PBXObject<PBXProject> root)
        {
            var mainGroup = root.Value.mainGroup;

            var group = Project.FindObject<PBXGroup>(mainGroup);
            if (group == null)
            {
                return;
            }

            PopulateGroup(null, group, "");
        }

        private string CombinePath(string a, string b)
        {
            if (b == null)
            {
                return a;
            }

            if (a.EndsWith("/"))
            {
                a += b;
            }
            else
            {
                a += "/" + b;
            }

            return a;
        }

        private DarkTreeNode PopulateGroup(DarkTreeNode parentNode, PBXObject<PBXGroup> group, string path, DarkTreeNode target = null)
        {
            var name = group.Value.name ?? group.Value.path;
            if (name == null && parentNode == null)
            {
                name = "Project";
            }

            var groupNode = target ?? new DarkTreeNode(name);
            groupNode.ExpandedIcon = Icons.folder_16x;
            groupNode.Icon = Icons.folder_Closed_16xLG;

            var groupPath = path;
            if (group.Value.sourceTree == "SOURCE_ROOT")
            {
                groupPath = group.Value.path;
            }
            else
            {
                groupPath = CombinePath(groupPath, group.Value.path);
            }

            var groupInfo = new GroupNode
            {
                Name = name,
                Group = group,
                FilePath = path
            };

            groupNode.Tag = groupInfo;

            if (group.Value.children == null)
            {
                group.Value.children = new List<dynamic>();
            }

            foreach (var child in group.Value.children)
            {
                var childId = child as string;

                var fileRef = Project.FindObject(childId);
                if (fileRef is PBXObject<PBXGroup>)
                {
                    PopulateGroup(groupNode, fileRef, groupPath);
                }
                else if (fileRef is PBXObject<PBXFileReference> fileObj)
                {
                    var fileName = fileObj.Value.name ?? fileObj.Value.path;

                    var info = new FileNode
                    {
                        Name = fileName,
                        Group = group,
                        File = fileObj
                    };
                    if (fileObj.Value.sourceTree == "SOURCE_ROOT")
                    {
                        info.FilePath = fileObj.Value.path;
                    }
                    else
                    {
                        info.FilePath = CombinePath(groupPath, fileName);
                    }

                    var fileNode = new DarkTreeNode(fileName)
                    {
                        Icon = Icons.Files_7954,
                        Tag = info
                    };

                    groupNode.Nodes.Add(fileNode);
                }
            }

            if (parentNode != null)
            {
                if (target == null)
                {
                    parentNode.Nodes.Add(groupNode);
                }
            }
            else
            {
                treeView.Nodes.Add(groupNode);
            }

            return groupNode;
        }

        private DarkTreeNode GetSelectedObject()
        {
            foreach (var selected in treeView.SelectedNodes)
            {
                return selected;
            }
            return null;
        }

        private void OnSelectionChange(object sender, System.EventArgs e)
        {
            treeView.ContextMenuStrip = null;

            var selected = GetSelectedObject();
            if (selected == null)
            {
                return;
            }

            if (selected.Tag is GroupNode)
            {
                treeView.ContextMenuStrip = treeContextMenuDir;
            }
            else if (selected.Tag is FileNode)
            {
                treeView.ContextMenuStrip = treeContextMenuFile;
            }
        }

        public void AddFiles(PBXObject<PBXGroup> group, List<string> files)
        {
            var dlgAdd = new DlgAddFile(Project, files.ToArray(), group);
            dlgAdd.ShowDialog();
        }

        private List<string> BrowseForFiles()
        {
            var files = new List<string>();

            openFileDialog1.Multiselect = true;
            openFileDialog1.Title = "Select files to add...";
            openFileDialog1.Filter = "Source Code (*.h;*.hpp;*.cpp;*.c;*.cxx)|*.h;*.hpp;*.cpp;*.c;*.cxx|All Files (*.*)|*.*";

            var res = openFileDialog1.ShowDialog();
            if (res != System.Windows.Forms.DialogResult.OK)
            {
                return files;
            }

            foreach (var file in openFileDialog1.FileNames)
            {
                files.Add(file);
            }

            return files;
        }

        private void InvalidateNodeTree(DarkTreeNode selectedNode)
        {
            var groupInfo = selectedNode.Tag as GroupNode;

            bool expanded = selectedNode.Expanded;

            selectedNode.Nodes.Clear();

            PopulateGroup(selectedNode.ParentNode, groupInfo.Group, groupInfo.FilePath, selectedNode);

            selectedNode.Expanded = expanded;
        }

        private void OnClickAddFile(object sender, System.EventArgs e)
        {
            var selectedNode = GetSelectedObject();
            if (selectedNode == null)
            {
                return;
            }

            var selected = selectedNode.Tag;
            if (selected is GroupNode)
            {
                var groupInfo = selected as GroupNode;

                var files = BrowseForFiles();
                if (files.Count == 0)
                {
                    return;
                }

                AddFiles(groupInfo.Group, files);

                InvalidateNodeTree(selectedNode);
            }
        }

        private void OnClickOpenFile(object sender, System.EventArgs e)
        {
            var selectedNode = GetSelectedObject();
            if (selectedNode == null)
            {
                return;
            }

            var selected = selectedNode.Tag;
            if (selected is FileNode fileInfo)
            {
                var main = Program.GetMain();
                main.OpenDocument(fileInfo.FilePath);
            }
        }

        private void OnDblClick(object sender, EventArgs e)
        {
            OnClickOpenFile(sender, e);
        }

        private void RemoveFile(DarkTreeNode node)
        {
            var fileNode = node.Tag as FileNode;

            Project.RemoveObjectAndDependencies(fileNode.File);

            node.Remove();
        }

        private void RemoveRecursive(DarkTreeNode node)
        {
            var groupNode = node.Tag as GroupNode;

            for (int i = node.Nodes.Count - 1; i >= 0; i--)
            {
                var child = node.Nodes[i];
                if (child.Tag is GroupNode)
                {
                    RemoveRecursive(child);
                }
                else if (child.Tag is FileNode)
                {
                    RemoveFile(child);
                }
            }

            Project.RemoveObjectAndDependencies(groupNode.Group);

            node.Remove();
        }

        private void OnRemove(object sender, EventArgs e)
        {
            var selectedNode = GetSelectedObject();
            if (selectedNode == null)
            {
                return;
            }

            if (selectedNode.Tag is GroupNode)
            {
                RemoveRecursive(selectedNode);
            }
            else if (selectedNode.Tag is FileNode)
            {
                RemoveFile(selectedNode);
            }
        }

        private void OnDelete(object sender, EventArgs e)
        {

        }

        private void OnAddGroup(object sender, EventArgs e)
        {
            var selectedNode = GetSelectedObject();
            if (selectedNode == null)
            {
                return;
            }

            if (!(selectedNode.Tag is GroupNode))
            {
                return;
            }

            var groupNode = selectedNode.Tag as GroupNode;
            var group = groupNode.Group;

            var groupNameDlg = new DlgTextInput();
            groupNameDlg.Text = "Enter a name";
            var res = groupNameDlg.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }

            var groupName = groupNameDlg.InputText;
            if (groupName.Length == 0)
            {
                return;
            }

            // Keep folders always at the top.
            int idx = 0;
            foreach (var childKey in group.Value.children)
            {
                var child = Project.FindObject(childKey);
                if (child is PBXObject<PBXFileReference>)
                {
                    break;
                }
                idx++;
            }

            var newGroup = Project.AddGroup();
            newGroup.Value.name = groupName;
            newGroup.Value.sourceTree = "<group>";

            group.Value.children.Insert(idx, newGroup.Key);

            InvalidateNodeTree(selectedNode);
        }
    }
}
