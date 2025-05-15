

using RequestManagement.Server.Controllers;

namespace WpfClient.Converters
{
    public static class IncomingConverter
    {
        public static IEnumerable<RequestManagement.Common.Models.Incoming> FromGrpc(GetAllIncomingsResponse response)
        {
            var incomings = new List<RequestManagement.Common.Models.Incoming>();
            var stocksDict = response.IncomingStock.ToDictionary(x => x.Id);
            var warehousesDict = response.IncomingWarehouse.ToDictionary(x => x.Id);
            var nomenclaturesDict = response.IncomingNomenclature.ToDictionary(x => x.Id);
            var applicationsDict = response.IncomingApplication.ToDictionary(x => x.Id);
            var driversDict = response.IncomingDriver.ToDictionary(x => x.Id);
            var equipmentsDict = response.IncomingEquipment.ToDictionary(x => x.Id);
            foreach (var incomingProto in response.Incoming)
            {
                var incoming = new RequestManagement.Common.Models.Incoming
                {
                    Id = incomingProto.Id,
                    StockId = incomingProto.StockId,
                    Quantity = (decimal)incomingProto.Quantity,
                    Date = DateTime.Parse(incomingProto.Date),
                    Code = incomingProto.Code,
                    DocType = incomingProto.DocType,
                    ApplicationId = incomingProto.ApplicationId
                };
                if (stocksDict.TryGetValue(incomingProto.StockId, out var stockProto))
                {
                    incoming.Stock = new RequestManagement.Common.Models.Stock
                    {
                        Id = stockProto.Id,
                        WarehouseId = stockProto.WarehouseId,
                        InitialQuantity = (decimal)stockProto.InitialQuantity,
                        ReceivedQuantity = (decimal)stockProto.ReceivedQuantity,
                        ConsumedQuantity = (decimal)stockProto.ConsumedQuantity,
                        NomenclatureId = stockProto.NomenclatureId
                    };
                    if (warehousesDict.TryGetValue(stockProto.WarehouseId, out var warehouseProto))
                    {
                        incoming.Stock.Warehouse = new RequestManagement.Common.Models.Warehouse
                        {
                            Id = warehouseProto.Id,
                            Name = warehouseProto.Name,
                            Code = warehouseProto.Code
                        };
                    }
                    if (nomenclaturesDict.TryGetValue(stockProto.NomenclatureId, out var nomenclatureProto))
                    {
                        incoming.Stock.Nomenclature = new RequestManagement.Common.Models.Nomenclature
                        {
                            Id = nomenclatureProto.Id,
                            Code = nomenclatureProto.Code,
                            Name = nomenclatureProto.Name,
                            Article = nomenclatureProto.Article,
                            UnitOfMeasure = nomenclatureProto.UnitOfMeasure
                        };
                    }
                }
                if (incomingProto.ApplicationId > 0 && applicationsDict.TryGetValue(incomingProto.ApplicationId, out var applicationProto))
                {
                    incoming.Application = new RequestManagement.Common.Models.Application
                    {
                        Id = applicationProto.Id,
                        Number = applicationProto.Number,
                        Date = DateTime.Parse(applicationProto.Date),
                        ResponsibleId = applicationProto.ResponsibleId,
                        EquipmentId = applicationProto.EquipmentId
                    };
                    if (applicationProto.ResponsibleId > 0 && driversDict.TryGetValue(applicationProto.ResponsibleId, out var driverProto))
                    {
                        incoming.Application.Responsible = new RequestManagement.Common.Models.Driver
                        {
                            Id = driverProto.Id,
                            FullName = driverProto.FullName,
                            ShortName = driverProto.ShortName,
                            Position = driverProto.Position,
                            Code = driverProto.Code
                        };
                    }
                    if (applicationProto.EquipmentId > 0 && equipmentsDict.TryGetValue(applicationProto.EquipmentId, out var equipmentProto))
                    {
                        incoming.Application.Equipment = new RequestManagement.Common.Models.Equipment
                        {
                            Id = equipmentProto.Id,
                            Name = equipmentProto.Name,
                            StateNumber = equipmentProto.LicensePlate,
                            Code = equipmentProto.Code
                        };
                    }
                }

                incomings.Add(incoming);
            }

            return incomings;
        }
    }

}
