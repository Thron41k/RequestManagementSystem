using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequestManagement.Common.Models.Interfaces;

namespace RequestManagement.Common.Models
{
    public class MaterialsInUse : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IsOut { get; set; }
        public string DocumentNumber { get; set; } = "";
        public DateTime Date { get; set; }
        public decimal Quantity { get; set; }
        public int NomenclatureId { get; set; }
        public Nomenclature Nomenclature { get; set; } = null!;
        public int EquipmentId { get; set; }
        public Equipment? Equipment { get; set; } = null;
        public Driver FinanciallyResponsiblePerson { get; set; } = null!;
        public int FinanciallyResponsiblePersonId { get; set; } = 1;
        public string ReasonForWriteOff { get; set; } = "";
        public string DocumentNumberForWriteOff { get; set; } = "";
        public DateTime? DateForWriteOff { get; set; }
    }
}
