using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MyClassLibrary;

namespace WFHelper.User_Classes
{
    class FirstLaunch
    {
        public string WorkingDirectory { get; set; }
        public string LogsDirectory { get; set; }
        public Dictionary<string, string> FilesList { get; set; }



        public FirstLaunch()
        {
            SetDefaultFoldersValues();

            FilesList = new Dictionary<string, string>();

            WorkingDirectory = Application.StartupPath + "\\system_files\\";
            LogsDirectory = WorkingDirectory + "Logs\\";

            FilesList.Add("Clean", "impbuild clean > " + LogsDirectory + "clean.log");
            FilesList.Add("Build", "impbuild -Dhub=<HUB> WebForms > " + LogsDirectory + "build.log");
            FilesList.Add("Deploy", "impdeploy all  > " + LogsDirectory + "deploy.log");
            FilesList.Add("Run", "cd C:\\jv\\src\\sps\\engine-dist\\ && run.bat sps.webec.engine.Engine prof clean -r > " + LogsDirectory + "run.log");

            FilesList.Add("WebBuild", "webbuild -Dhub=<HUB> docViews > " + LogsDirectory + "webbuild.log");
            FilesList.Add("WebDeploy", "webdeploy all > " + LogsDirectory + "webdeploy.log");
            FilesList.Add("CopyJarFiles", "copy C:\\tomcat\\webapps\\wfds\\xtencils\\<HUB>\\*.* C:\\tomcat\\webapps\\webec\\implementation\\<HUB>\\");
        }

        public void SetDefaultFoldersValues(bool overrideValues = false)
        {
            var wwr = new WorkWithRegistry("WF_Helper");

            wwr.WriteValue("WorkingDirectory", Application.StartupPath + "\\system_files\\", overrideValues);
            wwr.WriteValue("LogsDirectory", Application.StartupPath + "\\system_files\\" + "Logs\\", overrideValues);
            wwr.WriteValue("TomcatDirectory", "C:\\tomcat\\", overrideValues);
            wwr.WriteValue("TomcatXtencilsDirectory", "C:\\tomcat\\webapps\\wfds\\xtencils\\", overrideValues);
            wwr.WriteValue("TomcatImplementationDirectory", "C:\\tomcat\\webapps\\webec\\implementation\\", overrideValues);
            wwr.WriteValue("TestFileImportDirectory", @"C:\jv\src\sps\engine-dist\inbound\", overrideValues);
            wwr.WriteValue("TestFilesDirectory", @"C:\testFiles\", overrideValues);
            wwr.WriteValue("FileImportLogDirectory", "C:\\jv\\src\\sps\\engine-dist\\logs\\", overrideValues);
            wwr.WriteValue("MapsDirectory", "C:\\jv\\implementation\\maps\\", overrideValues);
        }

        public void CreateAndFillFiles()
        {
            if (!Directory.Exists(WorkingDirectory))
            {
                Directory.CreateDirectory(WorkingDirectory);
                Directory.CreateDirectory(LogsDirectory);
            }

            foreach (var kvp in FilesList)
            {
                StreamToFile(kvp);
            }
        }

        public void StreamToFile(string fileNameWithoutExtension, string fileContent)
        {
            using (var fs = new FileStream(WorkingDirectory + fileNameWithoutExtension + ".bat", FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                var sw = new StreamWriter(fs);
                sw.WriteLine(fileContent);
                sw.Dispose();
            }

        }

        public void StreamToFile(KeyValuePair<string, string> kvp)
        {
            using (var fs = new FileStream(WorkingDirectory + kvp.Key + ".bat", FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                var sw = new StreamWriter(fs);
                sw.WriteLine(kvp.Value);
                sw.Dispose();
            }

        }

        public List<string> StreamFromFile(string fileNameWithoutExtension)
        {
            var allText = new List<string>();

            using (var fs = new FileStream(WorkingDirectory + fileNameWithoutExtension + ".bat", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var sr = new StreamReader(fs);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    allText.Add(line);
                }

                sr.Dispose();
            }

            return allText;
        }



    }

}




