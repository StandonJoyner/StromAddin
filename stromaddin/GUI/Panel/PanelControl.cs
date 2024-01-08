using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace stromaddin.GUI.Panel
{
    public interface IMyUserControl { }

    /// <summary>
    /// Interaction logic for PanelControl.xaml
    /// </summary>
    [ComDefaultInterface(typeof(IMyUserControl))]
    public partial class PanelControl : UserControl, IMyUserControl
    {
        public PanelControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            ElementHost host  = new ElementHost();
            host.Dock = DockStyle.Fill;

            //var wpfControl = new stromaddin.GUI.View.SymbolsChoose();
            //host.Child = wpfControl;
            //this.Controls.Add(host);
            // Add any initialization after the InitializeComponent() call.
        }
    }
}
