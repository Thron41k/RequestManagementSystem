using RequestManagement.Common.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Models
{
    public class UserLastSelection : IEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int? DriverId { get; set; }
        public Driver? Driver { get; set; }
        public int? EquipmentId { get; set; }
        public Equipment? Equipment { get; set; }
        public int? CommissionsId { get; set; }
        public Commissions? Commissions { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
