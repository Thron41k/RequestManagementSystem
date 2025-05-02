using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfClient.Services.Interfaces
{
    public interface IExcelPrintService
    {
        void Print(string filePath, string printerName, int copies = 1);
    }
}
