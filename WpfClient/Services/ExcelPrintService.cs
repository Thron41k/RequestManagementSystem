using System.Runtime.InteropServices;
using RequestManagement.WpfClient.Services.Interfaces;
using Excel = Microsoft.Office.Interop.Excel;

namespace RequestManagement.WpfClient.Services;

public class ExcelPrintService : IExcelPrintService
{
    public void Print(string filePath, string printerName, int copies = 1)
    {
        Excel.Application? excelApp = null;
        Excel.Workbook? workbook = null;

        try
        {
            excelApp = new Excel.Application
            {
                Visible = false,
                DisplayAlerts = false
            };

            workbook = excelApp.Workbooks.Open(filePath, ReadOnly: true);

            for (int i = 0; i < copies; i++)
            {
                workbook.PrintOut(ActivePrinter: printerName);
            }

            workbook.Close(SaveChanges: false);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Ошибка при печати Excel-файла.", ex);
        }
        finally
        {
            if (workbook != null) Marshal.ReleaseComObject(workbook);
            if (excelApp != null)
            {
                excelApp.Quit();
                Marshal.ReleaseComObject(excelApp);
            }
        }
    }
}