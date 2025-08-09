using OfficeOpenXml;
using RequestManagement.Common.Utilities;
using RequestManagement.WpfClient.Models.ExcelWriterModels;
using RequestManagement.WpfClient.Utilities;
using static RequestManagement.WpfClient.ViewModels.Helpers.ExcelColumnsWidth;

namespace RequestManagement.WpfClient.Services.ExcelTemplate;

public class RequisitionInvoiceTemplate : ExcelTemplateWriterBase<IncomingPrintModel>
{
    public override ExcelTemplateType TemplateType => ExcelTemplateType.RequisitionInvoice;

    public override byte[] FillTemplate(IncomingPrintModel data)
    {
        var startDataRow = 19;
        ExcelPackage.License.SetNonCommercialPersonal("Thron41k");
        using var stream = GetTemplateStream("RequisitionInvoiceTemplate.xlsx");
        using var package = new ExcelPackage(stream);
        var templateSheet = package.Workbook.Worksheets[0];
        templateSheet.Cells[5, 2].Value = data.Commissions?.BranchName;
        templateSheet.Cells[14, 10].Value = data.Commissions?.ApproveForAct?.Position;
        templateSheet.Cells[14, 16].Value = data.Commissions?.ApproveForAct?.ShortName;
        var grouped = data.Incomings
            .GroupBy(e => e.Code)
            .OrderBy(g => g.Key)
            .ToList();
        foreach (var group in grouped)
        {
            var startRow = startDataRow;
            var code = group.Key;
            var sheetName = ExcelHelpers.GetSafeSheetName(code);
            var newSheet = package.Workbook.Worksheets.Add(sheetName, templateSheet);
            newSheet.Cells[11, 2].Value = group.First().Date.ToString("dd.MM.yyyy");
            newSheet.Cells[4, 2].Value = $"ТРЕБОВАНИЕ - НАКЛАДНАЯ №{code}";
            newSheet.Cells[11, 5].Value = group.First().InWarehouse?.Name;
            newSheet.Cells[23, 4].Value = group.First().InWarehouse?.FinanciallyResponsiblePerson?.Position;
            newSheet.Cells[23, 7].Value = group.First().InWarehouse?.FinanciallyResponsiblePerson?.ShortName;
            newSheet.Cells[11, 9].Value = group.First().Stock.Warehouse.Name;
            newSheet.Cells[23, 10].Value = group.First().Stock.Warehouse.FinanciallyResponsiblePerson?.Position;
            newSheet.Cells[23, 17].Value = group.First().Stock.Warehouse.FinanciallyResponsiblePerson?.ShortName;
            newSheet.Cells[20, 12].Value = group.Sum(x => x.Quantity);
            newSheet.Cells[20, 13].Value = group.Sum(x => x.Quantity);
            foreach (var item in group.OrderBy(x=>x.Stock.Nomenclature.Name))
            {
                newSheet.Cells[startRow, 5].Value = item.Stock.Nomenclature.Name;
                newSheet.Cells[startRow, 5, startRow, 6].Merge = true;
                var nameHeight = ExcelHelpers.GetRowHeight(GetColumnsWidth(newSheet, 5, 6),
                    newSheet.Cells[startRow, 8].Style.Font.Name,
                    newSheet.Cells[startRow, 8].Style.Font.Size,
                    item.Stock.Nomenclature.Name);
                newSheet.Cells[startRow, 7].Value = item.Stock.Nomenclature.Article;
                var articleHeight = ExcelHelpers.GetRowHeight(GetColumnsWidth(newSheet, 7, 7),
                    newSheet.Cells[startRow, 8].Style.Font.Name,
                    newSheet.Cells[startRow, 8].Style.Font.Size,
                    item.Stock.Nomenclature.Article!);
                newSheet.Row(startRow).Height = new[] { nameHeight, articleHeight }.Max();
                newSheet.Cells[startRow, 10].Value = item.Stock.Nomenclature.UnitOfMeasure;
                newSheet.Cells[startRow, 10, startRow, 11].Merge = true;
                newSheet.Cells[startRow, 12].Value = item.Quantity;
                newSheet.Cells[startRow, 13].Value = item.Quantity;
                newSheet.Cells[startRow, 13, startRow, 15].Merge = true;
                startRow++;
                var cc = group.Count();
                if (startDataRow + cc > startRow)
                {
                    newSheet.Cells[startRow, 1].Insert(eShiftTypeInsert.EntireRow);
                }
            }

        }
        package.Workbook.Worksheets.Delete(0);
        return package.GetAsByteArray();
    }
}