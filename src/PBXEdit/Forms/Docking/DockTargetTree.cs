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

        void Populate()
        {
            treeView.Nodes.Clear();

            if (Project == null)
                return;

            var root = Project.getRoot();
            if (root is PBXObject<PBXProject>)
            {
                PopulateTargets(root as PBXObject<PBX.PBXProject>);
            }
        }
        void PopulateTargets(PBXObject<PBXProject> root)
        {
            foreach (var targetId in root.Value.targets)
            {
                var buildTarget = Project.findObject(targetId);
                if (buildTarget is PBXObject<PBXNativeTarget>)
                {
                    AddBuildTarget(buildTarget as PBXObject<PBXNativeTarget>);
                }
            }
        }

        void AddBuildTarget(PBXObject<PBXNativeTarget> target)
        {
            var name = target.Value.name;

            var node = new DarkTreeNode(name);
            node.Icon = Icons.properties_16xLG;
            node.Tag = target;

            treeView.Nodes.Add(node);
        }

    }
}
