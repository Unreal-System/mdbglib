using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WizardBase;

namespace ASDumpAnalyzer
{
    public partial class WizForm : Form
    {
        public WizForm()
        {
            InitializeComponent();
        }

        private void wizardControl1_CancelButtonClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void wizardControl1_NextButtonClick(object sender, GenericCancelEventArgs<WizardControl> args)
        {
            MessageBox.Show("next");
        }

        private void wizardControl1_NextButtonClick(object sender)
        {

        }
    }
}
