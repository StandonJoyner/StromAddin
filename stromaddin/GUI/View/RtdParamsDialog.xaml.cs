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
using stromaddin.Config;

namespace stromaddin.GUI.View
{
    public class RtdParamProperty : INotifyPropertyChanged
    {
        private RtdParam _param;
        public event PropertyChangedEventHandler PropertyChanged;
        public RtdParamProperty(RtdParam param)
        {
            _param = param;
        }
        public RtdParam Param 
        {
            get => _param;
        }
        public string Name
        {
            get => _param.Name;
            set
            {
                _param.Name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }
        public string Value
        {
            get => _param.Value;
            set
            {
                _param.Value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Value"));
            }
        }
        public List<RtdChoice> Choices
        {
            get => _param.Choices;
        }
    }
    /// <summary>
    /// RtdParamsDialog.xaml
    /// </summary>
    public partial class RtdParamsDialog : Window
    {
        private ObservableCollection<RtdParamProperty> _rtdParams;
        public RtdParamsDialog(XMLIndicator rtdIndi)
        {
            _rtdParams = new ObservableCollection<RtdParamProperty>();
            foreach (var item in rtdIndi.Params)
                _rtdParams.Add(new RtdParamProperty(item));
            InitializeComponent();
        }

        public List<RtdParam> GetChoosenParams()
        {
            List<RtdParam> parms = new List<RtdParam>();
            foreach (var item in _rtdParams)
                parms.Add(item.Param);
            return parms;
        }
        public ObservableCollection<RtdParamProperty> RtdParams
        {
            get => _rtdParams;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
