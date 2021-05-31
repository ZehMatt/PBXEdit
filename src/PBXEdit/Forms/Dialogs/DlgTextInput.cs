namespace PBXEdit
{
    public partial class DlgTextInput : DarkUI.Forms.DarkForm
    {
        public string InputText { get => txtInput.Text; }

        public DlgTextInput()
        {
            InitializeComponent();
        }

        private void OnClickOk(object sender, System.EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void OnClickCancel(object sender, System.EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }
    }
}
