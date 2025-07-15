using System.Globalization;
using OfficeOpenXml;
using RequestManagement.Common.Utilities;
using RequestManagement.WpfClient.Models.ExcelWriterModels;
using static RequestManagement.WpfClient.ViewModels.Helpers.ExcelColumnsWidth;

namespace RequestManagement.WpfClient.Services.ExcelTemplate;

public class MovingBetweenWarehousesTemplate : ExcelTemplateWriterBase<IncomingPrintModel>
{
    public override ExcelTemplateType TemplateType => ExcelTemplateType.MovingBetweenWarehouses;

    public override byte[] FillTemplate(IncomingPrintModel data)
    {
        var startDataRow = 12;
        ExcelPackage.License.SetNonCommercialPersonal("Thron41k");
        using var stream = GetTemplateStream("MovingBetweenWarehousesTemplate.xlsx");
        using var package = new ExcelPackage(stream);
        var templateSheet = package.Workbook.Worksheets[0];
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
            var title = $"Перемещение между складами № {code} от {data.EndDate.ToString("d MMMM yyyyг.", new CultureInfo("ru-RU"))}";
            newSheet.Cells[2, 2].Value = title;
            newSheet.Cells[6, 7].Value = group.First().InWarehouse?.Name;
            newSheet.Cells[18, 6].Value = group.First().InWarehouse?.FinanciallyResponsiblePerson?.ShortName;
            newSheet.Cells[8, 7].Value = group.First().Stock.Warehouse.Name;
            newSheet.Cells[18, 24].Value = group.First().Stock.Warehouse.FinanciallyResponsiblePerson?.ShortName;
            newSheet.Cells[15, 2].Value = $"Всего наименований {group.Count()}";
            foreach (var item in group.OrderBy(x => x.Stock.Nomenclature.Name))
            {
                newSheet.Cells[startRow, 2].Value = startRow - startDataRow + 1;
                newSheet.Cells[startRow, 2, startRow, 3].Merge = true;
                newSheet.Cells[startRow, 4].Value = item.Stock.Nomenclature.Article;
                newSheet.Cells[startRow, 4, startRow, 7].Merge = true;
                var articleHeight = ExcelHelpers.GetRowHeight(GetColumnsWidth(newSheet, 4, 7),
                    newSheet.Cells[startRow, 4].Style.Font.Name,
                    newSheet.Cells[startRow, 4].Style.Font.Size,
                    item.Stock.Nomenclature.Article!);
                newSheet.Cells[startRow, 8].Value = item.Stock.Nomenclature.Name;
                newSheet.Cells[startRow, 8, startRow, 31].Merge = true;
                var nameHeight = ExcelHelpers.GetRowHeight(GetColumnsWidth(newSheet, 8, 31),
                    newSheet.Cells[startRow, 8].Style.Font.Name,
                    newSheet.Cells[startRow, 8].Style.Font.Size,
                    item.Stock.Nomenclature.Name);
                newSheet.Cells[startRow, 32].Value = item.Quantity.ToString("0.000");
                newSheet.Cells[startRow, 32, startRow, 37].Merge = true;
                newSheet.Cells[startRow, 38].Value = item.Stock.Nomenclature.UnitOfMeasure;
                newSheet.Cells[startRow, 38, startRow, 41].Merge = true;
                var unitOfMeasureHeight = ExcelHelpers.GetRowHeight(GetColumnsWidth(newSheet, 38, 41),
                    newSheet.Cells[startRow, 38].Style.Font.Name,
                    newSheet.Cells[startRow, 38].Style.Font.Size,
                    item.Stock.Nomenclature.UnitOfMeasure);
                newSheet.Row(startRow).Height = new[] { nameHeight, articleHeight, unitOfMeasureHeight }.Max();
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