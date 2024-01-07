using ExcelDna.Integration;
using ExcelDna.Integration.CustomUI;
using stromaddin.GUI.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using Microsoft.Office.Interop;
using System.Windows.Forms;
using System.Windows;
using stromaddin.Config;
using Microsoft.Office.Interop.Excel;
using stromaddin.Core.Excel;
using System.IO;
using System.Reflection;
using System.Drawing;
using stdole;
using EnvDTE;
using stromddin.Resources;

namespace stromaddin.GUI.Ribbon
{
    [ComVisible(true)]
    public class RibbonControler : ExcelRibbon
    {
        private CustomTaskPane _myTaskPane;

        public void OnButtonPressed(IRibbonControl control)
        {
            if (_myTaskPane == null)
            {
                _myTaskPane = CustomTaskPaneFactory.CreateCustomTaskPane(typeof(PanelControl), "My Task Pane");
                _myTaskPane.Visible = true;
                //_myTaskPane.Visible = true;
                _myTaskPane.DockPosition = MsoCTPDockPosition.msoCTPDockPositionLeft;
                //_myTaskPane.DockPositionStateChange += ctp_DockPositionStateChange;
                //_myTaskPane.VisibleStateChange += ctp_VisibleStateChange;
            }
            else
            {
                _myTaskPane.Visible = !_myTaskPane.Visible;
            }
            RtdIndicators.Instance.GetIndicators();
        }

        public void OnTestButtonPressed(IRibbonControl control)
        {
            //TableOutput table = new TableOutput(2, 2, true);
            //table.SetData(0, 0, "h1234");
            //table.SetData(0, 1, 123);
            //table.SetHeader(0, "hello");
            //table.SetFormat(0, "yyyy-mm");

            //_Application app = (_Application)ExcelDnaUtil.Application;
            //table.Output(app.ActiveCell);
        }

        public void OnActionButton(IRibbonControl control)
        {
            switch (control.Id)
            {
                case "date_history":
                    break;
                case "real_time":
                    OnRTD();
                    break;
                case "identifier_lookup":
                    OnIdentifierLookup();
                    break;
                case "refresh":
                    break;
            }
        }

        private void OnIdentifierLookup()
        {
            var rtdDlg = new View.IdentifierLookupDialog();
            var win = new WindowInteropHelper(rtdDlg);
            win.Owner = ExcelDnaUtil.WindowHandle;
            rtdDlg.ShowDialog();
        }
        public void OnRTD()
        {
            // show rtd window
            var rtdDlg = new View.RtdDialog();
            var win = new WindowInteropHelper(rtdDlg);
            win.Owner = ExcelDnaUtil.WindowHandle;
            rtdDlg.Show();
        }

        public override string GetCustomUI(string RibbonID)
        {
            return RibbonResources.Ribbon;
        }

        public override object LoadImage(string imageId)
        {
            // This will return the image resource with the name specified in the image='xxxx' tag
            return RibbonResources.ResourceManager.GetObject(imageId);
        }
    }
}
