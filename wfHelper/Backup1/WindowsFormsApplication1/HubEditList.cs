using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WFHelper
{
    public partial class HubEditList : Form
    {
        private List<string> HubList { get; set; }
        private MainForm Form1 { get; set; }

        public HubEditList()
        {
            InitializeComponent();
        }

        public HubEditList(List<string> hublist, MainForm f1)
        {
            InitializeComponent();
            Form1 = f1;

            HubList = hublist;
            checkedListBox1.Items.Clear();
            // ReSharper disable CoVariantArrayConversion
            if (HubList != null) checkedListBox1.Items.AddRange(HubList.Distinct().ToArray());
            // ReSharper restore CoVariantArrayConversion
        }


        private void button1_Click(object sender, EventArgs e)
        {
            DeleteSelectedItems();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateItem();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                textBox1.Text = checkedListBox1.Items[checkedListBox1.SelectedIndex].ToString();
            }
// ReSharper disable EmptyGeneralCatchClause
            catch { }
// ReSharper restore EmptyGeneralCatchClause
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                UpdateItem();
            }
        }


        public void DeleteSelectedItems()
        {
            var atLeastOneItemChecked = false;

            for (var i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if (checkedListBox1.GetItemCheckState(i) != CheckState.Checked) continue;

                checkedListBox1.Items.RemoveAt(i--);
                atLeastOneItemChecked = true;
            }

            if (!atLeastOneItemChecked)
            {
                checkedListBox1.Items.RemoveAt(checkedListBox1.SelectedIndex);
            }

            UpdateList();
        }

        public void UpdateItem()
        {
            checkedListBox1.Items[checkedListBox1.SelectedIndex] = textBox1.Text;
            UpdateList();
        }

        public void UpdateList()
        {
            HubList.Clear();
            HubList.AddRange(checkedListBox1.Items.Cast<String>());

            Form1.RefreshComboBox();
        }





    }
}
