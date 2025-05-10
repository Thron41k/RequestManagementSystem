using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequestManagement.Common.Models.Interfaces;

namespace RequestManagement.Common.Models
{
    public class Application : IEntity
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public Driver? Responsible { get; set; }
        public int ResponsibleId { get; set; }
        public Equipment? Equipment { get; set; }
        public int EquipmentId { get; set; }
    }
}
