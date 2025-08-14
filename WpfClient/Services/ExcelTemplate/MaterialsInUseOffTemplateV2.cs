using OfficeOpenXml;
using RequestManagement.WpfClient.Models.ExcelWriterModels;

namespace RequestManagement.WpfClient.Services.ExcelTemplate;

public class MaterialsInUseOffTemplateV2 : ExcelTemplateWriterBase<MaterialsInUseOffPrintModel>
{
    public override ExcelTemplateType TemplateType => ExcelTemplateType.MaterialsInUseOffTemplateV2;
    public override byte[] FillTemplate(MaterialsInUseOffPrintModel data)
    {
        var startDataRow = 25;
        ExcelPackage.License.SetNonCommercialPersonal("Thron41k");
        using var stream = GetTemplateStream("MaterialsInUseOffTemplateV2.xlsx");
        using var package = new ExcelPackage(stream);
        var templateSheet = package.Workbook.Worksheets[0];

        package.Workbook.Worksheets.Delete(0);
        return package.GetAsByteArray();
    }
}