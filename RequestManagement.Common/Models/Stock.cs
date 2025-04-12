using RequestManagement.Common.Models.Interfaces;

namespace RequestManagement.Common.Models;

public class Stock : IEntity
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!; // Внешний ключ на таблицу Warehouse
    public int NomenclatureId { get; set; }
    public Nomenclature Nomenclature { get; set; } = null!; // Внешний ключ на таблицу Nomenclature
    public decimal InitialQuantity { get; set; } // Начальное количество
    public decimal ReceivedQuantity { get; set; } // Количество поступления
    public decimal ConsumedQuantity { get; set; }
    public decimal FinalQuantity => InitialQuantity + ReceivedQuantity - ConsumedQuantity;
}