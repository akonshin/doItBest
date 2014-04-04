using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;


namespace WFHelper
{
    public partial class TestFileImport : Form
    {

        private List<string> HublistF4 { get; set; }
        private string TestFilesDirectory { get; set; }
        private string WorkingDirectory { get; set; }
        private string InboundDirectory { get; set; }
        private Process Proc { get; set; }
        private System.Timers.Timer MyTimer { get; set; }
        private static int _counter;


        public TestFileImport()
        {
            InitializeComponent();
        }

        public TestFileImport(IEnumerable<string> hublist)
        {
            InitializeComponent();

            HublistF4 = hublist.Distinct().ToList();
            TestFilesDirectory = @"C:\testFiles\";
            InboundDirectory = @"C:\jv\src\sps\engine-dist\inbound\webfedsib.dat";
            WorkingDirectory = Application.StartupPath + "\\system_files\\";

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Invoke(new UpdateLabelDelegate(UpdateLabelSafe), "In process...");
                File.WriteAllLines(InboundDirectory, richTextBox1.Lines.ToArray());

                var psi = new ProcessStartInfo(WorkingDirectory + "Run.bat")
                    {
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                Proc = new Process { StartInfo = psi };
                Proc.Start();

                MyTimer = new System.Timers.Timer();
                MyTimer.Elapsed += myTimer_Elapsed;
                MyTimer.Interval = 1000;
                MyTimer.Start();

            }
            catch
            {
                MessageBox.Show(Resources.Resources.TestFileImport_button1_Click_File_is_used_by_another_application);
            }
        }


        void UpdateProgressSafe(int value)
        {
            progressBar1.Value = value;
        }

        void UpdateLabelSafe(string value)
        {
            label1.Text = value;
        }

        delegate void UpdateProgressDelegate(int value);
        delegate void UpdateLabelDelegate(string value);


        void myTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Proc.HasExited)
            {
                MyTimer.Stop();
                Invoke(new UpdateProgressDelegate(UpdateProgressSafe), progressBar1.Maximum);
                Invoke(new UpdateLabelDelegate(UpdateLabelSafe), "Done");
            }
            else
            {
                Invoke(new UpdateProgressDelegate(UpdateProgressSafe), _counter++);
                if (_counter == progressBar1.Maximum)
                {
                    _counter = 0;
                }
            }
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = HublistF4;
            comboBox2.DataSource = null;
            comboBox1.Text = string.Empty;
            comboBox2.Text = string.Empty;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(TestFilesDirectory))
                {
                    Directory.CreateDirectory(TestFilesDirectory);
                }

                string fileName = TestFilesDirectory + comboBox1.Text + comboBox2.Text + ".feds";

                if (File.Exists(fileName))
                {
                    DialogResult dResult = MessageBox.Show(Resources.Resources.TestFileImport_button2_Click_Are_you_sure__, Resources.Resources.TestFileImport_button2_Click_Update_confirmation, MessageBoxButtons.YesNo);

                    if (dResult == DialogResult.Yes)
                    {
                        File.WriteAllLines(fileName, richTextBox1.Lines.ToArray());
                    }
                }
                else
                {
                    File.WriteAllLines(fileName, richTextBox1.Lines.ToArray());
                }
            }
            catch
            {
                MessageBox.Show(Resources.Resources.TestFileImport_button2_Click_Something_happened____);
            }
            comboBox1.Text = string.Empty;
            comboBox2.Text = string.Empty;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var dResult = MessageBox.Show(Resources.Resources.TestFileImport_button2_Click_Are_you_sure__, Resources.Resources.TestFileImport_button3_Click_Delete_confirmation, MessageBoxButtons.YesNo);

            if (dResult != DialogResult.Yes) return;

            var fileforDeleting = TestFilesDirectory + comboBox1.Text + comboBox2.Text + ".feds";

            if (!File.Exists(fileforDeleting))
            {
                MessageBox.Show(Resources.Resources.TestFileImport_button3_Click_File_is_not_exists);
            }
            else
            {
                try
                {
                    File.Delete(fileforDeleting);
                }
                catch
                {
                    MessageBox.Show(
                        Resources.Resources
                                 .TestFileImport_button3_Click_Unable_to_delete_file__it_is_may_be_used_by_another_process);
                }
            }
            comboBox1.Text = string.Empty;
            comboBox2.Text = string.Empty;
        }


        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            FillRichTexBoxFromFile();
        }


        public void FillRichTexBoxFromFile()
        {
            var fileforOpening = string.Empty;
            try
            {
                fileforOpening = TestFilesDirectory + comboBox1.Text + comboBox2.Text + ".feds";
            }
            catch
            {
                messageLabel.Text = Resources.Resources.TestFileImport_FillRichTexBoxFromFile_Test_files_directory_is_not_found;
            }

            if (File.Exists(fileforOpening))
            {
                richTextBox1.Clear();
                richTextBox1.Lines = File.ReadAllLines(fileforOpening);
            }
            else
            {
                richTextBox1.Clear();
            }

        }


        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            var dir = new DirectoryInfo(TestFilesDirectory);
            var fileList = new List<string>();
            var hubName = comboBox1.Text.Trim();

            try
            {
                if (hubName.Length <= 0) return;

                fileList.AddRange(from file in dir.GetFiles(comboBox1.Text + "*.feds") select Path.GetFileNameWithoutExtension(file.FullName) into fileName where fileName != null select fileName.Replace(hubName, string.Empty));

                comboBox2.DataSource = null;
                comboBox2.DataSource = fileList;
            }
            catch { comboBox1.Text = Resources.Resources.TestFileImport_comboBox1_TextChanged_Test_Files_Folder_is_not_exists; }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                Proc.Kill();
            }
            catch
            {
                Invoke(new UpdateProgressDelegate(UpdateProgressSafe), progressBar1.Maximum);
                Invoke(new UpdateLabelDelegate(UpdateLabelSafe), "File isn't importing now");
            }
        }



        private static void ShowImportLogs()
        {
            const string s = "C:\\jv\\src\\sps\\engine-dist\\logs\\";

            try
            {
                var directoryList = Directory.GetDirectories(s);

                foreach (var dir in directoryList)
                {
                    var dinfo = new DirectoryInfo(dir);

                    if (dinfo.CreationTime.ToShortDateString() != DateTime.Now.ToShortDateString()) continue;

                    var fileList = dinfo.GetFiles();
                    var filename = fileList[0].Name;
                    DateTime[] temp = { fileList[0].CreationTime };

                    if (fileList.Length > 1)
                    {
                        foreach (var t in fileList.Where(t => t.CreationTime > temp[0]))
                        {
                            temp[0] = t.CreationTime;
                            filename = t.Name;
                        }

                        Process.Start(dinfo + "\\" + filename);
                    }
                    else if (fileList.Length == 1)
                    {
                        MessageBox.Show(Resources.Resources.TestFileImport_ShowImportLogs_File_Imported_without_Errors);
                    }
                }
            }
            catch
            {
                MessageBox.Show(Resources.Resources.TestFileImport_ShowImportLogs_Log_folder_didn_t_contains_any_files_);
            }
        }

        private void fileImportLogsButton_Click(object sender, EventArgs e)
        {
            ShowImportLogs();
        }

    }
}
