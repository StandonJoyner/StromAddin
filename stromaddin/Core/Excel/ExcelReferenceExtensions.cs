using ExcelDna.Integration;
using System;
using static ExcelDna.Integration.XlCall;

public static class ExcelReferenceExtensions
{
    public static string GetSheetName(ExcelReference excelReference)
    {
        object sheetName;
        XlReturn xlReturn = XlCall.TryExcel(XlCall.xlSheetNm, out sheetName, excelReference);

        if (xlReturn == XlReturn.XlReturnSuccess)
        {
            return sheetName.ToString();
        }
        else
        {
            // Handle error
            return null;
        }
    }
    public static string ConvertToA1Style(this ExcelReference excelReference)
    {
        if (excelReference == null)
        {
            throw new ArgumentNullException(nameof(excelReference));
        }
        string sheetName = GetSheetName(excelReference);
        int rowFirst = excelReference.RowFirst + 1; // Excel is 1-indexed
        int columnFirst = excelReference.ColumnFirst + 1; // Excel is 1-indexed

        string columnFirstLetter = GetExcelColumnName(columnFirst);
        string a1Reference = $"{sheetName}!{columnFirstLetter}{rowFirst}";

        if (excelReference.RowFirst != excelReference.RowLast || excelReference.ColumnFirst != excelReference.ColumnLast)
        {
            int rowLast = excelReference.RowLast + 1; // Excel is 1-indexed
            int columnLast = excelReference.ColumnLast + 1; // Excel is 1-indexed

            string columnLastLetter = GetExcelColumnName(columnLast);
            a1Reference += $":{columnLastLetter}{rowLast}";
        }

        return a1Reference;
    }

    private static string GetExcelColumnName(int columnNumber)
    {
        int dividend = columnNumber;
        string columnName = String.Empty;

        while (dividend > 0)
        {
            int modulo = (dividend - 1) % 26;
            columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
            dividend = (dividend - modulo) / 26;
        }

        return columnName;
    }
}
