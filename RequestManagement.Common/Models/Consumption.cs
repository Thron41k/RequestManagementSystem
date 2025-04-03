using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Models
{
    public class Consumption
    {
        public int Id { get; set; }
        public string Number { get; set; }        // Номер
        public DateTime Date { get; set; }        // Дата
        public int WarehouseId { get; set; }      // Склад (внешний ключ)
        public Warehouse Warehouse { get; set; }  // Навигационное свойство
        public int EquipmentId { get; set; }      // Единица техники (внешний ключ)
        public Equipment Equipment { get; set; }  // Навигационное свойство
        public int DriverId { get; set; }         // Водитель (внешний ключ)
        public Driver Driver { get; set; }        // Навигационное свойство
        public List<ConsumptionItem> Items { get; set; } // Элементы расхода
    }
}
