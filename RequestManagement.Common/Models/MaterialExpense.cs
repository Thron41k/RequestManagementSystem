using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Models
{
    public class MaterialExpense
    {
        public string Number { get; set; }
        public DateTime Date { get; set; }
        public string DriverFullName { get; set; }
        public string DriverCode { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentCode { get; set; }
        public string NomenclatureName { get; set; }
        public string NomenclatureCode { get; set; }
        public string NomenclatureArticle { get; set; } 
        public string NomenlatureUnitOfMeasure { get; set; }
        public decimal Quantity { get; set; }
    }
}
