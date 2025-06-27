using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using WpfClient.Services.Interfaces;
using Commissions = RequestManagement.Common.Models.Commissions;
using Driver = RequestManagement.Common.Models.Driver;

namespace WpfClient.Services;

public class GrpcCommissionsService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : ICommissionsService
{
    public async Task<List<Commissions>> GetAllCommissionsAsync(string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateCommissionsClient();
        var response = await client.GetAllCommissionsAsync(new GetAllCommissionsRequest { Filter = filter }, headers);
        return response.Commissions.Select(commissions => new Commissions
        {
            Id = commissions.Id, 
            Name = commissions.Name,
            BranchName = commissions.BranchName,
            ApproveForAct = new Driver
            {
                Id = commissions.ApproveAct.Id, 
                FullName = commissions.ApproveAct.FullName, 
                ShortName = commissions.ApproveAct.ShortName, 
                Position = commissions.ApproveAct.Position
            },
            ApproveForDefectAndLimit = new Driver
            {
                Id = commissions.ApproveDefectAndLimit.Id, 
                FullName = commissions.ApproveDefectAndLimit.FullName, 
                ShortName = commissions.ApproveDefectAndLimit.ShortName, 
                Position = commissions.ApproveDefectAndLimit.Position
            },
            Chairman = new Driver
            {
                Id = commissions.Chairman.Id, 
                FullName = commissions.Chairman.FullName,
                ShortName = commissions.Chairman.ShortName, 
                Position = commissions.Chairman.Position
            },
            Member1 = new Driver
            {
                Id = commissions.Member1.Id,
                FullName = commissions.Member1.FullName,
                ShortName = commissions.Member1.ShortName,
                Position = commissions.Member1.Position
            },
            Member2 = new Driver
            {
                Id = commissions.Member2.Id,
                FullName = commissions.Member2.FullName,
                ShortName = commissions.Member2.ShortName,
                Position = commissions.Member2.Position
            },
            Member3 = new Driver
            {
                Id = commissions.Member3.Id,
                FullName = commissions.Member3.FullName,
                ShortName = commissions.Member3.ShortName,
                Position = commissions.Member3.Position
            },
            Member4 = new Driver
            {
                Id = commissions.Member4.Id,
                FullName = commissions.Member4.FullName,
                ShortName = commissions.Member4.ShortName,
                Position = commissions.Member4.Position
            }
        }).ToList();
    }

    public async Task<int> CreateCommissionsAsync(Commissions commissions)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateCommissionsClient();
        var result = await client.CreateCommissionsAsync(new CreateCommissionsRequest
        {
            Name = commissions.Name,
            BranchName = commissions.BranchName,
            ApproveActId = commissions.ApproveForAct?.Id ?? 0,
            ApproveDefectAndLimitId = commissions.ApproveForDefectAndLimit?.Id ?? 0,
            ChairmanId = commissions.Chairman?.Id ?? 0,
            Member1Id = commissions.Member1?.Id ?? 0,
            Member2Id = commissions.Member2?.Id ?? 0,
            Member3Id = commissions.Member3?.Id ?? 0,
            Member4Id = commissions.Member4?.Id ?? 0
        }, headers);
        return result.Id;
    }

    public async Task<bool> UpdateCommissionsAsync(Commissions commissions)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateCommissionsClient();
        var result = await client.UpdateCommissionsAsync(new UpdateCommissionsRequest
        {
            Id = commissions.Id,
            Name = commissions.Name,
            BranchName = commissions.BranchName,
            ApproveActId = commissions.ApproveForAct?.Id ?? 0,
            ApproveDefectAndLimitId = commissions.ApproveForDefectAndLimit?.Id ?? 0,
            ChairmanId = commissions.Chairman?.Id ?? 0,
            Member1Id = commissions.Member1?.Id ?? 0,
            Member2Id = commissions.Member2?.Id ?? 0,
            Member3Id = commissions.Member3?.Id ?? 0,
            Member4Id = commissions.Member4?.Id ?? 0
        }, headers);
        return result.Success;
    }

    public async Task<bool> DeleteCommissionsAsync(int id)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateCommissionsClient();
        var result = await client.DeleteCommissionsAsync(new DeleteCommissionsRequest { Id = id }, headers);
        return result.Success;
    }
}