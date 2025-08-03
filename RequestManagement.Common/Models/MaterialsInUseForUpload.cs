using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Models
{
    public class MaterialsInUseForUpload
    {
        public string DocumentNumber { get; set; }
        public string Date { get; set; }
        public decimal Quantity { get; set; }
        public string NomenclatureCode { get; set; }
        public string NomenclatureName { get; set; }
        public string NomenclatureArticle { get; set; }
        public string NomenclatureUnitOfMeasure { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentCode { get; set; }
        public string FinanciallyResponsiblePersonFullName { get; set; }
    }
}
