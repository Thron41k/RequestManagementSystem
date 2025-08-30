using RequestManagement.Server.Controllers;

namespace RequestManagement.Server.Converters;

public static class MaterialsInUseConverter
{
    public static GetAllMaterialsInUseResponse ToGrpc(IEnumerable<RequestManagement.Common.Models.MaterialsInUse> materialsInUse)
    {
        var response = new GetAllMaterialsInUseResponse();
        response.MaterialsInUse.AddRange(materialsInUse.Select(ToProto));
        return response;
    }

    private static MaterialsInUse ToProto(RequestManagement.Common.Models.MaterialsInUse entity)
    {
        return new MaterialsInUse
        {
            Id = entity.Id,
            DocumentNumber = entity.DocumentNumber,
            Date = entity.Date.ToString("dd.MM.yyyy"),
            Quantity = (double)entity.Quantity,
            NomenclatureId = entity.NomenclatureId,
            Term = entity.Term,
            Nomenclature = new MaterialsInUseNomenclature
            {
                Id = entity.Nomenclature.Id,
                Code = entity.Nomenclature.Code,
                Name = entity.Nomenclature.Name,
                Article = entity.Nomenclature.Article,
                UnitOfMeasure = entity.Nomenclature.UnitOfMeasure
            },
            EquipmentId = entity.EquipmentId,
            Equipment = new MaterialsInUseEquipment
            {
                Id = entity.Equipment.Id,
                Name = entity.Equipment.Name,
                Code = entity.Equipment.Code,
                LicensePlate = entity.Equipment.StateNumber,
                ShortName = entity.Equipment.ShortName,
                EquipmentGroup = entity.Equipment.EquipmentGroup is null ? null : new MaterialsInUseEquipmentGroup
                {
                    Id = entity.Equipment.EquipmentGroup.Id,
                    Name = entity.Equipment.EquipmentGroup.Name
                }
            },
            FinanciallyResponsiblePersonId = entity.FinanciallyResponsiblePersonId,
            FinanciallyResponsiblePerson = new MaterialsInUseDriver
            {
                Id = entity.FinanciallyResponsiblePerson.Id,
                FullName = entity.FinanciallyResponsiblePerson.FullName,
                ShortName = entity.FinanciallyResponsiblePerson.ShortName,
                Position = entity.FinanciallyResponsiblePerson.Position,
                Code = entity.FinanciallyResponsiblePerson.Code
            },
            IsOut = entity.IsOut,
            MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperationId = entity.ReasonForWriteOff.Id,
            MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperation = new MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperation
            {
                Id = entity.ReasonForWriteOff.Id,
                Reason = entity.ReasonForWriteOff.Reason
            },
            DateForWriteOff = entity.DateForWriteOff.ToString("dd.MM.yyyy"),
            DocumentNumberForWriteOff = entity.DocumentNumberForWriteOff
        };
    }
}