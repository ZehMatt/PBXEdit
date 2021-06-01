using DarkUI.Controls;
using PBX;

namespace PBXEdit
{
    public partial class DockTargetTree : DockProject
    {
        public DockTargetTree()
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
                PopulateTargets(proj);
            }
        }

        private void PopulateTargets(PBXObject<PBXProject> root)
        {
            foreach (var targetId in root.Value.targets)
            {
                var obj = Project.FindObject(targetId);
                if (obj is PBXObject<PBXNativeTarget> target)
                {
                    AddBuildTarget(target);
                }
            }
        }

        private void AddBuildTarget(PBXObject<PBXNativeTarget> target)
        {
            var name = target.Value.name;
            var node = new DarkTreeNode(name)
            {
                Icon = Icons.properties_16xLG,
                Tag = target
            };

            treeView.Nodes.Add(node);
        }

    }
}
