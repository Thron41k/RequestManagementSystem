using System.Globalization;
using Grpc.Core;
using OneCOverlayClient.Services.Interfaces;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using static OneCOverlayClient.Converters.MaterialsInUseConverter;
using MaterialsInUse = RequestManagement.Common.Models.MaterialsInUse;
using MaterialsInUseForUpload = RequestManagement.Common.Models.MaterialsInUseForUpload;

namespace OneCOverlayClient.Services;

public class GrpcMaterialsInUseService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : IMaterialsInUseService
{
    public async Task<List<MaterialsInUse>> GetAllMaterialsInUseAsync(int financiallyResponsiblePersonId, string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateMaterialsInUseClient();
        var response = await client.GetAllMaterialsInUseAsync(new GetAllMaterialsInUseRequest
        {
            FinanciallyResponsiblePersonId = financiallyResponsiblePersonId,
            Filter = filter
        }, headers);
        return FromGrpc(response.MaterialsInUse);
    }

    public async Task<List<MaterialsInUse>> GetAllMaterialsInUseForOffAsync(int financiallyResponsiblePersonId, DateTime date)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateMaterialsInUseClient();
        var response = await client.GetAllMaterialsInUseForOffAsync(new GetAllMaterialsInUseForOffRequest
        {
            FinanciallyResponsiblePersonId = financiallyResponsiblePersonId,
            DateForOff = date.ToString("dd.MM.yyyy")
        }, headers);
        return FromGrpc(response.MaterialsInUse);
    }

    public async Task<int> CreateMaterialsInUseAsync(MaterialsInUse materialsInUse)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateMaterialsInUseClient();
        var result = await client.CreateMaterialsInUseAsync(new CreateMaterialsInUseRequest
        {
            MaterialsInUse = new RequestManagement.Server.Controllers.MaterialsInUse
            {
                DocumentNumber = materialsInUse.DocumentNumber,
                Date = materialsInUse.Date.ToString("dd.MM.yyyy"),
                Quantity = (double)materialsInUse.Quantity,
                NomenclatureId = materialsInUse.NomenclatureId,
                EquipmentId = materialsInUse.EquipmentId,
                FinanciallyResponsiblePersonId = materialsInUse.FinanciallyResponsiblePersonId,
                IsOut = materialsInUse.IsOut,
                MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperationId = materialsInUse.ReasonForWriteOffId,
                MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperation = new MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperation
                {
                    Id = materialsInUse.ReasonForWriteOff.Id,
                    Reason = materialsInUse.ReasonForWriteOff.Reason
                },
                DocumentNumberForWriteOff = materialsInUse.DocumentNumberForWriteOff,
                DateForWriteOff = materialsInUse.DateForWriteOff.ToString("dd.MM.yyyy")
            }
        }, headers);
        return result.Id;
    }

    public async Task<bool> UploadMaterialsInUseAsync(List<MaterialsInUseForUpload> materialsInUse)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateMaterialsInUseClient();
        var result = await client.UploadMaterialsInUseAsync(new UploadMaterialsInUseRequest { 
            MaterialsInUse = { materialsInUse.Select(x => new RequestManagement.Server.Controllers.MaterialsInUseForUpload
            {
                DocumentNumber = x.DocumentNumber,
                Date = x.Date,
                Quantity = (double)x.Quantity,
                NomenclatureCode = x.NomenclatureCode,
                NomenclatureName = x.NomenclatureName,
                NomenclatureUnitOfMeasure = x.NomenclatureUnitOfMeasure,
                EquipmentCode = x.EquipmentCode,
                EquipmentName = x.EquipmentName,
                NomenclatureArticle = x.NomenclatureArticle,
                FinanciallyResponsiblePersonFullName = x.FinanciallyResponsiblePersonFullName,
                FinanciallyResponsiblePersonFullCode = x.FinanciallyResponsiblePersonCode
            })}
        }, headers);
        return result.Success;
    }

    public async Task<bool> UpdateMaterialsInUseAsync(MaterialsInUse materialsInUse)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateMaterialsInUseClient();
        var result = await client.UpdateMaterialsInUseAsync(new UpdateMaterialsInUseRequest
        {
            MaterialsInUse = new RequestManagement.Server.Controllers.MaterialsInUse
            {
                Id = materialsInUse.Id,
                Date = materialsInUse.Date.ToString("dd.MM.yyyy"),
                Quantity = (double)materialsInUse.Quantity,
                DocumentNumber =  materialsInUse.DocumentNumber,
                NomenclatureId = materialsInUse.NomenclatureId,
                EquipmentId = materialsInUse.EquipmentId,
                FinanciallyResponsiblePersonId = materialsInUse.FinanciallyResponsiblePersonId,
                IsOut = materialsInUse.IsOut, 
                MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperationId = materialsInUse.ReasonForWriteOffId,
                MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperation = new MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperation
                {
                    Id = materialsInUse.ReasonForWriteOff.Id,
                    Reason = materialsInUse.ReasonForWriteOff.Reason
                },
                DocumentNumberForWriteOff = materialsInUse.DocumentNumberForWriteOff,
                DateForWriteOff = materialsInUse.DateForWriteOff.ToString("dd.MM.yyyy")
            }
        }, headers);
        return result.Success;
    }

    public async Task<bool> UpdateMaterialsInUseAnyAsync(List<MaterialsInUse> materialsInUseAny)
    {
        var headers = new Metadata();
        var token = tokenStore.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            headers.Add("Authorization", $"Bearer {token}");
        }
        var client = clientFactory.CreateMaterialsInUseClient();
        var grpcMaterials = materialsInUseAny
            .Select(m => new RequestManagement.Server.Controllers.MaterialsInUse
            {
                Id = m.Id,
                Date = m.Date.ToString("dd.MM.yyyy"),
                Quantity = (double)m.Quantity,
                DocumentNumber = m.DocumentNumber ?? string.Empty,
                NomenclatureId = m.NomenclatureId,
                EquipmentId = m.EquipmentId,
                FinanciallyResponsiblePersonId = m.FinanciallyResponsiblePersonId,
                IsOut = m.IsOut,
                MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperationId = m.ReasonForWriteOffId,
                MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperation = new MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperation
                {
                    Id = m.ReasonForWriteOff.Id,
                    Reason = m.ReasonForWriteOff.Reason
                },
                DocumentNumberForWriteOff = m.DocumentNumberForWriteOff,
                DateForWriteOff = m.DateForWriteOff.ToString("dd.MM.yyyy")
            })
            .ToList();

        var request = new UpdateMaterialsInUseAnyRequest
        {
            MaterialsInUse = { grpcMaterials }
        };

        var result = await client.UpdateMaterialsInUseAnyAsync(request, headers);

        return result.Success;
    }


    public async Task<bool> DeleteMaterialsInUseAsync(int id)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateMaterialsInUseClient();
        var result = await client.DeleteMaterialsInUseAsync(new DeleteMaterialsInUseRequest { Id = id }, headers);
        return result.Success;
    }

    public Task<bool> CreateMaterialsInUseAnyAsync(IEnumerable<MaterialsInUse> materialsInUseList)
    {
        throw new NotImplementedException();
    }
}