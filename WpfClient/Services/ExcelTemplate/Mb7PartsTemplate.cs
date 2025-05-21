using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RequestManagement.Common.Utilities;
using WpfClient.Models.ExcelWriterModels;

namespace WpfClient.Services.ExcelTemplate
{
    public class Mb7PartsTemplate : ExcelTemplateWriterBase<ActPartsModel>
    {
        public override ExcelTemplateType TemplateType => ExcelTemplateType.Mb7Parts;
        public override byte[] FillTemplate(ActPartsModel data)
        {
            var startDataRow = 15;
            ExcelPackage.License.SetNonCommercialPersonal("Thron41k");
            using var stream = GetTemplateStream("Mb7PartsTemplate.xlsx");
            using var package = new ExcelPackage(stream);
            var templateSheet = package.Workbook.Worksheets[0];
            templateSheet.Cells[6, 3].Value = data.Commissions?.Name;
            templateSheet.Cells[18, 12].Value = data.Commissions?.ApproveForDefectAndLimit?.Position;
            templateSheet.Cells[18, 17].Value = data.Commissions?.ApproveForDefectAndLimit?.ShortName;
            templateSheet.Cells[18, 3].Value = data.Frp?.Position;
            templateSheet.Cells[18, 7].Value = data.Frp?.ShortName;
            var grouped = data.Expenses
                .Where(e => !string.IsNullOrWhiteSpace(e.Code) && e.Defect.Id == 2 || e.Defect.Id == 3)
                .OrderBy(e=>e.Stock.Nomenclature.Name)
                .GroupBy(e => e.Code!)
                .ToList();
            foreach (var group in grouped)
            {
                var code = group.Key;
                var sheetName = ExcelHelpers.GetSafeSheetName(code);
                var newSheet = package.Workbook.Worksheets.Add(sheetName, templateSheet);
                newSheet.Cells[2, 7].Value = $"ВЕДОМОСТЬ № {code}";
                newSheet.Cells[10, 12].Value = group.First().Date.ToString("dd.MM.yyyy");
                var gn = string.IsNullOrEmpty(group.First().Equipment.StateNumber) ? "" : $"({group.First().Equipment.StateNumber})";
                newSheet.Cells[7, 3].Value = $"{group.First().Equipment.Name}{gn}";
                var startRow = startDataRow;
                foreach (var item in group.OrderBy(x=>x.Stock.Nomenclature.Name))
                {
                    var cc = group.Count();
                    if (cc == 1)
                    {
                        newSheet.DeleteRow(startRow);
                    }
                    newSheet.Cells[startRow, 1].Value = startRow - startDataRow + 1;
                    newSheet.Cells[startRow, 2].Value = item.Driver.FullName;
                    var driverNameHeight = ExcelHelpers.GetRowHeight(newSheet.Column(2).Width+ newSheet.Column(3).Width+newSheet.Column(4).Width, newSheet.Cells[startRow, 2].Style.Font.Name, newSheet.Cells[startRow, 2].Style.Font.Size, item.Driver.FullName);
                    newSheet.Cells[startRow, 6].Value = item.Stock.Nomenclature.Name;
                    var nameHeight = ExcelHelpers.GetRowHeight(newSheet.Column(6).Width+newSheet.Column(7).Width, newSheet.Cells[startRow, 6].Style.Font.Name, newSheet.Cells[startRow, 6].Style.Font.Size, item.Stock.Nomenclature.Name);
                    newSheet.Cells[startRow, 8].Value = item.Stock.Nomenclature.Article;
                    var articleHeight = ExcelHelpers.GetRowHeight(newSheet.Column(8).Width, newSheet.Cells[startRow, 8].Style.Font.Name, newSheet.Cells[startRow, 8].Style.Font.Size, item.Stock.Nomenclature.Article!);
                    newSheet.Row(startRow).Height = new[]{nameHeight, articleHeight, driverNameHeight}.Max();
                    newSheet.Cells[startRow, 11].Value = item.Stock.Nomenclature.UnitOfMeasure;
                    newSheet.Cells[startRow, 13].Value = item.Quantity;
                    newSheet.Cells[startRow, 15].Value = item.Date.ToString("dd.MM.yyyy");
                    newSheet.Cells[startRow, 17].Value = item.DefectId switch
                    {
                        2 => "24 мес.",
                        3 => "12 мес.",
                        _ => string.Empty
                    };
                    startRow++;
                    
                    if (startDataRow + cc - 1 > startRow)
                    {
                        newSheet.Cells[startRow, 1].Insert(eShiftTypeInsert.EntireRow);
                    }
                }
            }
            package.Workbook.Worksheets.Delete(0);
            return package.GetAsByteArray();
        }
    }
}