using OfficeOpenXml;
using RequestManagement.Common.Utilities;
using WpfClient.Models.ExcelWriterModels;

namespace WpfClient.Services.ExcelTemplate;

public class OperationsTemplate : ExcelTemplateWriterBase<ActPartsModel>
{
    public override ExcelTemplateType TemplateType => ExcelTemplateType.Operations;
    public override byte[] FillTemplate(ActPartsModel data)
    {
        var startDataRow = 25;
        ExcelPackage.License.SetNonCommercialPersonal("Thron41k");
        using var stream = GetTemplateStream("OperationsTemplate.xlsx");
        using var package = new ExcelPackage(stream);
        var templateSheet = package.Workbook.Worksheets[0];
        templateSheet.Cells[16, 4].Value = data.Commissions?.BranchName;
        templateSheet.Cells[5, 20].Value = data.Commissions?.ApproveForAct?.Position;
        templateSheet.Cells[7, 20].Value = data.Commissions?.ApproveForAct?.ShortName;
        templateSheet.Cells[28, 13].Value = data.Commissions?.ApproveForDefectAndLimit?.Position;
        templateSheet.Cells[28, 20].Value = data.Commissions?.ApproveForDefectAndLimit?.ShortName;
        templateSheet.Cells[28, 3].Value = data.Frp?.Position;
        templateSheet.Cells[28, 9].Value = data.Frp?.ShortName;
        var grouped = data.Expenses
            .Where(e => !string.IsNullOrWhiteSpace(e.Code) && e.Defect.Id == 4)
            .OrderBy(e => e.Stock.Nomenclature.Name)
            .GroupBy(e => e.Code!)
            .ToList();
        foreach (var group in grouped)
        {
            var code = group.Key;
            var sheetName = ExcelHelpers.GetSafeSheetName(code);
            var newSheet = package.Workbook.Worksheets.Add(sheetName, templateSheet);
            newSheet.Cells[10, 1].Value = $"ВЕДОМОСТЬ № {code}";
            newSheet.Cells[20, 13].Value = group.First().Date.ToString("dd.MM.yyyy");
            var gn = string.IsNullOrEmpty(group.First().Equipment.StateNumber) ? "" : $"({group.First().Equipment.StateNumber})";
            newSheet.Cells[17, 3].Value = $"{group.First().Equipment.Name}{gn}";
            var startRow = startDataRow;
            foreach (var item in group.OrderBy(x => x.Stock.Nomenclature.Name))
            {
                var cc = group.Count();
                if (cc == 1)
                {
                    newSheet.DeleteRow(startRow);
                }
                newSheet.Cells[startRow, 1].Value = startRow - startDataRow + 1;
                newSheet.Cells[startRow, 2].Value = item.Driver.FullName;
                var driverNameHeight = ExcelHelpers.GetRowHeight(newSheet.Column(2).Width +
                                                                 newSheet.Column(3).Width +
                                                                 newSheet.Column(4).Width +
                                                                 newSheet.Column(5).Width,
                    newSheet.Cells[startRow, 2].Style.Font.Name, newSheet.Cells[startRow, 2].Style.Font.Size, item.Driver.FullName);
                newSheet.Cells[startRow, 2, startRow, 5].Merge = true;
                newSheet.Cells[startRow, 7].Value = item.Stock.Nomenclature.Name;
                var nameHeight = ExcelHelpers.GetRowHeight(newSheet.Column(7).Width +
                                                           newSheet.Column(8).Width +
                                                           newSheet.Column(9).Width,
                    newSheet.Cells[startRow, 7].Style.Font.Name, newSheet.Cells[startRow, 7].Style.Font.Size, item.Stock.Nomenclature.Name);
                newSheet.Cells[startRow, 7, startRow, 9].Merge = true;
                newSheet.Cells[startRow, 8].Value = item.Stock.Nomenclature.Article;
                var articleHeight = ExcelHelpers.GetRowHeight(newSheet.Column(10).Width,
                    newSheet.Cells[startRow, 10].Style.Font.Name, newSheet.Cells[startRow, 10].Style.Font.Size, item.Stock.Nomenclature.Article!);
                try
                {
                    newSheet.Row(startRow).Height = new[] { nameHeight, articleHeight, driverNameHeight }.Max();
                    newSheet.Cells[startRow, 12].Value = item.Stock.Nomenclature.UnitOfMeasure;
                    newSheet.Cells[startRow, 12, startRow, 13].Merge = true;
                    newSheet.Cells[startRow, 14].Value = item.Quantity;
                    newSheet.Cells[startRow, 14, startRow, 16].Merge = true;
                    newSheet.Cells[startRow, 17].Value = item.Date.ToString("dd.MM.yyyy");
                    newSheet.Cells[startRow, 17, startRow, 19].Merge = true;
                    newSheet.Cells[startRow, 20].Value = item.Term is 0 or null ? "" : item.Term.ToString();
                    newSheet.Cells[startRow, 20, startRow, 21].Merge = true;
                }

                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
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