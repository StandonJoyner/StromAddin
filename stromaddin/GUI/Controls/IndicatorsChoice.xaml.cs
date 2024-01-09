using stromaddin.Config;
using stromaddin.GUI.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace stromaddin.GUI.Controls
{
    /// <summary>
    /// IndicatorsChoice.xaml 的交互逻辑
    /// </summary>
    public partial class IndicatorsChoice : UserControl
    {
        private ObservableCollection<XMLIndicator> _selecteds = new ObservableCollection<XMLIndicator>();
        public IndicatorsChoice()
        {
            InitializeComponent();
        }

        public List<XMLIndicator> Indicators
        {
            get
            {
                return DataHistoryIndicators.Instance.GetIndicators();
            }
        }

        public ObservableCollection<XMLIndicator> Selecteds
        {
            get
            {
                return _selecteds;
            }
        }

        private bool EditIndicator(XMLIndicator indiSel)
        {
            if (indiSel.Params.Count == 0)
                return false;
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

        private void ListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selIndi = (sender as ListView).SelectedItem as XMLIndicator;
            if (selIndi != null)
            {
                var newIndi = (XMLIndicator)selIndi.Clone();
                if (newIndi.Params.Count > 0)
                {
                    if (EditIndicator(newIndi))
                    {
                        _selecteds.Add(newIndi);
                    }
                }
                else
                {
                    _selecteds.Add(newIndi);
                }
            }
        }

        private void Selecteds_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int row = (sender as ListView).SelectedIndex;
            if (row >= 0)
            {
                _selecteds.RemoveAt(row);
            }
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selIndi = button.DataContext as XMLIndicator;
            if (selIndi != null)
            {
                var newIndi = (XMLIndicator)selIndi.Clone();
                if (EditIndicator(newIndi))
                {
                    var listViewItem = FindParent<ListViewItem>(button);
                    var listView = ItemsControl.ItemsControlFromItemContainer(listViewItem) as ListView;
                    int row = listView.ItemContainerGenerator.IndexFromContainer(listViewItem);
                    _selecteds.RemoveAt(row);
                    _selecteds.Insert(row, newIndi);
                }
            }
        }

        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindParent<T>(parentObject);
            }
        }

        private void Params_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selIndi = (sender as ListBox).DataContext as XMLIndicator;
            if (selIndi == null)
                return;
            int row = _selecteds.IndexOf(selIndi);
            if (row >= 0)
            {
                _selecteds.RemoveAt(row);
            }
        }
    }
}
