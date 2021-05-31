
namespace PBXEdit
{
    partial class DockDocument
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
            this.TextBox = new ScintillaNET.Scintilla();
            this.SuspendLayout();
            // 
            // TextBox
            // 
            this.TextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox.EolMode = ScintillaNET.Eol.Lf;
            this.TextBox.Lexer = ScintillaNET.Lexer.Cpp;
            this.TextBox.Location = new System.Drawing.Point(0, 0);
            this.TextBox.Name = "TextBox";
            this.TextBox.Size = new System.Drawing.Size(615, 450);
            this.TextBox.TabIndex = 0;
            this.TextBox.Text = "Hello";
            // 
            // DockDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TextBox);
            this.DockText = "Untitled";
            this.Name = "DockDocument";
            this.Size = new System.Drawing.Size(615, 450);
            this.ResumeLayout(false);

        }

        #endregion

        private ScintillaNET.Scintilla TextBox;
    }
}
