using System.Runtime.InteropServices;
using System.Text;
using WpfClient.Services.Interfaces;

namespace WpfClient.Services
{
    public class PrinterService : IPrinterService
    {
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetDefaultPrinter(StringBuilder pszBuffer, ref int pcchBuffer);

        public string GetDefaultPrinterName()
        {
            try
            {
                int bufferSize = 256;
                var buffer = new StringBuilder(bufferSize);

                if (GetDefaultPrinter(buffer, ref bufferSize))
                {
                    return buffer.ToString();
                }

                if (Marshal.GetLastWin32Error() == 122) // ERROR_INSUFFICIENT_BUFFER
                {
                    buffer = new StringBuilder(bufferSize);
                    if (GetDefaultPrinter(buffer, ref bufferSize))
                    {
                        return buffer.ToString();
                    }
                }

                return "No default printer found";
            }
            catch (Exception ex)
            {
                // Логирование (предполагается использование ILogger)
                Console.WriteLine($"Error retrieving default printer: {ex.Message}");
                return null;
            }
        }
    }
}
