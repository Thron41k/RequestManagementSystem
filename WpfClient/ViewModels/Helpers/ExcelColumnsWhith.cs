using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace RequestManagement.WpfClient.ViewModels.Helpers
{
    //Column(5).Width +
    public static class ExcelColumnsWidth
    {
        public static double GetColumnsWidth(ExcelWorksheet sheet, int start, int end)
        {
            var width = 0.0;
            for (var i = start; i <= end; i++)
            {
                width += sheet.Column(i).Width;
            }
            return width;
        }
    }
}
