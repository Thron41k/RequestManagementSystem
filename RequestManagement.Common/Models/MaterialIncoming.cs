using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfClient.Models;

namespace RequestManagement.Common.Models
{
    public class MaterialIncoming
    {
        public string? WarehouseName { get; set; }
        public List<MaterialIncomingItem> Items { get; set; }
    }

    public class MaterialIncomingItem
    {
        public string RegistratorType { get; set; }
        public string RegistratorNumber { get; set; }
        public string RegistratorDate { get; set; }
        public string ReceiptOrderNumber { get; set; }
        public string ReceiptOrderDate { get; set; }
        public string ApplicationNumber { get; set; }
        public string ApplicationDate { get; set; } 
        public string ApplicationResponsibleName { get; set; }
        public string ApplicationEquipmentName { get; set; }
        public string ApplicationEquipmentCode { get; set; }
        public List<MaterialStock> Items { get; set; }
    }
}
