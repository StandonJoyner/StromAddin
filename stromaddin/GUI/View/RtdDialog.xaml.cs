﻿using System;
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
        private ObservableCollection<XMLIndicator> _selecteds = new ObservableCollection<XMLIndicator>();

        public RtdDialog()
        {
            InitializeComponent();
            this.Loaded += OnDialogLoaded;
            this.Closed += OnDialogClosed;
        }

        public List<XMLIndicator> Indicators
        {
            get
            {
                return RtdIndicators.Instance.GetIndicators();
            }
        }

        public ObservableCollection<XMLIndicator> Selecteds
        {
            get
            {
                return _selecteds;
            }
        }

        private void OnDialogLoaded(object sender, RoutedEventArgs e)
        {
            hook.HookKeyboard(this);
        }
        private void OnDialogClosed(object sender, EventArgs e)
        {
            hook.UnHookKeyboard();
        }

        private void ListItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var indi = (sender as ListBox).SelectedItem as XMLIndicator;
            if (indi == null)
                return;
            var indiSel = (XMLIndicator)indi.Clone();
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
            int row = _selectedGrid.SelectedIndex;
            if (row == -1)
                return;
            _selecteds.RemoveAt(row);
        }
        private void TextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get the TextBlock
            var textBlock = sender as TextBlock;

            // Get the DataGridRow
            var dataGridRow = FindParent<DataGridRow>(textBlock);

            // Select the row
            dataGridRow.IsSelected = true;
        }
        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            // Get the parent object
            var parentObject = VisualTreeHelper.GetParent(child);

            // If the parent is null, return null
            if (parentObject == null) return null;

            // If the parent is of the correct type, return it
            if (parentObject is T parent) return parent;

            // Otherwise, call this method recursively
            return FindParent<T>(parentObject);
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

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            int row = _selectedGrid.SelectedIndex;
            if (row == -1)
                return;
            var indi = _selecteds[row];
            if (indi == null)
                return;
            if (EditIndicator(indi))
            {
                _selecteds.RemoveAt(row);
                _selecteds.Insert(row, indi);
            }
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
