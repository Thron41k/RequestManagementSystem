using RequestManagement.Common.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Models;
public class Incoming : IEntity
{
    [NotMapped]
    public bool IsSelected { get; set; }
    public int Id { get; set; }
    public int StockId { get; set; }
    public Stock Stock { get; set; } = null!;
    public decimal Quantity { get; set; }
    public DateTime Date { get; set; }
    public string Code { get; set; } = string.Empty;
    public Application? Application { get; set; }
    public int ApplicationId { get; set; }
}
