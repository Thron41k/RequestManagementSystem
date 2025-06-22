using RequestManagement.Common.Models;

namespace WpfClient.Models.ExcelWriterModels;

public class ActPartsModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Commissions? Commissions { get; set; } = new Commissions();
    public Driver? Frp { get; set; } = new Driver();
    public List<Expense> Expenses { get; set; } = [];
}