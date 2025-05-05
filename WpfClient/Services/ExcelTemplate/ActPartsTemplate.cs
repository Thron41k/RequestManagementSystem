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
            ExcelPackage.License.SetNonCommercialPersonal("Thron41k");
            using var stream = GetTemplateStream("ActPartsTemplate.xlsx");
            using var package = new ExcelPackage(stream);
            var templateSheet = package.Workbook.Worksheets[0];
            templateSheet.Cells[5, 2].Value = data.Commissions?.Name;
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
                newSheet.Cells[16, 9].Value = group.First().Equipment.StateNumber;
                newSheet.Cells[17, 5].Value = group.First().Driver.ShortName;
            }

            package.Workbook.Worksheets.Delete(0);
            return package.GetAsByteArray();
        }
    }
}
