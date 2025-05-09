using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RequestManagement.Common.Utilities;
using WpfClient.Models.ExcelWriterModels;

namespace WpfClient.Services.ExcelTemplate
{
    public class ActPartsTemplate : ExcelTemplateWriterBase<ActPartsModel>
    {
        public override ExcelTemplateType TemplateType => ExcelTemplateType.ActParts;
        public override byte[] FillTemplate(ActPartsModel data)
        {
            var startDataRow = 21;
            ExcelPackage.License.SetNonCommercialPersonal("Thron41k");
            using var stream = GetTemplateStream("ActPartsTemplate.xlsx");
            using var package = new ExcelPackage(stream);
            var templateSheet = package.Workbook.Worksheets[0];
            templateSheet.Cells[5, 1].Value = data.Commissions?.Name;
            templateSheet.Cells[5, 1, 5, 3].Merge = true;
            templateSheet.Cells[5, 1, 5, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            templateSheet.Cells[6, 6].Value = data.Commissions?.ApproveForAct?.Position;
            templateSheet.Cells[8, 6].Value = data.Commissions?.ApproveForAct?.ShortName;
            var grouped = data.Expenses
                .Where(e => !string.IsNullOrWhiteSpace(e.Code) && e.Defect.Id != 10 && e.Defect.Id != 11)
                .GroupBy(e => e.Code!)
                .ToList();
            foreach (var group in grouped)
            {
                var code = group.Key;
                var sheetName = ExcelHelpers.GetSafeSheetName(code);
                var newSheet = package.Workbook.Worksheets.Add(sheetName, templateSheet);
                newSheet.Cells[13, 5].Value = code;
                newSheet.Cells[13, 6].Value = group.First().Date.ToString("dd.MM.yyyy");
                var equipmentName = group.First().Equipment.Name;
                var w2 = newSheet.Column(5).Width;
                var w3 = newSheet.Column(6).Width;
                var w4 = newSheet.Column(7).Width;
                newSheet.Cells[16, 5, 16, 7].Merge = true;
                newSheet.Cells[16, 5, 16, 7].Value = equipmentName;
                newSheet.Cells[16, 5, 16, 7].Style.WrapText = true;
                newSheet.Cells[16, 5, 16, 7].Style.VerticalAlignment = ExcelVerticalAlignment.Bottom;
                newSheet.Cells[16, 5, 16, 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                newSheet.Row(16).Height = ExcelHelpers.GetRowHeight( w2 + w3 + w4, newSheet.Cells[16, 5].Style.Font.Name, newSheet.Cells[16, 5].Style.Font.Size, equipmentName);
                newSheet.Cells[16, 9].Value = group.First().Equipment.StateNumber;
                newSheet.Cells[17, 5].Value = group.First().Driver.ShortName;
                newSheet.Cells[17, 5, 17, 7].Merge = true;
                newSheet.Cells[17, 5, 17, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                var startRow = startDataRow;
                foreach (var expense in group.OrderBy(e => e.Stock.Nomenclature.Name))
                {
                    newSheet.Cells[startRow, 1].Value = expense.Stock.Nomenclature.Name;
                    newSheet.Cells[startRow, 1, startRow, 2].Merge = true;
                    var hN = ExcelHelpers.GetRowHeight(
                        newSheet.Column(1).Width + newSheet.Column(2).Width, 
                        newSheet.Cells[startRow, 1, startRow, 2].Style.Font.Name, 
                        newSheet.Cells[startRow, 1, startRow, 2].Style.Font.Size, 
                        expense.Stock.Nomenclature.Name);
                    newSheet.Row(startRow).Height = hN;
                    newSheet.Cells[startRow, 3].Value = expense.Stock.Nomenclature.Article;
                    var hA = ExcelHelpers.GetRowHeight(
                        newSheet.Column(3).Width, 
                        newSheet.Cells[startRow, 3].Style.Font.Name, 
                        newSheet.Cells[startRow, 3].Style.Font.Size,
                        expense.Stock.Nomenclature.Article!);
                    newSheet.Row(startRow).Height = hA > hN ? hA : hN;
                    newSheet.Cells[startRow, 5].Value = expense.Stock.Nomenclature.UnitOfMeasure;
                    newSheet.Cells[startRow, 6].Value = expense.Quantity;
                    newSheet.Cells[startRow, 7].Value = expense.Defect.Id == 5 ? "ТО" : "Текущий ремонт";
                    newSheet.Cells[startRow, 7, startRow, 9].Merge = true;
                    startRow++;
                    var cc = group.Count();
                    if (startDataRow + cc > startRow)
                    {
                        newSheet.Cells[startRow, 1].Insert(eShiftTypeInsert.EntireRow);
                    }
                }
                var commissionRow = startRow + 8;
                if (data.Commissions != null)
                {
                    newSheet.Cells[commissionRow, 2].Value = data.Commissions.Chairman?.Position;
                    newSheet.Cells[commissionRow, 7].Value = data.Commissions.Chairman?.ShortName;
                    newSheet.Cells[commissionRow+3, 2].Value = data.Commissions.Member1?.Position;
                    newSheet.Cells[commissionRow+3, 7].Value = data.Commissions.Member1?.ShortName;
                    newSheet.Cells[commissionRow+5, 2].Value = data.Commissions.Member2?.Position;
                    newSheet.Cells[commissionRow+5, 7].Value = data.Commissions.Member2?.ShortName;
                    newSheet.Cells[commissionRow + 9, 2].Value = data.Frp?.Position;
                    newSheet.Cells[commissionRow + 9, 7].Value = data.Frp?.ShortName;
                }
                
            }
            package.Workbook.Worksheets.Delete(0);
            return package.GetAsByteArray();
        }
    }
}