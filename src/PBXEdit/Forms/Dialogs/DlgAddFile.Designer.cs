
namespace PBXEdit
{
    partial class DlgAddFile
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
            this.darkLabel1 = new DarkUI.Controls.DarkLabel();
            this.btOk = new DarkUI.Controls.DarkButton();
            this.listTargets = new DarkUI.Controls.DarkListView();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.fileList = new DarkUI.Controls.DarkListView();
            this.darkLabel2 = new DarkUI.Controls.DarkLabel();
            this.SuspendLayout();
            // 
            // darkLabel1
            // 
            this.darkLabel1.AutoSize = true;
            this.darkLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel1.Location = new System.Drawing.Point(13, 221);
            this.darkLabel1.Name = "darkLabel1";
            this.darkLabel1.Size = new System.Drawing.Size(141, 13);
            this.darkLabel1.TabIndex = 1;
            this.darkLabel1.Text = "Select targets to add files to:";
            // 
            // btOk
            // 
            this.btOk.Location = new System.Drawing.Point(236, 383);
            this.btOk.Name = "btOk";
            this.btOk.Padding = new System.Windows.Forms.Padding(5);
            this.btOk.Size = new System.Drawing.Size(75, 23);
            this.btOk.TabIndex = 2;
            this.btOk.Text = "OK";
            this.btOk.Click += new System.EventHandler(this.OnClickAddFiles);
            // 
            // listTargets
            // 
            this.listTargets.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.listTargets.Location = new System.Drawing.Point(16, 238);
            this.listTargets.MultiSelect = true;
            this.listTargets.Name = "listTargets";
            this.listTargets.Size = new System.Drawing.Size(296, 139);
            this.listTargets.TabIndex = 3;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // fileList
            // 
            this.fileList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.fileList.Location = new System.Drawing.Point(15, 26);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(296, 146);
            this.fileList.TabIndex = 5;
            // 
            // darkLabel2
            // 
            this.darkLabel2.AutoSize = true;
            this.darkLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.darkLabel2.Location = new System.Drawing.Point(12, 9);
            this.darkLabel2.Name = "darkLabel2";
            this.darkLabel2.Size = new System.Drawing.Size(45, 13);
            this.darkLabel2.TabIndex = 4;
            this.darkLabel2.Text = "File List:";
            // 
            // DlgAddFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.ClientSize = new System.Drawing.Size(324, 415);
            this.Controls.Add(this.fileList);
            this.Controls.Add(this.darkLabel2);
            this.Controls.Add(this.listTargets);
            this.Controls.Add(this.btOk);
            this.Controls.Add(this.darkLabel1);
            this.ForeColor = System.Drawing.Color.Gainsboro;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "DlgAddFile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add File(s)";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DarkUI.Controls.DarkLabel darkLabel1;
        private DarkUI.Controls.DarkButton btOk;
        private DarkUI.Controls.DarkListView listTargets;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private DarkUI.Controls.DarkListView fileList;
        private DarkUI.Controls.DarkLabel darkLabel2;
    }
}
