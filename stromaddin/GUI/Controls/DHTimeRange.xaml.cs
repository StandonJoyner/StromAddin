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
    /// DHTimeRange.xaml 的交互逻辑
    /// </summary>
    public partial class DHTimeRange : UserControl
    {
        private DateTime _startDate = DateTime.Today.AddMonths(-1);
        private DateTime _endDate = DateTime.Today;
        private bool _isNewest = true;
        public DHTimeRange()
        {
            InitializeComponent();
        }

        public DateTime StartDate { 
            get => _startDate; 
            set => _startDate = value; 
        }
        public DateTime EndDate
        {
            get => _endDate; 
            set => _endDate = value; 
        }
        public bool Newest
        {
            get => _isNewest;
            set => _isNewest = value;
        }
    }
}
