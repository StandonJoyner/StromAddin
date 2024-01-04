using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace stromaddin.GUI.View
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        private const int WM_GETDLGCODE = 0x0087;
        private const int DLGC_WANTALLKEYS = 0x0004;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_CHAR = 0x102;

        public UserControl1()
        {
            InitializeComponent();
        }
    }
}
