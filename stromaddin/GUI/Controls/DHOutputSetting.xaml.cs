using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace stromaddin.GUI.Controls
{
    /// <summary>
    /// DHOutputSetting.xaml 的交互逻辑
    /// </summary>
    public partial class DHOutputSetting : UserControl
    {
        bool _isUnion = true;
        bool _groupBySymbol = true;
        public DHOutputSetting()
        {
            InitializeComponent();
        }
        public bool IsUnion
        {
            get => _isUnion;
            set => _isUnion = value;
        }
        public bool GroupBySymbol
        {
            get => _groupBySymbol;
            set => _groupBySymbol = value;
        }
        public bool GroupByIndicator
        {
            get => !_groupBySymbol;
            set => _groupBySymbol = !value;
        }
    }
}
