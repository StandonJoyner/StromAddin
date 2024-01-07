using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;
using stromaddin.Core.Excel;
using stromddin.Core;

namespace stromaddin.GUI.View
{
    public class OutputLayoutProperty : INotifyPropertyChanged
    {
        bool _isVertical = true;
        public bool IsVertical {
            get => _isVertical;
            set { 
                _isVertical = value; 
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsVertical")); }
            }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    /// <summary>
    /// InsertCodes.xaml
    /// </summary>
    public partial class IdentifierLookupDialog : Window
    {
        private ObservableCollection<TickerSymbol> _selecteds = new ObservableCollection<TickerSymbol>();
        private OutputLayoutProperty _layout = new OutputLayoutProperty();
        public IdentifierLookupDialog()
        {
            InitializeComponent();
        }

        public bool IsVertical
        {
            get => _layout.IsVertical;
            set => _layout.IsVertical = value;
        }
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

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (_selecteds.Count == 0)
            {
                MessageBox.Show("Please select at least one code.");
                return;
            }
            if (IsVertical)
            {
                TableOutput tb = new TableOutput(_selecteds.Count, 1, headerRows: 0);
                for (int i = 0; i < _selecteds.Count; ++i)
                    tb.SetData(i, 0, _selecteds[i].Symbol);
                tb.Output(null);
            }
            else
            {
                TableOutput tb = new TableOutput(1, _selecteds.Count, headerRows:0);
                for (int i = 0; i < _selecteds.Count; ++i)
                {
                    tb.SetData(0, i, _selecteds[i].Symbol);
                }
                tb.Output(null);
            }
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void AddCode_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lv = (sender as ListView);
            var sym = lv.SelectedItem as TickerSymbol;
            _selecteds.Add(sym);
        }

        private void DelCode_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var lv = (sender as ListView);
            var sym = lv.SelectedItem as TickerSymbol;
            _selecteds.Remove(sym);
        }
    }
}
