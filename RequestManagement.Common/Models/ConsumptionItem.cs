using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Models
{
    public class ConsumptionItem
    {
        public int Id { get; set; }
        public int NomenclatureId { get; set; }      // Номенклатура (внешний ключ)
        public Nomenclature Nomenclature { get; set; } // Навигационное свойство
        public int Quantity { get; set; }            // Кол-во
        public int DefectId { get; set; }            // Дефект (внешний ключ)
        public Defect Defect { get; set; }           // Навигационное свойство
        public int ConsumptionId { get; set; }       // Расход (внешний ключ)
        public Consumption Consumption { get; set; } // Навигационное свойство
    }
}
