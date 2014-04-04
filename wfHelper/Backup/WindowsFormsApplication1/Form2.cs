using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            FirstLaunch fl = new FirstLaunch();

            richTextBox1.Clear();
            foreach (string read_line in fl.StreamFromFile(comboBox1.Text.Trim()))
            {
                richTextBox1.AppendText(read_line);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FirstLaunch fl = new FirstLaunch();

            richTextBox1.Clear();
            foreach (string read_line in fl.StreamFromFile(comboBox1.Text.Trim()))
            {
                richTextBox1.AppendText(read_line);
            }
        }

    }


}

