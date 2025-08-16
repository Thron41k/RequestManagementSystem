using System.Globalization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RequestManagement.Common.Utilities;
using RequestManagement.WpfClient.Models.ExcelWriterModels;
using RequestManagement.WpfClient.Utilities;
using static RequestManagement.WpfClient.ViewModels.Helpers.ExcelColumnsWidth;

namespace RequestManagement.WpfClient.Services.ExcelTemplate;

public class DefectPartsTemplate : ExcelTemplateWriterBase<ActPartsModel>
{
    public override ExcelTemplateType TemplateType => ExcelTemplateType.DefectParts;
    public override byte[] FillTemplate(ActPartsModel data)
    {
        var startDataRow = 20;
        ExcelPackage.License.SetNonCommercialPersonal("Thron41k");
        using var stream = GetTemplateStream("DefectPartsTemplate.xlsx");
        using var package = new ExcelPackage(stream);
        var templateSheet = package.Workbook.Worksheets[0];
        templateSheet.Cells[4, 1].Value = data.Commissions?.BranchName;
        templateSheet.Cells[4, 1, 4, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
        templateSheet.Cells[5, 6].Value = data.Commissions?.ApproveForDefectAndLimit?.Position;
        templateSheet.Cells[5, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        templateSheet.Cells[7, 5].Value = data.Commissions?.ApproveForDefectAndLimit?.ShortName;
        templateSheet.Cells[7, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        templateSheet.Cells[11, 3].Value = data.EndDate.ToString("d MMMM yyyyг.", new CultureInfo("ru-RU"));
        templateSheet.Cells[22, 3].Value = data.Frp?.ShortName;
        var grouped = data.Expenses
            .Where(e => !string.IsNullOrWhiteSpace(e.Code) && e.Defect.Id != 2 && e.Defect.Id != 3 && e.Defect.DefectGroupId != 15 && e.Defect.DefectGroupId != 16 && e.Defect.DefectGroupId != 19)
            .GroupBy(e => e.Code!)
            .OrderBy(g => g.Key)
            .ToList();
        foreach (var group in grouped)
        {
            var code = group.Key;
            var sheetName = ExcelHelpers.GetSafeSheetName(code);
            var newSheet = package.Workbook.Worksheets.Add(sheetName, templateSheet);
            newSheet.Cells[13, 4].Value = group.First().Equipment.Name;
            var equipmentHeight = ExcelHelpers.GetRowHeight(GetColumnsWidth(newSheet,4,6), newSheet.Cells[13, 4].Style.Font.Name, newSheet.Cells[13, 4].Style.Font.Size, group.First().Equipment.Name);
            var preEquipmentHeight = ExcelHelpers.GetRowHeight(GetColumnsWidth(newSheet, 1, 2), newSheet.Cells[13, 1].Style.Font.Name, newSheet.Cells[13, 1].Style.Font.Size, "Марка транспортного средства (машины, механизма):");
            newSheet.Row(13).Height = new[]{equipmentHeight, preEquipmentHeight}.Max();
            newSheet.Cells[14, 2].Value = group.First().Equipment.StateNumber;
            var startRow = startDataRow;
            var groupingByDefectGroupName = group.OrderBy(x=>x.Defect.DefectGroup.Name).GroupBy(e => e.Defect.DefectGroup.Name).ToList();
            var defectGroupIndex = 1;
            foreach (var defectGroup in groupingByDefectGroupName)
            {
                var defectGroupName = defectGroup.Key;
                newSheet.Cells[startRow, 1].Insert(eShiftTypeInsert.EntireRow);
                newSheet.Cells[startRow, 1].Value = defectGroupIndex;
                newSheet.Cells[startRow, 2].Value = defectGroupName;
                newSheet.Cells[startRow, 2].Style.HorizontalAlignment =
                    ExcelHorizontalAlignment.Left;
                startRow++;
                var defectGroupItemIndex = 1;
                foreach (var defectGroupItem in defectGroup.OrderBy(x=>x.Stock.Nomenclature.Name))
                {
                    newSheet.Cells[startRow, 1].Insert(eShiftTypeInsert.EntireRow);
                    newSheet.Cells[startRow, 1].Value = $"{defectGroupIndex}.{defectGroupItemIndex++}";
                    newSheet.Cells[startRow, 2].Value = defectGroupItem.Defect.Name;
                    newSheet.Cells[startRow, 2, startRow, 3].Merge = true;
                    newSheet.Cells[startRow, 2, startRow, 3].Style.HorizontalAlignment =
                        ExcelHorizontalAlignment.Right;
                    newSheet.Cells[startRow, 4].Style.HorizontalAlignment =
                        ExcelHorizontalAlignment.Left;
                    newSheet.Cells[startRow, 4].Value = defectGroupItem.Stock.Nomenclature.Name;
                    var nomenclatureNameHeight = ExcelHelpers.GetRowHeight(newSheet.Column(4).Width, newSheet.Cells[startRow, 4].Style.Font.Name, newSheet.Cells[startRow, 4].Style.Font.Size, defectGroupItem.Stock.Nomenclature.Name);
                    newSheet.Cells[startRow, 5].Value = defectGroupItem.Quantity;
                    newSheet.Cells[startRow, 6].Style.HorizontalAlignment =
                        ExcelHorizontalAlignment.Left;
                    newSheet.Cells[startRow, 6].Value = defectGroupItem.Stock.Nomenclature.Article;
                    var articleNameHeight = ExcelHelpers.GetRowHeight(newSheet.Column(6).Width, newSheet.Cells[startRow, 6].Style.Font.Name, newSheet.Cells[startRow, 6].Style.Font.Size, defectGroupItem.Stock.Nomenclature.Article);
                    newSheet.Row(startRow).Height = Math.Max(nomenclatureNameHeight, articleNameHeight);
                    startRow++;
                }
                defectGroupIndex++;
            }
            newSheet.DeleteRow(startRow);
        }
        package.Workbook.Worksheets.Delete(0);
        return package.GetAsByteArray();
    }
}