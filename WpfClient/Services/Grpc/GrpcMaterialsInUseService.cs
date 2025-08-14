using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using RequestManagement.WpfClient.Services.Interfaces;
using System.Globalization;
using static RequestManagement.WpfClient.Converters.MaterialsInUseConverter;
using MaterialsInUse = RequestManagement.Common.Models.MaterialsInUse;
using MaterialsInUseForUpload = RequestManagement.Common.Models.MaterialsInUseForUpload;

namespace RequestManagement.WpfClient.Services.Grpc;

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

    public Task<List<MaterialsInUse>> GetAllMaterialsInUseForOffAsync(int financiallyResponsiblePersonId, DateTime date)
    {
        throw new NotImplementedException();
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
            MaterialsInUse = new Server.Controllers.MaterialsInUse
            {
                DocumentNumber = materialsInUse.DocumentNumber,
                Date = materialsInUse.Date.ToString(CultureInfo.CurrentCulture),
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
                DateForWriteOff = materialsInUse.DateForWriteOff.ToString("yyyy-MM-dd")
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
            MaterialsInUse = { materialsInUse.Select(x => new Server.Controllers.MaterialsInUseForUpload
            {
                DocumentNumber = x.DocumentNumber,
                Date = x.Date.ToString(CultureInfo.CurrentCulture),
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
            MaterialsInUse = new Server.Controllers.MaterialsInUse
            {
                Id = materialsInUse.Id,
                Date = materialsInUse.Date.ToString(CultureInfo.CurrentCulture),
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
                DateForWriteOff = materialsInUse.DateForWriteOff.ToString("yyyy-MM-dd")
            }
        }, headers);
        return result.Success;
    }

    public Task<bool> UpdateMaterialsInUseAnyAsync(List<MaterialsInUse> materialsInUseAny)
    {
        throw new NotImplementedException();
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
}