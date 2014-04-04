using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MyClassLibrary;
using WFHelper.User_Classes;

namespace WFHelper
{
    public partial class Settings : Form
    {
        private readonly WorkWithRegistry _wwr;

        public Settings()
        {
            InitializeComponent();
            _wwr = new WorkWithRegistry("WF_Helper");

            CreateControlsAndSetValues();
        }

        private void CreateControlsAndSetValues()
        {
            tabPage1.Controls.Clear();

            var i = 0;

            var labelsArray = new Label[_wwr.GetValuesCount()];
            var textBoxesArray = new TextBox[_wwr.GetValuesCount()];
            var buttonsArray = new Button[_wwr.GetValuesCount()];

            foreach (var vars in _wwr.ReadAllValues())
            {
                labelsArray[i] = new Label
                    {
                        Text = vars,
                        Location = new Point(10, (i + 1) * 30 - 10),
                        Width = Width * 2,
                        Name = string.Concat(vars, "_Label")
                    };

                textBoxesArray[i] = new TextBox
                    {
                        Text = _wwr.ReadValue(vars),
                        Location = new Point(200, (i + 1) * 30 - 10),
                        Width = Width / 2,
                        Name = string.Concat(vars, "_TextBox")
                    };

                buttonsArray[i] = new Button
                    {
                        Text = Resources.Resources.Settings_Relocate_Directories,
                        Location = new Point(535, (i + 1) * 30 - 10),
                        Width = Width / 13,
                        Name = string.Concat(vars, "_Button")
                    };

                buttonsArray[i].Click += ChangeFolderButton_Click;

                tabPage1.Controls.Add(buttonsArray[i]);
                tabPage1.Controls.Add(textBoxesArray[i]);
                tabPage1.Controls.Add(labelsArray[i]);

                i++;
            }

        }

        void ChangeFolderButton_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog { RootFolder = Environment.SpecialFolder.MyComputer })
            {
                fbd.ShowDialog();

                if (string.IsNullOrEmpty(fbd.SelectedPath)) return;

                tabPage1.Controls[((Button)sender).Name.Replace("Button", "TextBox")].Text = fbd.SelectedPath;
            }

        }
        
        private void setDefautsButton_Click(object sender, EventArgs e)
        {
            var fl = new FirstLaunch();
            fl.SetDefaultFoldersValues(true);
            CreateControlsAndSetValues();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            foreach (var tb in tabPage1.Controls.Cast<Control>().Where(tb => tb.Name.Contains("TextBox")))
            {
                _wwr.WriteValue(tb.Name.Replace("_TextBox", string.Empty), tb.Text, true);
            }
            Dispose();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }

}

