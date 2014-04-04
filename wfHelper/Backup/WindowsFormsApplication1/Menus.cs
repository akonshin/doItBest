using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Threading;


namespace WindowsFormsApplication1
{    
    public partial class Form1 : Form
    {
        // ----------------------------------- Main Menu ----------------------------------------------

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
        }

        // ----------------------------------- Minimize / Maximize Main Form ----------------------------------------------

        public void Form1_Deactivate(object sender, EventArgs e)
        {//свернуть трей
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                notifyIcon1.Visible = true;
            }
        }

        public void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MainWindowRestore();
        }

        public void MainWindowRestore() //востановление из трея
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                notifyIcon1.Visible = false;
            }
        }

        private void notifyIcon1_MouseDown(object sender, MouseEventArgs e)
        {
            ContextMenuStrip cms = new ContextMenuStrip();
            cms.Items.Add("Restore");
            cms.Items.Add("Exit");
            notifyIcon1.ContextMenuStrip = cms;
            cms.ItemClicked += new ToolStripItemClickedEventHandler(cms_ItemClicked);
        }

        void cms_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.ToString())
            {
                case "Exit":
                    this.Close();
                    break;
                case "Restore":
                    MainWindowRestore();
                    break;
                default:
                    break;
            }
        }

        // ----------------------------------- Logs context Menu ----------------------------------------------

        private void button3_Click(object sender, EventArgs e)
        {
            Point p = button3.PointToScreen(Point.Empty);
            contextMenuStrip1.Show(p);
            contextMenuStrip1.Show(p);            
        }


        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            contextMenuStrip1.Items.Clear();
            int i = 0;
            foreach (KeyValuePair<string, string> kvp in fl.FilesList)
            {
                contextMenuStrip1.Items.Add(kvp.Key);
                i++;
            }

        }


        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                Process p = new Process();
                ProcessStartInfo psi = new ProcessStartInfo(e.ClickedItem.ToString() + ".log");
                psi.WorkingDirectory = fl.LogsDirectory;
                p = Process.Start(psi);
                p.Close();
            }
            catch
            { 
            
            }
        }       

        

        

    }
}
