using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using stromaddin.Config;
using stromaddin.Core.Excel;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;

namespace stromaddin.GUI.View
{
    /// <summary>
    /// RtdDialog.xaml
    /// </summary>
    public partial class RtdDialog : Window
    {
        private AddinMessageHook hook = new AddinMessageHook();
        private ObservableCollection<RtdIndicator> _selecteds = new ObservableCollection<RtdIndicator>();

        public RtdDialog()
        {
            InitializeComponent();
            this.Loaded += RtdDialog_Loaded;
            this.Closed += RtdDialog_Closed;
        }

        public List<RtdIndicator> Indicators
        {
            get
            {
                return RtdIndicators.Instance.GetIndicators();
            }
        }

        public ObservableCollection<RtdIndicator> Selecteds
        {
            get
            {
                return _selecteds;
            }
        }

        private void RtdDialog_Loaded(object sender, RoutedEventArgs e)
        {
            hook.HookKeyboard(this);
        }
        private void RtdDialog_Closed(object sender, EventArgs e)
        {
            hook.UnHookKeyboard();
        }

        private void ListItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var indi = (sender as ListBox).SelectedItem as RtdIndicator;
            if (indi == null)
                return;
            var indiSel = (RtdIndicator)indi.Clone();
            if (indiSel.Params.Count > 0)
            {
                if (EditIndicator(indiSel))
                    _selecteds.Add(indiSel);
            }
            else
            {
                _selecteds.Add(indiSel);
            }
        }
        private void Selecteds_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var indi = (sender as DataGrid).SelectedItem as RtdIndicator;
            if (indi == null)
                return;
            if (EditIndicator(indi))
            {
                int row = _selectedGrid.SelectedIndex;
                _selecteds.RemoveAt(row);
                _selecteds.Insert(row, indi);
            }
        }
        private bool EditIndicator(RtdIndicator indiSel)
        {
            var dlg = new RtdParamsDialog(indiSel);
            dlg.ShowDialog();
            if (dlg.DialogResult.Value)
            {
                indiSel.Params = dlg.GetChoosenParams();
                return true;
            }
            else
            { 
                return false;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            int row = _selectedGrid.SelectedIndex;
            if (row == -1)
                return;
            _selecteds.RemoveAt(row);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            var sym = _symEditor.Text.Trim();
            if (sym.Length == 0)
            {
                MessageBox.Show("Please enter one symbol!");
            }
            else if (_selecteds.Count == 0)
            {
                MessageBox.Show("Please select at least one indicator!");
            }
            else
            {
                RtdOutput rtdOutput = new RtdOutput(sym, _selecteds);
                rtdOutput.Output();
            }
            Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
