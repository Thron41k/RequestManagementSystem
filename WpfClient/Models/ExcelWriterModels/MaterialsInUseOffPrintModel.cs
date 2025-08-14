using RequestManagement.Common.Models;

namespace RequestManagement.WpfClient.Models.ExcelWriterModels;

public class MaterialsInUseOffPrintModel
{
    public Commissions? Commissions { get; set; } = new();
    public List<MaterialsInUse> MaterialsInUse { get; set; } = [];
}