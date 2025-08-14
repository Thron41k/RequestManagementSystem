using OfficeOpenXml;
using RequestManagement.Common.Utilities;
using RequestManagement.WpfClient.Models.ExcelWriterModels;
using RequestManagement.WpfClient.Utilities;

namespace RequestManagement.WpfClient.Services.ExcelTemplate;

public class MaterialsInUseOffTemplateV1 : ExcelTemplateWriterBase<MaterialsInUseOffPrintModel>
{
    public override ExcelTemplateType TemplateType => ExcelTemplateType.MaterialsInUseOffTemplateV1;
    public override byte[] FillTemplate(MaterialsInUseOffPrintModel data)
    {
        var startDataRow = 23;
        ExcelPackage.License.SetNonCommercialPersonal("Thron41k");
        using var stream = GetTemplateStream("MaterialsInUseOffTemplateV1.xlsx");
        using var package = new ExcelPackage(stream);
        var templateSheet = package.Workbook.Worksheets[0];
        templateSheet.Cells[11, 5].Value = data.Commissions?.BranchName;
        templateSheet.Cells[3, 18].Value = data.Commissions?.ApproveForAct?.Position;
        templateSheet.Cells[5, 20].Value = data.Commissions?.ApproveForAct?.ShortName;
        templateSheet.Cells[41, 9].Value = data.Commissions?.Chairman?.Position;
        templateSheet.Cells[41, 14].Value = data.Commissions?.Chairman?.Position;
        templateSheet.Cells[43, 9].Value = data.Commissions?.Member1?.Position;
        templateSheet.Cells[43, 14].Value = data.Commissions?.Member1?.Position;
        templateSheet.Cells[45, 9].Value = data.Commissions?.Member2?.Position;
        templateSheet.Cells[45, 14].Value = data.Commissions?.Member2?.Position;
        var grouped = data.MaterialsInUse
            .GroupBy(e => e.DocumentNumberForWriteOff)
            .OrderBy(g => g.Key)
            .ToList();
        foreach (var group in grouped)
        {
            var code = group.Key;
            var sheetName = ExcelHelpers.GetSafeSheetName(code);
            var newSheet = package.Workbook.Worksheets.Add(sheetName, templateSheet);
            newSheet.Cells[3, 3].Value = $"АКТ № {code}";
            newSheet.Cells[11, 10].Value = group.First().DateForWriteOff.ToString("dd.MM.yyyy");
            var startRow = startDataRow;
            foreach (var item in group.OrderBy(x => x.Nomenclature.Name))
            {
                var cc = group.Count();
                if (cc == 1)
                {
                    newSheet.DeleteRow(startRow);
                }
                newSheet.Cells[startRow, 3].Value = item.Nomenclature.Name;
                var nameHeight = ExcelHelpers.GetRowHeight(ViewModels.Helpers.ExcelColumnsWidth.GetColumnsWidth(newSheet,3,4), newSheet.Cells[startRow, 3].Style.Font.Name, newSheet.Cells[startRow, 3].Style.Font.Size, item.Nomenclature.Name);
                newSheet.Cells[startRow, 5].Value = item.Nomenclature.Article;
                var articleHeight = ExcelHelpers.GetRowHeight(ViewModels.Helpers.ExcelColumnsWidth.GetColumnsWidth(newSheet, 5, 5), newSheet.Cells[startRow, 5].Style.Font.Name, newSheet.Cells[startRow, 5].Style.Font.Size, item.Nomenclature.Article!);
                newSheet.Cells[startRow, 10].Value = item.Nomenclature.UnitOfMeasure;
                newSheet.Cells[startRow, 12].Value = item.Quantity;
                newSheet.Cells[startRow, 13].Value = item.DateForList;
                newSheet.Cells[startRow, 17].Value = item.ServiceLife;
                newSheet.Cells[startRow, 18].Value = item.ReasonForWriteOff.Reason;
                var reasonHeight = ExcelHelpers.GetRowHeight(ViewModels.Helpers.ExcelColumnsWidth.GetColumnsWidth(newSheet, 18, 19), newSheet.Cells[startRow, 18].Style.Font.Name, newSheet.Cells[startRow, 18].Style.Font.Size, item.ReasonForWriteOff.Reason);
                newSheet.Row(startRow).Height = new[] { nameHeight, articleHeight, reasonHeight }.Max();
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