using RequestManagement.Common.Models;

namespace WpfClient.Models.ExcelWriterModels;

public class IncomingPrintModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Commissions? Commissions { get; set; } = new Commissions();
    public List<Incoming> Incomings { get; set; } = [];
}