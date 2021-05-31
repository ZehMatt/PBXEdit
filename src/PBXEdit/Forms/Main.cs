using DarkUI.Forms;
using DarkUI.Win32;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace PBXEdit
{
    public partial class FormMain : Form
    {
        PBX.File projectFile;

        DockProjectTree dockProjectTree;
        DockTargetTree dockTargetTree;
        List<DockDocument> documents = new List<DockDocument>();

        public FormMain()
        {
            InitializeComponent();

            // Add the control scroll message filter to re-route all mouse wheel events
            // to the control the user is currently hovering over with their cursor.
            Application.AddMessageFilter(new ControlScrollFilter());

            // Add the dock content drag message filter to handle moving dock content around.
            Application.AddMessageFilter(dockPanel.DockContentDragFilter);

            // Add the dock panel message filter to filter through for dock panel splitter
            // input before letting events pass through to the rest of the application.
            Application.AddMessageFilter(dockPanel.DockResizeFilter);

            dockProjectTree = new DockProjectTree();
            dockProjectTree.DockArea = DarkUI.Docking.DarkDockArea.Left;
            dockProjectTree.DefaultDockArea = DarkUI.Docking.DarkDockArea.Left;
            dockProjectTree.Order = 0;
            dockPanel.AddContent(dockProjectTree, dockPanel.ActiveGroup);

            dockTargetTree = new DockTargetTree();
            dockTargetTree.DockArea = DarkUI.Docking.DarkDockArea.Left;
            dockTargetTree.DefaultDockArea = DarkUI.Docking.DarkDockArea.Left;
            dockTargetTree.Order = 1;
            dockPanel.AddContent(dockTargetTree, dockPanel.ActiveGroup);

            /*
            var newDoc = new DockDocument(@"F:\C++\OpenRCT2\src\openrct2\Game.cpp", Icons.document_16xLG);
            dockPanel.AddContent(newDoc, newDoc.DockGroup);
            */

            dockPanel.ActiveContent = dockProjectTree;

            dockPanel.PerformLayout();
        }

        void SetActiveProject()
        {
            if (dockProjectTree != null)
                dockProjectTree.Project = projectFile;

            if (dockTargetTree != null)
                dockTargetTree.Project = projectFile;
        }

        public void OpenProject(string file)
        {
            try
            {
                projectFile = new PBX.File(file);
            }
            catch (System.Exception)
            {
                MessageBox.Show($"ERROR: Failed to open file: {file}");

                projectFile = null;
                return;
            }

            SetActiveProject();

            mnuBtSaveProject.Enabled = true;
            menuBtSaveProjectAs.Enabled = true;
            closeProjectToolStripMenuItem.Enabled = true;
        }

        public void SaveProject(string file)
        {
            try
            {
                projectFile.save(file);
            }
            catch (System.Exception)
            {
                MessageBox.Show($"ERROR: Failed to save file to: {file}");
            }
        }

        public static string SanitizePath(string path)
        {
            var res = path.Replace('/', '\\');
            if (res.StartsWith("\\"))
                return res.Substring(1);
            return res;
        }

        public void OpenDocument(string file)
        {
            var sourceRoot = projectFile.getSourceRoot();
            var sanitized = SanitizePath(file);
            var fullPath = Path.Combine(sourceRoot, sanitized);

            foreach (var doc in documents)
            {
                if (doc.File == fullPath)
                {
                    doc.Select();
                    return;
                }
            }

            var newDoc = new DockDocument(fullPath, Icons.document_16xLG);
            dockPanel.AddContent(newDoc, newDoc.DockGroup);
        }

        #region Menu Handlers
        void OnMenuOpenProjectClick(object sender, System.EventArgs e)
        {
            if (openFileDlg.ShowDialog() != DialogResult.OK)
                return;

            OpenProject(openFileDlg.FileName);
        }

        void OnMenuCloseProjectClick(object sender, System.EventArgs e)
        {
            projectFile = null;
            SetActiveProject();

            mnuBtSaveProject.Enabled = false;
            menuBtSaveProjectAs.Enabled = false;
            closeProjectToolStripMenuItem.Enabled = false;
        }

        private void OnMenuSaveProjectClick(object sender, System.EventArgs e)
        {
            SaveProject(projectFile.FilePath);
        }

        private void OnClickSaveAs(object sender, System.EventArgs e)
        {
            if (saveFileDlg.ShowDialog() != DialogResult.OK)
                return;

            SaveProject(saveFileDlg.FileName);
        }
        #endregion

        private void OnDockControlAdded(object sender, ControlEventArgs e)
        {
            if (e.Control is DockDocument)
            {
                documents.Add(e.Control as DockDocument);
            }
        }

        private void OnDockControlRemoved(object sender, ControlEventArgs e)
        {
            if (e.Control == dockProjectTree)
                dockProjectTree = null;
            else if (e.Control == dockTargetTree)
                dockTargetTree = null;
            else if (e.Control is DockDocument)
            {
                documents.Remove(e.Control as DockDocument);
            }
        }

        private void OnContentAdded(object sender, DarkUI.Docking.DockContentEventArgs e)
        {
            if (e.Content is DockDocument)
            {
                documents.Add(e.Content as DockDocument);
            }
        }

        private void OnContentRemoved(object sender, DarkUI.Docking.DockContentEventArgs e)
        {
            if (e.Content == dockProjectTree)
                dockProjectTree = null;
            else if (e.Content == dockTargetTree)
                dockTargetTree = null;
            else if (e.Content is DockDocument)
            {
                documents.Remove(e.Content as DockDocument);
            }
        }

        private void OnMnuExitClick(object sender, System.EventArgs e)
        {
            Close();
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            var modified = new List<DockDocument>();

            foreach (var doc in documents)
            {
                if (doc.Modified)
                {
                    modified.Add(doc);
                }
            }

            if (modified.Count > 0)
            {
                var result = DarkMessageBox.ShowWarning("You have unsaved changes, save before closing?", "Close", DarkDialogButton.YesNoCancel);
                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }

                if (result == DialogResult.Yes)
                {
                    foreach (var doc in modified)
                    {
                        doc.SaveFile();
                    }
                }
            }
        }
    }
}
