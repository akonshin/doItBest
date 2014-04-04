using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;


namespace WFHelper
{
    public partial class MainForm
    {
        private Process _openLogProcess;


        // ----------------------------------- Main Menu ----------------------------------------------

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f2 = new Settings();

            f2.Show();


        }

        // ----------------------------------- Minimize / Maximize Main Form ----------------------------------------------

        public void MainFormDeactivate(object sender, EventArgs e)
        {//свернуть трей
            if (WindowState != FormWindowState.Minimized) return;

            ShowInTaskbar = false;
            Notify.Visible = true;
        }

        public void NotifyMouseDoubleClick(object sender, MouseEventArgs e)
        {
            MainWindowRestore();
        }

        public void MainWindowRestore() //востановление из трея
        {
            if (WindowState != FormWindowState.Minimized) return;

            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            Notify.Visible = false;
        }

        private void notifyIcon1_MouseDown(object sender, MouseEventArgs e)
        {
            using (var cms = new ContextMenuStrip())
            {
                cms.Items.Add("Restore");
                cms.Items.Add("Exit");
                Notify.ContextMenuStrip = cms;
                cms.ItemClicked += cms_ItemClicked;
            }

        }

        void cms_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.ToString())
            {
                case "Exit":
                    Close();
                    break;
                case "Restore":
                    MainWindowRestore();
                    break;
            }
        }

        // ----------------------------------- Logs context Menu ----------------------------------------------        

        private void buildLogs_MouseDown(object sender, MouseEventArgs e)
        {
            var point = buildLogs.PointToScreen(Point.Empty);
            ContextMenu2.Show(point);
            ContextMenu2.Show(point);
        }


        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            ContextMenu2.Items.Clear();

            foreach (var kvp in _fl.FilesList)
            {
                ContextMenu2.Items.Add(kvp.Key);
            }

        }


        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                _openLogProcess = new Process();
                var psi = new ProcessStartInfo(e.ClickedItem + ".log") { WorkingDirectory = _fl.LogsDirectory };
                _openLogProcess = Process.Start(psi);

            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            {

            }
            finally
            {
                _openLogProcess.Dispose();
            }
        }




    }
}
