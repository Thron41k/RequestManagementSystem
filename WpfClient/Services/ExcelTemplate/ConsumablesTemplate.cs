using OfficeOpenXml;
using OfficeOpenXml.Style;
using RequestManagement.Common.Utilities;
using WpfClient.Models.ExcelWriterModels;

namespace WpfClient.Services.ExcelTemplate;

public class ConsumablesTemplate : ExcelTemplateWriterBase<ActPartsModel>
{
    public override ExcelTemplateType TemplateType => ExcelTemplateType.Consumables;
    public override byte[] FillTemplate(ActPartsModel data)
    {
        var startDataRow = 20;
        ExcelPackage.License.SetNonCommercialPersonal("Thron41k");
        using var stream = GetTemplateStream("ConsumablesTemplate.xlsx");
        using var package = new ExcelPackage(stream);
        var templateSheet = package.Workbook.Worksheets[0];
        templateSheet.Cells[4, 1].Value = data.Commissions?.BranchName;
        templateSheet.Cells[5, 8].Value = data.Commissions?.ApproveForAct?.Position;
        templateSheet.Cells[7, 7].Value = data.Commissions?.ApproveForAct?.ShortName;
        templateSheet.Cells[23, 2].Value = data.Commissions?.Chairman?.Position;
        templateSheet.Cells[23, 8].Value = data.Commissions?.Chairman?.ShortName;
        templateSheet.Cells[26, 2].Value = data.Commissions?.Member1?.Position;
        templateSheet.Cells[26, 8].Value = data.Commissions?.Member1?.ShortName;
        templateSheet.Cells[29, 2].Value = data.Commissions?.Member2?.Position;
        templateSheet.Cells[29, 8].Value = data.Commissions?.Member2?.ShortName;
        templateSheet.Cells[33, 2].Value = data.Frp?.Position;
        templateSheet.Cells[33, 8].Value = data.Frp?.ShortName;
        templateSheet.Cells[16, 3].Value = data.Frp?.Position;
        templateSheet.Cells[16, 6].Value = data.Frp?.ShortName;
        var grouped = data.Expenses
            .Where(e => !string.IsNullOrWhiteSpace(e.Code) && e.Defect.DefectGroupId == 15)
            .GroupBy(e => e.Code!)
            .ToList();
        foreach (var group in grouped)
        {
            var code = group.Key;
            var sheetName = ExcelHelpers.GetSafeSheetName(code);
            var newSheet = package.Workbook.Worksheets.Add(sheetName, templateSheet);
            newSheet.Cells[13, 5, 13, 6].Merge = true;
            newSheet.Cells[13, 5, 13, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            newSheet.Cells[13, 5, 13, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            newSheet.Cells[13, 5].Value = code;
            newSheet.Cells[13, 7].Value = group.First().Date.ToString("dd.MM.yyyy");
            var startRow = startDataRow;
            foreach (var expense in group.OrderBy(e => e.Stock.Nomenclature.Name))
            {
                newSheet.Cells[startRow, 1, startRow, 2].Merge = true;
                newSheet.Cells[startRow, 1, startRow, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                newSheet.Cells[startRow, 1, startRow, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                newSheet.Cells[startRow, 1].Value = expense.Stock.Nomenclature.Name;
                var hN = ExcelHelpers.GetRowHeight(
                    newSheet.Column(1).Width + newSheet.Column(2).Width,
                    newSheet.Cells[startRow, 1, startRow, 2].Style.Font.Name,
                    newSheet.Cells[startRow, 1, startRow, 2].Style.Font.Size,
                    expense.Stock.Nomenclature.Name);
                newSheet.Cells[startRow, 3].Value = expense.Stock.Nomenclature.UnitOfMeasure;
                newSheet.Cells[startRow, 4, startRow, 5].Merge = true;
                newSheet.Cells[startRow, 4, startRow, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                newSheet.Cells[startRow, 4, startRow, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                newSheet.Cells[startRow, 4].Value = expense.Quantity;
                newSheet.Cells[startRow, 6, startRow, 10].Merge = true;
                newSheet.Cells[startRow, 6, startRow, 10].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                newSheet.Cells[startRow, 6, startRow, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                newSheet.Cells[startRow, 6].Value = expense.Defect.Name;
                var dN = ExcelHelpers.GetRowHeight(
                    newSheet.Column(6).Width +
                    newSheet.Column(7).Width +
                    newSheet.Column(8).Width +
                    newSheet.Column(9).Width +
                    newSheet.Column(10).Width,
                    newSheet.Cells[startRow, 6, startRow, 10].Style.Font.Name,
                    newSheet.Cells[startRow, 6, startRow, 10].Style.Font.Size,
                    expense.Defect.Name);
                newSheet.Row(startRow).Height = new[] { hN, dN }.Max();
                startRow++;
                var cc = group.Count();
                if (startDataRow + cc > startRow)
                {
                    newSheet.Cells[startRow, 1].Insert(eShiftTypeInsert.EntireRow);
                }
            }
            newSheet.DeleteRow(startRow);
        }
        package.Workbook.Worksheets.Delete(0);
        return package.GetAsByteArray();
    }
}