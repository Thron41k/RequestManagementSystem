using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RequestManagement.Common.Utilities;
using WpfClient.Models.ExcelWriterModels;

namespace WpfClient.Services.ExcelTemplate
{
    public class LimitPartsTemplate : ExcelTemplateWriterBase<ActPartsModel>
    {
        public override ExcelTemplateType TemplateType => ExcelTemplateType.LimitParts;
        public override byte[] FillTemplate(ActPartsModel data)
        {
            var startDataRow = 17;
            ExcelPackage.License.SetNonCommercialPersonal("Thron41k");
            using var stream = GetTemplateStream("LimitPartsTemplate.xlsx");
            using var package = new ExcelPackage(stream);
            var templateSheet = package.Workbook.Worksheets[0];
            templateSheet.Cells[3, 1].Value = data.Commissions?.Name;
            templateSheet.Cells[3, 1, 3, 4].Merge = true;
            templateSheet.Cells[3, 1, 3, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            templateSheet.Cells[4, 9].Value = data.Commissions?.ApproveForDefectAndLimit?.Position;
            templateSheet.Cells[3, 1, 3, 4].Merge = true;
            templateSheet.Cells[4, 9, 4, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            templateSheet.Cells[6, 9].Value = data.Commissions?.ApproveForDefectAndLimit?.ShortName;
            templateSheet.Cells[6, 9, 6, 10].Merge = true;
            templateSheet.Cells[6, 9, 6, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            templateSheet.Cells[12, 3].Value = $"за период c {data.StartDate:dd.MM.yyyy} по {data.EndDate:dd.MM.yyyy}";
            templateSheet.Cells[20, 8].Value = data.Frp?.ShortName;
            var grouped = data.Expenses
                .Where(e => !string.IsNullOrWhiteSpace(e.Code) && e.Defect.Id != 10 && e.Defect.Id != 11)
                .GroupBy(e => e.Code!)
                .ToList();
            foreach (var group in grouped)
            {
                var code = group.Key;
                var sheetName = ExcelHelpers.GetSafeSheetName(code);
                var newSheet = package.Workbook.Worksheets.Add(sheetName, templateSheet);
                newSheet.Cells[11, 9].Value = code;
                newSheet.Cells[11, 10].Value = group.First().Date.ToString("dd.MM.yyyy");
                newSheet.Cells[14, 3].Value = $"{group.First().Equipment.Name}({group.First().Equipment.StateNumber})";
                var startRow = startDataRow;
                foreach (var item in group.OrderBy(x=>x.Stock.Nomenclature.Name))
                {
                    var cc = group.Count();
                    if (cc == 1)
                    {
                        newSheet.DeleteRow(startRow);
                    }
                    ;
                    newSheet.Cells[startRow, 1].Value = startRow - startDataRow + 1;
                    newSheet.Cells[startRow, 2].Value = item.Stock.Nomenclature.Name;
                    var nameHeight = ExcelHelpers.GetRowHeight(newSheet.Column(2).Width, newSheet.Cells[startRow, 2].Style.Font.Name, newSheet.Cells[startRow, 2].Style.Font.Size, item.Stock.Nomenclature.Name);
                    newSheet.Cells[startRow, 3].Value = item.Stock.Nomenclature.Article;
                    var articleHeight = ExcelHelpers.GetRowHeight(newSheet.Column(3).Width, newSheet.Cells[startRow, 3].Style.Font.Name, newSheet.Cells[startRow, 3].Style.Font.Size, item.Stock.Nomenclature.Article);
                    newSheet.Cells[startRow, 4].Value = item.Defect.Name;
                    var defectHeight = ExcelHelpers.GetRowHeight(newSheet.Column(4).Width, newSheet.Cells[startRow, 4].Style.Font.Name, newSheet.Cells[startRow, 4].Style.Font.Size, item.Defect.Name);
                    newSheet.Row(startRow).Height = new[]{nameHeight, articleHeight, defectHeight}.Max();
                    newSheet.Cells[startRow, 6].Value = item.Stock.Nomenclature.UnitOfMeasure;
                    newSheet.Cells[startRow, 7].Value = item.Quantity;
                    newSheet.Cells[startRow, 8].Value = item.Quantity;
                    newSheet.Cells[startRow, 9].Value = item.Driver.ShortName;
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