using RequestManagement.Server.Controllers;

namespace RequestManagement.WpfClient.Converters;

public static class SparePartsOwnershipConverter
{
    public static List<RequestManagement.Common.Models.SparePartsOwnership> MapFromGrpcResponse(GetAllSparePartsOwnershipsResponse response)
    {
        // 1. Построим словарь Nomenclature по id
        var nomenclatureMap = response.Nomenclature
            .ToDictionary(n => n.Id, n => new RequestManagement.Common.Models.Nomenclature
            {
                Id = n.Id,
                Code = n.Code,
                Name = n.Name,
                Article = n.Article,
                UnitOfMeasure = n.UnitOfMeasure
            });

        // 2. Собираем список SparePartsOwnership с привязкой к Nomenclature
        var ownerships = response.SparePartsOwnership.Select(o => new RequestManagement.Common.Models.SparePartsOwnership
        {
            Id = o.SparePartsOwnershipId,
            EquipmentGroupId = o.EqipmentGroupId,
            NomenclatureId = o.NomenclatureId,
            RequiredQuantity = o.RequiredQuantity,
            CurrentQuantity = o.CurrentQuantity,
            Comment = string.IsNullOrWhiteSpace(o.Comment) ? null : o.Comment,
            Nomenclature = nomenclatureMap.TryGetValue(o.NomenclatureId, out var nomenclature) ? nomenclature : null!,
            AnalogId = o.AnalogId
        }).ToList();

        return ownerships;
    }

}