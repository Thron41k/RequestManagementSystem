using RequestManagement.Server.Controllers;

namespace RequestManagement.Server.Converters;

public static class SparePartsOwnershipConverter
{
    public static GetAllSparePartsOwnershipsResponse MapToGrpcResponse(List<RequestManagement.Common.Models.SparePartsOwnership> ownerships)
    {
        var response = new GetAllSparePartsOwnershipsResponse();
        var nomenclatures = ownerships
            .Select(o => o.Nomenclature)
            .DistinctBy(n => n.Id)
            .Select(n => new SparePartsOwnershipNomenclature
            {
                Id = n.Id,
                Code = n.Code,
                Name = n.Name,
                Article = n.Article,
                UnitOfMeasure = n.UnitOfMeasure
            });
        var ownershipItems = ownerships.Select(o => new SparePartsOwnership
        {
            SparePartsOwnershipId = o.Id,
            EqipmentGroupId = o.EquipmentGroupId,
            NomenclatureId = o.NomenclatureId,
            RequiredQuantity = o.RequiredQuantity,
            CurrentQuantity = o.CurrentQuantity,
            Comment = o.Comment ?? string.Empty,
            AnalogId = o.AnalogId ?? 0
        });
        response.SparePartsOwnership.AddRange(ownershipItems);
        response.Nomenclature.AddRange(nomenclatures);
        return response;
    }
}