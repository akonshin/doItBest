using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    class FirstLaunch
    {

        private string workingDirectory;
        public string WorkingDirectory
        {
            get { return workingDirectory; }
            set { workingDirectory = value; }
        }

        private string logsDirectory;
        public string LogsDirectory
        {
            get { return logsDirectory; }
            set { logsDirectory = value; }
        }

        private string fileImportLogsDirectory;
        public string FileImportLogsDirectory
        {
            get { return fileImportLogsDirectory; }
            set { fileImportLogsDirectory = value; }
        }


        Dictionary<string, string> filesList = new Dictionary<string, string>();
        public Dictionary<string, string> FilesList
        {
            get { return filesList; }
            set { filesList = value; }
        }


        public FirstLaunch()
        {
            WorkingDirectory = Application.StartupPath + "\\system_files\\";
            LogsDirectory = WorkingDirectory + "Logs\\";
                      
            filesList.Add("Clean", "impbuild clean > " + LogsDirectory + "clean.log");
            filesList.Add("Build", "impbuild -Dhub=<HUB> WebForms > " + LogsDirectory + "build.log");
            filesList.Add("Deploy", "impdeploy all  > " + LogsDirectory + "deploy.log");
            filesList.Add("Run", "cd C:\\jv\\src\\sps\\engine-dist\\ && run.bat sps.webec.engine.Engine prof clean -r > " + LogsDirectory + "run.log");
        }

        public void CreateAndFillFiles()
        {
            if (Directory.Exists(WorkingDirectory))
            {
            }
            else
            {
                Directory.CreateDirectory(WorkingDirectory);
                Directory.CreateDirectory(LogsDirectory);
                
                foreach (KeyValuePair<string, string> kvp in filesList)
                {
                    StreamToFile(kvp);                    
                }
            }
        }

        public void StreamToFile(string FileNameWithoutExtension, string FileContent)
        {
            FileStream fs = new FileStream(workingDirectory + FileNameWithoutExtension + ".bat", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(FileContent);
            sw.Close();
            fs.Close();
        }
        public void StreamToFile(KeyValuePair<string, string> kvp)
        {
            FileStream fs = new FileStream(workingDirectory + kvp.Key + ".bat", FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(kvp.Value);
            sw.Close();
            fs.Close();
        }

        public List<string> StreamFromFile(string FileNameWithoutExtension)
        {
            FileStream fs = new FileStream(WorkingDirectory + FileNameWithoutExtension + ".bat", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs);

            string line;
            List<string> allText = new List<string>();

            while ((line = sr.ReadLine()) != null)
            {
                allText.Add(line);
            }

            sr.Close();
            fs.Close();

            return allText;
        }
        

    }
}
