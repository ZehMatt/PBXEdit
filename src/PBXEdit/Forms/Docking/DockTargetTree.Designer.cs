
namespace PBXEdit
{
    partial class DockTargetTree
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.treeView = new DarkUI.Controls.DarkTreeView();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.AllowDrop = true;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Location = new System.Drawing.Point(0, 25);
            this.treeView.MaxDragChange = 20;
            this.treeView.Name = "treeView";
            this.treeView.ShowIcons = true;
            this.treeView.Size = new System.Drawing.Size(350, 425);
            this.treeView.TabIndex = 1;
            // 
            // DockTargetTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView);
            this.DockText = "Targets";
            this.Name = "DockTargetTree";
            this.Size = new System.Drawing.Size(350, 450);
            this.ResumeLayout(false);

        }

        #endregion

        private DarkUI.Controls.DarkTreeView treeView;
    }
}
