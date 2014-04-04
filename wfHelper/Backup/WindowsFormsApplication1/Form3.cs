using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form3 : Form
    {
        private List<string> HubList{get;set;}
        private Form1 form1{get;set;}

        public Form3()
        {
            InitializeComponent();
        }
        public Form3(List<string> hublist, Form1 f1)
        {
            InitializeComponent();
            form1 = f1;

            HubList = hublist;
            checkedListBox1.Items.Clear();
            checkedListBox1.Items.AddRange(HubList.Distinct().ToArray());
            
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemCheckState(i) == CheckState.Checked)
                {
                    checkedListBox1.Items.RemoveAt(i--);
                }

            }

            HubList.Clear();
            HubList.AddRange(checkedListBox1.Items.Cast<String>());

            form1.RefreshComboBox();
        }
                
    }
}
