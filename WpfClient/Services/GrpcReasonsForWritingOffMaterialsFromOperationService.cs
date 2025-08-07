using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using RequestManagement.WpfClient.Services.Interfaces;
using ReasonsForWritingOffMaterialsFromOperation = RequestManagement.Common.Models.ReasonsForWritingOffMaterialsFromOperation;

namespace RequestManagement.WpfClient.Services;

public class GrpcReasonsForWritingOffMaterialsFromOperationService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : IReasonsForWritingOffMaterialsFromOperationService
{
    public async Task<List<ReasonsForWritingOffMaterialsFromOperation>> GetAllReasonsForWritingOffMaterialsFromOperationAsync(string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateReasonsForWritingOffMaterialsFromOperationClient();
        var response = await client.GetAllReasonsForWritingOffMaterialsFromOperationAsync(new GetAllReasonsForWritingOffMaterialsFromOperationRequest
        {
            Filter = filter
        }, headers);
        return response.ReasonsForWritingOffMaterialsFromOperation.Select(reasonsForWritingOffMaterialsFromOperation => new ReasonsForWritingOffMaterialsFromOperation
        {
            Id = reasonsForWritingOffMaterialsFromOperation.Id, 
            Reason = reasonsForWritingOffMaterialsFromOperation.Reason
        }).ToList();
    }

    public async Task<int> CreateReasonsForWritingOffMaterialsFromOperationAsync(ReasonsForWritingOffMaterialsFromOperation reasonsForWritingOffMaterialsFromOperation)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateReasonsForWritingOffMaterialsFromOperationClient();
        var result = await client.CreateReasonsForWritingOffMaterialsFromOperationAsync(new CreateReasonsForWritingOffMaterialsFromOperationRequest
        {
            ReasonsForWritingOffMaterialsFromOperation = new RequestManagement.Server.Controllers.ReasonsForWritingOffMaterialsFromOperation
            {
                Reason = reasonsForWritingOffMaterialsFromOperation.Reason
            }
        }, headers);
        return result.Id;
    }

    public async Task<bool> UpdateReasonsForWritingOffMaterialsFromOperationAsync(ReasonsForWritingOffMaterialsFromOperation reasonsForWritingOffMaterialsFromOperation)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateReasonsForWritingOffMaterialsFromOperationClient();
        var result = await client.UpdateReasonsForWritingOffMaterialsFromOperationAsync(new UpdateReasonsForWritingOffMaterialsFromOperationRequest
        {
            ReasonsForWritingOffMaterialsFromOperation = new RequestManagement.Server.Controllers.ReasonsForWritingOffMaterialsFromOperation
            {
                Id = reasonsForWritingOffMaterialsFromOperation.Id, 
                Reason = reasonsForWritingOffMaterialsFromOperation.Reason
            }
        }, headers);
        return result.Success;
    }

    public async Task<bool> DeleteReasonsForWritingOffMaterialsFromOperationAsync(int id)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateReasonsForWritingOffMaterialsFromOperationClient();
        var result = await client.DeleteReasonsForWritingOffMaterialsFromOperationAsync(new DeleteReasonsForWritingOffMaterialsFromOperationRequest { Id = id }, headers);
        return result.Success;
    }
}