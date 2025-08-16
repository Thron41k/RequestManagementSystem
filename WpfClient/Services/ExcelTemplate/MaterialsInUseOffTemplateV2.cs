using OfficeOpenXml;
using RequestManagement.WpfClient.Models.ExcelWriterModels;
using RequestManagement.WpfClient.Utilities;
using static RequestManagement.WpfClient.ViewModels.Helpers.ExcelColumnsWidth;
using System.Globalization;

namespace RequestManagement.WpfClient.Services.ExcelTemplate;

public class MaterialsInUseOffTemplateV2 : ExcelTemplateWriterBase<MaterialsInUseOffPrintModel>
{
    public override ExcelTemplateType TemplateType => ExcelTemplateType.MaterialsInUseOffTemplateV2;
    public override byte[] FillTemplate(MaterialsInUseOffPrintModel data)
    {
        var startDataRow = 28;
        ExcelPackage.License.SetNonCommercialPersonal("Thron41k");
        using var stream = GetTemplateStream("MaterialsInUseOffTemplateV2.xlsx");
        using var package = new ExcelPackage(stream);
        var templateSheet = package.Workbook.Worksheets[0];
        templateSheet.Cells[14, 2].Value = data.Commissions?.BranchName;
        templateSheet.Cells[4, 24].Value = data.Commissions?.ApproveForAct?.Position;
        templateSheet.Cells[6, 30].Value = data.Commissions?.ApproveForAct?.ShortName;
        templateSheet.Cells[46, 2].Value = data.Commissions?.Chairman?.Position;
        templateSheet.Cells[46, 18].Value = data.Commissions?.Chairman?.ShortName;
        templateSheet.Cells[48, 2].Value = data.Commissions?.Member1?.Position;
        templateSheet.Cells[48, 18].Value = data.Commissions?.Member1?.ShortName;
        templateSheet.Cells[50, 2].Value = data.Commissions?.Member2?.Position;
        templateSheet.Cells[50, 18].Value = data.Commissions?.Member2?.ShortName;
        var grouped = data.MaterialsInUse
            .GroupBy(e => e.DocumentNumberForWriteOff)
            .OrderBy(g => g.Key)
            .ToList();
        foreach (var group in grouped)
        {
            var code = group.Key;
            var sheetName = ExcelHelpers.GetSafeSheetName(code);
            var newSheet = package.Workbook.Worksheets.Add(sheetName, templateSheet);
            newSheet.Cells[9, 12].Value = code;
            var date = group.First().DateForWriteOff;
            newSheet.Cells[20, 17].Value = date.ToString("dd.MM.yyyy");
            newSheet.Cells[8, 22].Value = date.ToString("dd");
            newSheet.Cells[8, 25].Value = date.ToString("MMMM", new CultureInfo("ru-RU"));
            newSheet.Cells[8, 33].Value = date.ToString("yyyy");
            newSheet.Cells[15, 2].Value = group.First().Equipment.FullName;
            newSheet.Cells[30, 9].Value = group.Sum(x => x.Quantity);
            newSheet.Cells[31, 8].Value = ExcelHelpers.NumberToWords((int)group.Sum(x => x.Quantity));
            var startRow = startDataRow;
            foreach (var item in group.OrderBy(x => x.Nomenclature.Name))
            {
                var cc = group.Count();
                if (cc == 1)
                {
                    newSheet.DeleteRow(startRow);
                }
                newSheet.Cells[startRow, 1].Value = item.Nomenclature.Name;
                var nameHeight = ExcelHelpers.GetRowHeight(GetColumnsWidth(newSheet, 1, 1), newSheet.Cells[startRow, 1].Style.Font.Name, newSheet.Cells[startRow, 1].Style.Font.Size, item.Nomenclature.Name);
                newSheet.Cells[startRow, 2].Value = item.Nomenclature.Article;
                var articleHeight = ExcelHelpers.GetRowHeight(GetColumnsWidth(newSheet, 2, 2), newSheet.Cells[startRow, 2].Style.Font.Name, newSheet.Cells[startRow, 2].Style.Font.Size, item.Nomenclature.Article!);
                newSheet.Cells[startRow, 3, startRow, 4].Merge = true;
                newSheet.Cells[startRow, 5, startRow, 6].Merge = true;
                newSheet.Cells[startRow, 7].Value = item.Nomenclature.UnitOfMeasure;
                newSheet.Cells[startRow, 7, startRow, 8].Merge = true;
                newSheet.Cells[startRow, 9].Value = item.Quantity;
                newSheet.Cells[startRow, 9, startRow, 11].Merge = true;
                newSheet.Cells[startRow, 12].Value = item.Date.ToString("dd.MM.yyyy");
                newSheet.Cells[startRow, 12, startRow, 14].Merge = true;
                newSheet.Cells[startRow, 15].Value = item.ReasonForWriteOffId == 2 ? "12" : "24";
                newSheet.Cells[startRow, 15, startRow, 17].Merge = true;
                newSheet.Cells[startRow, 18].Value = item.ServiceLife;
                newSheet.Cells[startRow, 20].Value = item.ReasonForWriteOff.Reason;
                var reasonHeight = ExcelHelpers.GetRowHeight(GetColumnsWidth(newSheet, 20, 38), newSheet.Cells[startRow, 20].Style.Font.Name, newSheet.Cells[startRow, 20].Style.Font.Size, item.ReasonForWriteOff.Reason);
                newSheet.Cells[startRow, 20, startRow, 38].Merge = true;
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