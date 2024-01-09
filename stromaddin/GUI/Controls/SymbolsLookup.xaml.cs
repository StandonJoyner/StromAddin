using stromaddin.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// SymbolsLookup.xaml 的交互逻辑
    /// </summary>
    public partial class SymbolsLookup : UserControl
    {
        public SymbolsLookup()
        {
            InitializeComponent();
        }

        private ObservableCollection<TickerSymbol> _selecteds = new ObservableCollection<TickerSymbol>();

        public List<TickerSymbol> Symbols
        {
            get
            {
                return SymbolsSet.Symbols;
            }
        }
        public ObservableCollection<TickerSymbol> Selecteds
        {
            get
            {
                return _selecteds;
            }
        }
        public void AddCode_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lv = (sender as ListView);
            var sym = lv.SelectedItem as TickerSymbol;
            _selecteds.Add(sym);
        }

        public void DelCode_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lv = (sender as ListView);
            var sym = lv.SelectedItem as TickerSymbol;
            _selecteds.Remove(sym);
        }
    }
}
