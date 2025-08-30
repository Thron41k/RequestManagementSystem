using RequestManagement.Server.Controllers;

namespace RequestManagement.WpfClient.Converters;

public static class MaterialsInUseConverter
{
    public static List<RequestManagement.Common.Models.MaterialsInUse> FromGrpc(IEnumerable<MaterialsInUse>  data)
    {
        return data.Select(proto => new RequestManagement.Common.Models.MaterialsInUse
        {
            Id = proto.Id,
            DocumentNumber = proto.DocumentNumber,
            Date = DateTime.Parse(proto.Date),
            Quantity = (decimal)proto.Quantity,
            NomenclatureId = proto.NomenclatureId,
            Term = proto.Term,
            Nomenclature = proto.Nomenclature is null ? null! : new RequestManagement.Common.Models.Nomenclature
            {
                Id = proto.Nomenclature.Id,
                Code = proto.Nomenclature.Code,
                Name = proto.Nomenclature.Name,
                Article = proto.Nomenclature.Article,
                UnitOfMeasure = proto.Nomenclature.UnitOfMeasure
            },
            EquipmentId = proto.EquipmentId,
            Equipment = new RequestManagement.Common.Models.Equipment
            {
                Id = proto.Equipment.Id,
                Name = proto.Equipment.Name,
                Code = proto.Equipment.Code,
                StateNumber = proto.Equipment.LicensePlate,
                ShortName = proto.Equipment.ShortName,
                EquipmentGroup = proto.Equipment.EquipmentGroup is null ? null : new RequestManagement.Common.Models.EquipmentGroup
                {
                    Id = proto.Equipment.EquipmentGroup.Id,
                    Name = proto.Equipment.EquipmentGroup.Name
                }
            },
            FinanciallyResponsiblePersonId = proto.FinanciallyResponsiblePersonId,
            FinanciallyResponsiblePerson = new RequestManagement.Common.Models.Driver
            {
                Id = proto.FinanciallyResponsiblePerson.Id,
                FullName = proto.FinanciallyResponsiblePerson.FullName,
                ShortName = proto.FinanciallyResponsiblePerson.ShortName,
                Position = proto.FinanciallyResponsiblePerson.Position,
                Code = proto.FinanciallyResponsiblePerson.Code
            },
            IsOut = proto.IsOut,
            DocumentNumberForWriteOff = proto.DocumentNumberForWriteOff,
            ReasonForWriteOffId = proto.MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperationId,
            ReasonForWriteOff = new RequestManagement.Common.Models.ReasonsForWritingOffMaterialsFromOperation
            {
                Id = proto.MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperation.Id,
                Reason = proto.MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperation.Reason
            },
            DateForWriteOff = proto.DateForWriteOff == "" ? DateTime.MinValue : DateTime.Parse(proto.DateForWriteOff)
        }).ToList();
    }
}