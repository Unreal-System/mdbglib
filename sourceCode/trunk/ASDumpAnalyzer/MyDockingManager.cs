using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Darwen.Windows.Forms.Controls.Docking;


namespace ASDump
{
    public partial class MyDockingManager : Darwen.Windows.Forms.Controls.Docking.DockingManagerControl
    {
        public MyDockingManager()
        {
            InitializeComponent();
            this.Load += new EventHandler(MyDockingManager_Load);
        }

        void MyDockingManager_Load(object sender, EventArgs e)
        {
        }
    }
}
