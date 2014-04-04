using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.ObjectModel;
using WFHelper.User_Classes;


[assembly: CLSCompliant(true)]
namespace WFHelper
{

    public partial class MainForm : Form
    {

        static int _counter;
        List<string> _companyList;
        readonly FirstLaunch _fl;
        private TestFileImport _f4;
        private HubEditList _f3;
        Process _cleanBuildDeployProcess, _tomcatPpocess;
        System.Timers.Timer _myTimer, _myTimer2;
        string _hubNameValue, _subDivisionValue;

        enum BuildType
        {
            WebForms,
            ShippingLabels
        };

        
        

        delegate void UpdateConsoleWindowDelegate(String msg);
        delegate void UpdateProgressDelegate(int value);
        delegate void UpdateLabelDelegate(String value);


        public MainForm()
        {
            InitializeComponent();

            _companyList = new List<string>();

            _fl = new FirstLaunch();
            _fl.CreateAndFillFiles();

        }


        private void CleanBuildDeploy_Click(object sender, EventArgs e)
        {
            CleanBuildDeployMethod();
        }

        public void CleanBuildDeployMethod()
        {
            tomcat_stop();
            _counter = 0;
            _hubNameValue = comboBox1.Text;
            _subDivisionValue = subDivisionComboBox.Text;
            ProcessStart(_hubNameValue, _subDivisionValue);

            _companyList.Add(_hubNameValue);
            comboBox1.Text = string.Empty;
            subDivisionComboBox.Text = string.Empty;
            RefreshComboBox();
        }

        public void ShippingLabelAppletGeneratorMethod()
        {
            tomcat_stop();
            _counter = 0;
            _hubNameValue = comboBox1.Text;
            _companyList.Add(_hubNameValue);


            ProcessStart2(_hubNameValue);

            comboBox1.Text = string.Empty;
            subDivisionComboBox.Text = string.Empty;
            RefreshComboBox();
        }

        private void ProcessStart2(string x)
        {
            ProcessStartInfo[] psiArray =
                                     { 
                                        new ProcessStartInfo(_fl.WorkingDirectory+"WebBuild.bat"),  
                                        new ProcessStartInfo(_fl.WorkingDirectory+"WebDeploy.bat"), 
                                        new ProcessStartInfo(_fl.WorkingDirectory+"CopyJarFiles.bat")                                 
                                     };

            if ( _counter < 3 )
            {
                if ( _counter == 0 )
                {
                    _fl.StreamToFile("WebBuild", "webbuild -Dhub=" + x.Trim() + " docViews >  " + _fl.LogsDirectory + "webbuild.log");
                }
                if ( _counter == 2 )
                {
                    _fl.StreamToFile("CopyJarFiles", "copy C:\\tomcat\\webapps\\wfds\\xtencils\\" + x + "\\*.* C:\\tomcat\\webapps\\webec\\implementation\\" + x + "\\");
                }


                psiArray[_counter].UseShellExecute = false;
                psiArray[_counter].CreateNoWindow = true;

                _cleanBuildDeployProcess = new Process {StartInfo = psiArray[_counter]};
                _cleanBuildDeployProcess.Start();


                _counter++;

                _myTimer2 = new System.Timers.Timer();
                _myTimer2.Elapsed += myTimer2_Elapsed;
                _myTimer2.Interval = 1000;
                _myTimer2.Start();

                int leftBound = _cleanBuildDeployProcess.StartInfo.FileName.LastIndexOf("\\", StringComparison.CurrentCulture);
                int rigthBound = _cleanBuildDeployProcess.StartInfo.FileName.LastIndexOf(".bat", StringComparison.CurrentCulture);

                progressBar1.Invoke(new UpdateProgressDelegate(UpdateProgressSafe), _counter);
                progressBar1.Invoke(new UpdateLabelDelegate(UpdateLabelSafe), _cleanBuildDeployProcess.StartInfo.FileName.Substring(leftBound + 1, rigthBound - leftBound));


            }
        }

        void myTimer2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_cleanBuildDeployProcess.HasExited) return;
           
            _myTimer2.Stop();

            if (_counter < 3)
            {
                ProcessStart2(_hubNameValue);
            }
            else
            {
                progressBar1.Invoke(new UpdateLabelDelegate(UpdateLabelSafe), IsBuildOk(BuildType.ShippingLabels));
                tomcat_start();
            }
        }

        public void RefreshComboBox()
        {
            comboBox1.Items.Clear();
// ReSharper disable CoVariantArrayConversion
            if (_companyList != null) comboBox1.Items.AddRange(_companyList.Distinct().ToArray());
// ReSharper restore CoVariantArrayConversion
        }


        private void comboBox1_TextChanged_1(object sender, EventArgs e)
        {
            if ( comboBox1.Text.Trim().Length > 0 )
            {
                CleanBuildDeploy.Enabled = true;
                LabelAppletBuilder.Enabled = true;
                SubDivisionComboBoxFill();
            }
            else
            {
                CleanBuildDeploy.Enabled = false;
                LabelAppletBuilder.Enabled = false;
                SubDivisionComboBoxFill();
            }

        }

        private void SubDivisionComboBoxFill()
        {
            var folders = new List<string>();


            try
            {
                subDivisionComboBox.Items.Clear();
                folders.AddRange(Directory.GetDirectories("C:\\jv\\implementation\\maps\\" + comboBox1.Text.Substring(0, 1) + "\\" + comboBox1.Text + "\\web\\"));
                folders.Remove("C:\\jv\\implementation\\maps\\" + comboBox1.Text.Substring(0, 1) + "\\" + comboBox1.Text + "\\web\\.svn");

                if ( folders.Count > 0 )
                {
                    foreach ( string item in folders )
                    {
                        subDivisionComboBox.Items.Add(item.Substring(item.LastIndexOf("\\", StringComparison.CurrentCulture) + 1));
                    }
                }
            }
            catch
            {
                subDivisionComboBox.Items.Clear();
            }

        }


        private void TestFileImport_Click(object sender, EventArgs e)  // Test File Import
        {
            var readOnlyCompanyList = new ReadOnlyCollection<string>(_companyList);

            _f4 = new TestFileImport(readOnlyCompanyList);
            _f4.Show();    
        }

        private void ProcessStart(string x, string y)
        {
            ProcessStartInfo[] psiArray =
                                     { 
                                        new ProcessStartInfo(_fl.WorkingDirectory+"Clean.bat"),  
                                        new ProcessStartInfo(_fl.WorkingDirectory+"Build.bat"), 
                                        new ProcessStartInfo(_fl.WorkingDirectory+"Deploy.bat")                                 
                                     };

            if ( _counter < 3 )
            {
                if ( _counter == 1 )
                {
                    if ( string.IsNullOrEmpty(y.Trim()) )
                    {
                        _fl.StreamToFile("Build", "impbuild -Dhub=" + x.Trim() + " WebForms >  " + _fl.LogsDirectory + "build.log");
                    }
                    else
                    {
                        _fl.StreamToFile("Build", "impbuild -Dhub=" + x.Trim() + " " + "-Ddivision=" + y.Trim() + " WebForms >  " + _fl.LogsDirectory + "build.log");
                    }
                }


                psiArray[_counter].UseShellExecute = false;
                psiArray[_counter].CreateNoWindow = true;

                _cleanBuildDeployProcess = new Process {StartInfo = psiArray[_counter]};
                _cleanBuildDeployProcess.Start();


                _counter++;

                _myTimer = new System.Timers.Timer();
                _myTimer.Elapsed += myTimer_Elapsed;
                _myTimer.Interval = 1000;
                _myTimer.Start();

                var leftBound = _cleanBuildDeployProcess.StartInfo.FileName.LastIndexOf("\\", StringComparison.CurrentCulture);
                var rigthBound = _cleanBuildDeployProcess.StartInfo.FileName.LastIndexOf(".bat", StringComparison.CurrentCulture);

                progressBar1.Invoke(new UpdateProgressDelegate(UpdateProgressSafe), _counter);
                progressBar1.Invoke(new UpdateLabelDelegate(UpdateLabelSafe), _cleanBuildDeployProcess.StartInfo.FileName.Substring(leftBound + 1, rigthBound - leftBound));


            }
        }


        void UpdateProgressSafe(int value)
        {
            progressBar1.Value = value;
        }

        void UpdateLabelSafe(string value)
        {
            label2.Text = value;
        }


        private void myTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_cleanBuildDeployProcess.HasExited) return;
           
            _myTimer.Stop();
            
            if ( _counter < 3 )
            {
                ProcessStart(_hubNameValue, _subDivisionValue);
            }
            else
            {
                progressBar1.Invoke(new UpdateLabelDelegate(UpdateLabelSafe), IsBuildOk(BuildType.WebForms));
                tomcat_start();
            }
        }

        private string IsBuildOk(Enum e)
        {
            try
            {
                string fileContent;
                if ( e.Equals(BuildType.WebForms) )
                {
                    fileContent = File.ReadAllText(_fl.WorkingDirectory + "Logs\\build.log");
                }
                else if ( e.Equals(BuildType.ShippingLabels) )
                {
                    fileContent = File.ReadAllText(_fl.WorkingDirectory + "Logs\\webbuild.log");
                }
                else fileContent = null;

                if ( fileContent != null && fileContent.Contains("BUILD SUCCESSFUL") )
                {
                    return "Complete";
                }

                return "Complete with Errors";
            }
            catch
            {
                return "Log file is not exists";
            }

        }

        private void tomcat_restart()
        {
            tomcat_stop();
            tomcat_start();
        }

        private void tomcat_stop()
        {
            try
            {
                var ps1 = Process.GetProcessesByName("Tomcat6");

                if ( ps1.Length > 0 )
                {
                    foreach ( var p1 in ps1 )
                    {
                        p1.Kill();
                    }
                }

                TomcatStatusPanel.BackgroundImage = Resources.Resources.RedLabel;
            }

            catch { MessageBox.Show(Resources.Resources.MainForm_tomcat_stop_Having_problems_with_stopping_Tomcat); }
        }

        private void tomcat_start()
        {
            _tomcatPpocess = new Process
                {
                    StartInfo =
                        {
                            FileName = "C:\\tomcat\\bin\\Tomcat6",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                };

            _tomcatPpocess.OutputDataReceived += TomcatP_OutputDataReceived;


            try
            {
                _tomcatPpocess.Start();
                _tomcatPpocess.BeginOutputReadLine();
                TomcatStatusPanel.BackgroundImage = Resources.Resources.GreenLabel;
            }

            catch
            {
                MessageBox.Show(Resources.Resources.MainForm_tomcat_start_Having_problems_with_starting_Tomcat);
                TomcatStatusPanel.BackgroundImage = Resources.Resources.RedLabel;
            }
        }


        void TomcatP_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if ( !String.IsNullOrEmpty(e.Data) )
            {
                UpdateConsoleWindow(e.Data + "\r\n");
            }
        }


        private void UpdateConsoleWindow(String message)
        {


            if ( richTextBox1.InvokeRequired )
            {
                var update = new UpdateConsoleWindowDelegate(UpdateConsoleWindow);
                richTextBox1.Invoke(update, message);
            }
            else
            {
                richTextBox1.AppendText(message);

            }
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Serialize();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Deserialize();
// ReSharper disable CoVariantArrayConversion
            if (_companyList != null) comboBox1.Items.AddRange(_companyList.Distinct().ToArray());
// ReSharper restore CoVariantArrayConversion
        }

        private void Serialize()
        {
            using ( var fs = new FileStream(_fl.WorkingDirectory + "file.s", FileMode.Create, FileAccess.Write, FileShare.ReadWrite) )
            {
                var bf = new BinaryFormatter();

                bf.Serialize(fs, _companyList);                
            }

        }

        private void Deserialize()
        {
            try
            {
                using ( var fs = new FileStream(_fl.WorkingDirectory + "file.s", FileMode.Open, FileAccess.Read, FileShare.Read) )
                {
                    var bf = new BinaryFormatter();

                    _companyList = ( List<string> )bf.Deserialize(fs);
                }

            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            _f3 = new HubEditList(_companyList, this);

            _f3.Show();
        }

        private void label2_TextChanged(object sender, EventArgs e)
        {
            label2.ForeColor = label2.Text.Contains("Errors") ? Color.Red : Color.Black;
        }


        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if ( e.KeyData == Keys.Enter && !String.IsNullOrEmpty(comboBox1.Text.Trim()) )
            {
                CleanBuildDeploy.Focus();
            }
        }

        private void subDivisionComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if ( e.KeyData == Keys.Enter && !String.IsNullOrEmpty(comboBox1.Text.Trim()) )
            {
                CleanBuildDeploy.Focus();
            }
        }



        private void TomcatStartButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            tomcat_restart();
        }

        private void TomcatStopButton_Click(object sender, EventArgs e)
        {
            tomcat_stop();
        }

        private void TomcatLogsButton_Click(object sender, EventArgs e)
        {
            Width = Width != 986 ? 986 : 373;
        }

        private void label1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
            comboBox1.Focus();
        }

        private void LabelAppletBuilder_Click(object sender, EventArgs e)
        {
            ShippingLabelAppletGeneratorMethod();
        }

        private void OpenJarsFolders_Click(object sender, EventArgs e)
        {
            if (wfdsCheckBox.Checked)
            {

                if (Directory.Exists("C:\\tomcat\\webapps\\wfds\\xtencils\\" + _companyList.Last()))
                {
                    Process.Start("explorer", "C:\\tomcat\\webapps\\wfds\\xtencils\\" + _companyList.Last());
                }
                else
                {
                    Process.Start("explorer", "C:\\tomcat\\webapps\\wfds\\xtencils\\");
                }

            }

            if (webecCheckBox.Checked)
            {

                if (Directory.Exists("C:\\tomcat\\webapps\\webec\\implementation\\" + _companyList.Last()))
                {
                      Process.Start("explorer", "C:\\tomcat\\webapps\\webec\\implementation\\" + _companyList.Last());                
                }
                else
                {
                    Process.Start("explorer", "C:\\tomcat\\webapps\\webec\\implementation\\");
                }
            }

        }



    }
}
