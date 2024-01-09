using stromaddin.GUI.Controls;
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
using System.Windows.Shapes;

namespace stromaddin.GUI.View
{
    /// <summary>
    /// DataHistoryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DataHistoryDialog : Window
    {
        private int _currentPageIndex = 0;
        private ContentControl[] _pages;

        AddinMessageHook hook = new AddinMessageHook();
        public DataHistoryDialog()
        {
            InitializeComponent();
            this.Loaded += OnDialogLoaded;
            this.Closed += OnDialogClosed;

            //_pages = new ContentControl[] { new IndicatorsChoice() };
            //_PageContent = new Button(); //_pages[_currentPageIndex];
        }
        private void OnDialogLoaded(object sender, RoutedEventArgs e)
        {
            hook.HookKeyboard(this);
        }
        private void OnDialogClosed(object sender, EventArgs e)
        {
            hook.UnHookKeyboard();
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPageIndex > 0)
            {
                _currentPageIndex--;
                //_PageContent = _pages[_currentPageIndex];
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPageIndex < _pages.Length - 1)
            {
                _currentPageIndex++;
                //_PageContent = _pages[_currentPageIndex];
            }
        }
        private void OK_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
