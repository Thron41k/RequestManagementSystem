using RequestManagement.Common.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Models
{
    public class Expense : IEntity
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        public Stock Stock { get; set; } = null!;
        public decimal Quantity { get; set; }
        public int EquipmentId { get; set; }
        public Equipment Equipment { get; set; } = null!;
        public int DriverId { get; set; }
        public Driver Driver { get; set; } = null!;
        public int DefectId { get; set; }
        public Defect Defect { get; set; } = null!;
        public DateTime Date { get; set; }
    }
}
