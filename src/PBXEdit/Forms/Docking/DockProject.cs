using DarkUI.Docking;

namespace PBXEdit
{
    public partial class DockProject : DarkToolWindow
    {
        protected PBX.File pbx;

        public PBX.File Project
        {
            get
            {
                return pbx;
            }
            set
            {
                pbx = value;
                OnProjectChanged();
            }
        }

        public virtual void OnProjectChanged()
        {
        }
    }
}
