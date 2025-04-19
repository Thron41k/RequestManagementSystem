using RequestManagement.Common.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Models
{
    public class NomenclatureDefectMapping : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int NomenclatureId { get; set; }
        public Nomenclature Nomenclature { get; set; } = null!;
        public int DefectId { get; set; }
        public Defect Defect { get; set; } = null!;
        public DateTime LastUsed { get; set; }
    }
}
