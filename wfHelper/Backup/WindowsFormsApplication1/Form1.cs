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
    [Serializable]
    public partial class Form1 : Form
    {

        static int counter = 0;

        List<string> companyList;
        FirstLaunch fl;
        Process p;
        System.Timers.Timer myTimer;

        string x;

        ProcessStartInfo[] psiArray = { 
                                        new ProcessStartInfo("clean.bat"), 
                                        new ProcessStartInfo("build.bat"), 
                                        new ProcessStartInfo("deploy.bat"),
                                        new ProcessStartInfo("run.bat")
                                      };


        // ----------------------  Конструктор -------------------------------- //

        public Form1()
        {
            InitializeComponent();


            companyList = new List<string>();

            fl = new FirstLaunch();
            fl.CreateAndFillFiles();

            SetWorkingDirectory();


        }



        // ----------------------  Обработчики событий -------------------------------- //


        private void button1_Click_1(object sender, EventArgs e)
        {
            tomcat_restart();

            counter = 0;
            x = comboBox1.Text;
            processStart(x);

            companyList.Add(x);
            comboBox1.Text = string.Empty;
            RefreshComboBox();
        }

        public void RefreshComboBox()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(companyList.Distinct().ToArray());
        }        

        // ------------------ Проверка на наличие текста в ComboBox ------------- //

        private void comboBox1_TextChanged_1(object sender, EventArgs e)
        {
            if (comboBox1.Text.Trim().Length > 0)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;

            }
        }


        // ----------------------  Кастомные методы -------------------------------- //

        private void SetWorkingDirectory()
        {
            foreach (ProcessStartInfo temp in psiArray)
            {
                temp.WorkingDirectory = fl.WorkingDirectory;
            }
        }

        // ------------------ Запуск процессов ------------- //

        private void button2_Click_1(object sender, EventArgs e)  // Test File Import
        {
            Process.Start(psiArray[3]);
        }

        private void processStart(string x)
        {
            if (counter < 3)
            {
                if (counter == 1)
                {
                    fl.StreamToFile("Build", "impbuild -Dhub=" + x.Trim() + " WebForms >  " + fl.LogsDirectory + "build.log");
                }
                p = Process.Start(psiArray[counter]);
                counter++;

                myTimer = new System.Timers.Timer();
                myTimer.Elapsed += new System.Timers.ElapsedEventHandler(myTimer_Elapsed);
                myTimer.Interval = 1000;
                myTimer.Start();
            }
        }

        private void myTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (p.HasExited)
            {
                myTimer.Stop();
                if (counter < 3)
                {
                    processStart(x);
                }
                else
                {
                    MessageBox.Show("  -  Complete  -  ");
                }

            }

        }

        // ------------------ TomCAt restart ------------- //

        private void tomcat_restart()
        {
            Process[] ps1 = System.Diagnostics.Process.GetProcessesByName("Tomcat6");
            if (ps1.Length > 0)
            {
                foreach (Process p1 in ps1)
                {
                    p1.Kill();
                }

                Thread.Sleep(3000);
                Process.Start("C:\\tomcat\\bin\\Tomcat6");
                Thread.Sleep(7000);
            }
            else
            {
                Process.Start("C:\\tomcat\\bin\\Tomcat6");
                Thread.Sleep(7000);
            }
        }


        // ----------------------  Методы формы  -------------------------------- //

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Serialize();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Deserialize();
            comboBox1.Items.AddRange(companyList.Distinct().ToArray());
        }


        // ----------------------  Сериализация/Десериализация  -------------------------------- //


        private void Serialize()
        {
            FileStream fs = new FileStream(fl.WorkingDirectory + "file.s", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            BinaryFormatter bf = new BinaryFormatter();

            bf.Serialize(fs, companyList);
            fs.Close();
        }

        private void Deserialize()
        {
            try
            {
                FileStream fs = new FileStream(fl.WorkingDirectory + "file.s", FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryFormatter bf = new BinaryFormatter();

                companyList = (List<string>)bf.Deserialize(fs);
                fs.Close();
            }
            catch
            {

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string s = "C:\\jv\\src\\sps\\engine-dist\\logs\\";

            try
            {
                string[] d = Directory.GetDirectories(s);

                for (int i = 0; i < d.Length; i++)
                {
                    DirectoryInfo dinfo = new DirectoryInfo(d[i]);

                    if (dinfo.CreationTime.ToShortDateString() == System.DateTime.Now.ToShortDateString())
                    {
                        FileInfo[] fi = dinfo.GetFiles();

                        string filename = fi[0].Name;
                        DateTime temp = fi[0].CreationTime;                       

                        if (fi.Length > 1)
                        {
                            for (int j = 0; j < fi.Length; j++)
                            {
                                if (fi[j].CreationTime > temp)
                                {
                                    temp = fi[j].CreationTime;
                                    filename = fi[j].Name;
                                }
                            }

                            Process.Start(dinfo + "\\" + filename);
                        }
                        else if (fi.Length == 1)
                        {
                            MessageBox.Show("File Imported without Errors");
                        }

                        
                    }

                }


            }
            catch
            { 
                MessageBox.Show("Log folder didn't contains any files "); 
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form3 f3 = new Form3(companyList,this);
            f3.Show();
        }


    }
}
