using System;

using System.Windows.Forms;

namespace DigitalMeter
{
    public partial class Form3 : Form
    {
        public Form3( )
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            
            foreach (var formName in ValueList.MoreForms.Keys)
            {
                ComboBoxForms.Items.Add(formName);
            }

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedForm = ComboBoxForms.SelectedItem.ToString();

            if (ValueList.MoreForms.ContainsKey(selectedForm))
            {
                var formToOpen = (Form)Activator.CreateInstance(ValueList.MoreForms[selectedForm]);
                formToOpen.Show(); // Open the form
            }
            else
            {
                MessageBox.Show("Form not found.");
            }
        }
    }
}
