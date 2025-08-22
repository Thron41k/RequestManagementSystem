

using RequestManagement.Server.Controllers;

namespace RequestManagement.Server.Converters;

public static class IncomingConverter
{
    public static GetAllIncomingsResponse ToGrpc(List<RequestManagement.Common.Models.Incoming> incomings)
    {
        var response = new GetAllIncomingsResponse();

        var warehouseSet = new Dictionary<int, IncomingWarehouse>();
        var nomenclatureSet = new Dictionary<int, IncomingNomenclature>();
        var stockSet = new Dictionary<int, IncomingStock>();
        var applicationSet = new Dictionary<int, IncomingApplication>();
        var driverSet = new Dictionary<int, IncomingDriver>();
        var equipmentSet = new Dictionary<int, IncomingEquipment>();

        foreach (var inc in incomings)
        {
            response.Incoming.Add(new Incoming
            {
                Id = inc.Id,
                StockId = inc.StockId,
                Quantity = (double)inc.Quantity,
                Date = inc.Date.ToString("dd.MM.yyyy"),
                Code = inc.Code,
                DocType = inc.DocType,
                ApplicationId = inc.ApplicationId,
                InWarehouseId = inc.InWarehouseId ?? 0
            });

            if (inc.Stock != null && !stockSet.ContainsKey(inc.Stock.Id))
            {
                stockSet[inc.Stock.Id] = new IncomingStock
                {
                    Id = inc.Stock.Id,
                    WarehouseId = inc.Stock.WarehouseId,
                    InitialQuantity = (double)inc.Stock.InitialQuantity,
                    ReceivedQuantity = (double)inc.Stock.ReceivedQuantity,
                    ConsumedQuantity = (double)inc.Stock.ConsumedQuantity,
                    NomenclatureId = inc.Stock.NomenclatureId
                };
            }

            if (inc.Stock?.Warehouse != null && !warehouseSet.ContainsKey(inc.Stock.Warehouse.Id))
            {
                warehouseSet[inc.Stock.Warehouse.Id] = new IncomingWarehouse
                {
                    Id = inc.Stock.Warehouse.Id,
                    Name = inc.Stock.Warehouse.Name,
                    Code = inc.Stock.Warehouse.Code,
                    FinanciallyResponsiblePersonId = inc.Stock.Warehouse.FinanciallyResponsiblePerson?.Id ?? 0
                };
            }
            if (inc.InWarehouse != null && !warehouseSet.ContainsKey(inc.InWarehouse.Id))
            {
                warehouseSet[inc.InWarehouse.Id] = new IncomingWarehouse
                {
                    Id = inc.InWarehouse.Id,
                    Name = inc.InWarehouse.Name,
                    Code = inc.InWarehouse.Code,
                    FinanciallyResponsiblePersonId = inc.InWarehouse?.FinanciallyResponsiblePerson?.Id ?? 0
                };
            }
            if (inc.Stock?.Nomenclature != null && !nomenclatureSet.ContainsKey(inc.Stock.Nomenclature.Id))
            {
                nomenclatureSet[inc.Stock.Nomenclature.Id] = new IncomingNomenclature
                {
                    Id = inc.Stock.Nomenclature.Id,
                    Code = inc.Stock.Nomenclature.Code,
                    Name = inc.Stock.Nomenclature.Name,
                    Article = inc.Stock.Nomenclature.Article ?? string.Empty,
                    UnitOfMeasure = inc.Stock.Nomenclature.UnitOfMeasure
                };
            }

            if (inc.Application != null && !applicationSet.ContainsKey(inc.Application.Id))
            {
                applicationSet[inc.Application.Id] = new IncomingApplication
                {
                    Id = inc.Application.Id,
                    Number = inc.Application.Number,
                    Date = inc.Application.Date.ToString("dd.MM.yyyy"),
                    ResponsibleId = inc.Application.ResponsibleId,
                    EquipmentId = inc.Application.EquipmentId
                };
            }

            if (inc.Application?.Responsible != null && !driverSet.ContainsKey(inc.Application.Responsible.Id))
            {
                driverSet[inc.Application.Responsible.Id] = new IncomingDriver
                {
                    Id = inc.Application.Responsible.Id,
                    FullName = inc.Application.Responsible.FullName,
                    ShortName = inc.Application.Responsible.ShortName,
                    Position = inc.Application.Responsible.Position,
                    Code = inc.Application.Responsible.Code
                };
            }

            if (inc.InWarehouse is { FinanciallyResponsiblePerson: not null } && !driverSet.ContainsKey(inc.InWarehouse.FinanciallyResponsiblePerson.Id))
            {
                driverSet[inc.InWarehouse.FinanciallyResponsiblePerson.Id] = new IncomingDriver
                {
                    Id = inc.InWarehouse.FinanciallyResponsiblePerson.Id,
                    FullName = inc.InWarehouse.FinanciallyResponsiblePerson.FullName,
                    ShortName = inc.InWarehouse.FinanciallyResponsiblePerson.ShortName,
                    Position = inc.InWarehouse.FinanciallyResponsiblePerson.Position,
                    Code = inc.InWarehouse.FinanciallyResponsiblePerson.Code
                };
            }

            if (inc.Stock is { Warehouse.FinanciallyResponsiblePerson: not null } && !driverSet.ContainsKey(inc.Stock.Warehouse.FinanciallyResponsiblePerson.Id))
            {
                driverSet[inc.Stock.Warehouse.FinanciallyResponsiblePerson.Id] = new IncomingDriver
                {
                    Id = inc.Stock.Warehouse.FinanciallyResponsiblePerson.Id,
                    FullName = inc.Stock.Warehouse.FinanciallyResponsiblePerson.FullName,
                    ShortName = inc.Stock.Warehouse.FinanciallyResponsiblePerson.ShortName,
                    Position = inc.Stock.Warehouse.FinanciallyResponsiblePerson.Position,
                    Code = inc.Stock.Warehouse.FinanciallyResponsiblePerson.Code
                };
            }

            if (inc.Application?.Equipment != null && !equipmentSet.ContainsKey(inc.Application.Equipment.Id))
            {
                equipmentSet[inc.Application.Equipment.Id] = new IncomingEquipment
                {
                    Id = inc.Application.Equipment.Id,
                    Name = inc.Application.Equipment.Name,
                    LicensePlate = inc.Application.Equipment.StateNumber ?? string.Empty,
                    Code = inc.Application.Equipment.Code,
                    ShortName = inc.Application.Equipment.ShortName ?? string.Empty
                };
            }
        }

        response.IncomingStock.AddRange(stockSet.Values);
        response.IncomingWarehouse.AddRange(warehouseSet.Values);
        response.IncomingNomenclature.AddRange(nomenclatureSet.Values);
        response.IncomingApplication.AddRange(applicationSet.Values);
        response.IncomingDriver.AddRange(driverSet.Values);
        response.IncomingEquipment.AddRange(equipmentSet.Values);

        return response;
    }
}