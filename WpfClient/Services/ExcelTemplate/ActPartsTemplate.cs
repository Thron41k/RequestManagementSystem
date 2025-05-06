using OfficeOpenXml;
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
                .Where(e => !string.IsNullOrWhiteSpace(e.Code))
                .GroupBy(e => e.Code!)
                .ToList();
            foreach (var group in grouped)
            {
                var code = group.Key;
                var sheetName = ExcelHelpers.GetSafeSheetName(code);
                var newSheet = package.Workbook.Worksheets.Add(sheetName, templateSheet);
                newSheet.Cells[13, 5].Value = code;
                newSheet.Cells[13, 6].Value = group.First().Date.ToString("dd.MM.yyyy");
                newSheet.Cells[16, 5].Value = group.First().Equipment.Name;
                newSheet.Cells[16, 5, 16, 7].Merge = true;
                newSheet.Cells[16, 5, 16, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                newSheet.Cells[16, 9].Value = group.First().Equipment.StateNumber;
                newSheet.Cells[17, 5].Value = group.First().Driver.ShortName;
                newSheet.Cells[17, 5, 17, 7].Merge = true;
                newSheet.Cells[17, 5, 17, 7].Style.WrapText = true;
                newSheet.Cells[17, 5, 17, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                var startRow = startDataRow;
                foreach (var expense in group)
                {
                    newSheet.Cells[startRow, 1].Value = expense.Stock.Nomenclature.Name;
                    newSheet.Cells[startRow, 1, startRow, 2].Merge = true;
                    newSheet.Row(startRow).Height = MeasureTextHeight(expense.Stock.Nomenclature.Name, newSheet.Cells[startRow, 1, startRow, 2].Style.Font, newSheet.Column(1).Width + newSheet.Column(2).Width);
                    newSheet.Cells[startRow, 3].Value = expense.Stock.Nomenclature.Article;
                    newSheet.Cells[startRow, 3].Style.WrapText = true;
                    newSheet.Cells[startRow, 5].Value = expense.Stock.Nomenclature.UnitOfMeasure;
                    newSheet.Cells[startRow, 6].Value = expense.Quantity;
                    newSheet.Cells[startRow, 7].Value = expense.Defect.Name;
                    newSheet.Cells[startRow, 7, startRow, 9].Merge = true;
                    startRow++;
                    var cc = group.Count();
                    if (startDataRow + cc > startRow)
                    {
                        newSheet.Cells[startRow, 1].Insert(eShiftTypeInsert.EntireRow);
                    }
                }
                //newSheet.Cells[21, 1].Insert(eShiftTypeInsert.EntireRow);
            }

            package.Workbook.Worksheets.Delete(0);
            return package.GetAsByteArray();
        }
    }
}
