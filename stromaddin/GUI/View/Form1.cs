using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace stromaddin.GUI.View
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ElementHost host = new ElementHost();
            host.Dock = DockStyle.Fill;

            var wpfControl = new stromaddin.GUI.View.UserControl1();
            host.Child = wpfControl;
            this.Controls.Add(host);
        }
    }
}
