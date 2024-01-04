using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stromaddin.Core.Excel
{
    internal class NumberFormat
    {
        public static string GetFormat(string type)
        {
            if (type == "Time")
                return GetTimeFormat();
            else if (type == "Percent")
                return GetPercentFormat();
            else if (type == "String")
                return GetStringFormat();
            else if (type == "Number")
                return GetNumberFormat();
            else
                return GetRegularFormat();
        }
        public static string GetDateFormat()
        {
            return "dd/mm/yyyy";
        }
        public static string GetTimeFormat()
        {
            return "h:mm:ss AM/PM";
        }
        public static string GetPercentFormat()
        {
            return "0.00%";
        }
        public static string GetStringFormat()
        {
            return "@";
        }
        public static string GetNumberFormat()
        {
            return "";
        }
        public static string GetRegularFormat()
        {
            return "";
        }
    }
}
