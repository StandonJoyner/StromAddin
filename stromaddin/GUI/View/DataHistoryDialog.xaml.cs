using stromaddin.Core.Excel;
using stromaddin.GUI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace stromaddin.GUI.View
{
    internal class DataHistoryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private int _currentPageIndex = 0;
        private ContentControl[] _pages;
        private SymbolsLookup _symbolsPage = new SymbolsLookup();
        private IndicatorsChoice _indicsPage = new IndicatorsChoice();
        private DHTimeRange _timePage = new DHTimeRange();
        private DHOutputSetting _outputPage = new DHOutputSetting();
        public DataHistoryViewModel()
        {
            _pages = new ContentControl[] { _symbolsPage, _indicsPage, _timePage, _outputPage };
        }
        public UserControl CurrentPage
        {
            get => _pages[_currentPageIndex] as UserControl;
        }
        public string CurrentTitle
        {
            get
            {
                switch (_currentPageIndex)
                {
                    case 0:
                        return "Data History - Select Symbol";
                    case 1:
                        return "Data History - Select Indicator";
                    case 2:
                        return "Data History - Select Time Period";
                    case 3:
                        return "Data History - Layout Setting";
                    default:
                        return "Data History";
                }
            }
        }
        public void Next()
        {
            if (_currentPageIndex < _pages.Length - 1)
            {
                _currentPageIndex++;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPage"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentTitle"));
            }
        }
        public void Previous()
        {
            if (_currentPageIndex > 0)
            {
                _currentPageIndex--;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPage"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentTitle"));
            }
        }
        public void Output()
        {
            string strStartDate = _timePage.StartDate.ToString("dd/MM/yyyy");
            string strEndDate = _timePage.EndDate.ToString("dd/MM/yyyy");
            if (_timePage.Newest)
                strEndDate = "";
            DataHistoryOutput output = new DataHistoryOutput(
                _symbolsPage.Selecteds, _indicsPage.Selecteds,
                strStartDate, strEndDate);
            output.Output();
        }
    }
    /// <summary>
    /// DataHistoryDialog.xaml 的交互逻辑
    /// </summary>
    public partial class DataHistoryDialog : Window
    {
        private DataHistoryViewModel _viewModel = new DataHistoryViewModel();
        AddinMessageHook hook = new AddinMessageHook();
        public DataHistoryDialog()
        {
            InitializeComponent();
            DataContext = _viewModel;
            Loaded += OnDialogLoaded;
            Closed += OnDialogClosed;
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
            _viewModel.Previous();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Next();
        }
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Output();
            Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
