namespace RequestManagement.WpfClient.Services.Interfaces;

public interface IExcelPrintService
{
    void Print(string filePath, string printerName, int copies = 1);
}