using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequestManagement.Common.Models.Interfaces;

namespace RequestManagement.Common.Models
{
    public class EquipmentGroup : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Equipment> Equipments { get; set; } = [];
        public List<SparePartsOwnership> SparePartsOwnerships { get; set; } = [];
        public override bool Equals(object obj) => obj is EquipmentGroup equipmentGroup && Id == equipmentGroup.Id;
    }
}
