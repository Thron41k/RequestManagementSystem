

using System.Globalization;
using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Controllers;
using RequestManagement.WarehouseScan.Services.Interfaces;
using WpfClient.Converters;
using Incoming = RequestManagement.Server.Controllers.Incoming;

namespace RequestManagement.WarehouseScan.Services
{
    internal class GrpcIncomingService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : IIncomingService
    {
        public async Task<List<RequestManagement.Common.Models.Incoming>> GetAllIncomingsAsync(string filter, int requestWarehouseId, string requestFromDate, string requestToDate)
        {
            var headers = new Metadata();
            if (!string.IsNullOrEmpty(tokenStore.GetToken()))
            {
                headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
            }
            var client = clientFactory.CreateIncomingClient();
            var response = await client.GetAllIncomingsAsync(
                new GetAllIncomingsRequest
                {
                    Filter = filter,
                    WarehouseId = requestWarehouseId,
                    FromDate = requestFromDate,
                    ToDate = requestToDate
                }, headers);
            return IncomingConverter.FromGrpc(response).ToList();
        }

        public Task<bool> UploadIncomingsAsync(MaterialIncoming incoming)
        {
            throw new NotImplementedException();
        }

        public async Task<Common.Models.Incoming> FindIncomingByIdAsync(int id)
        {
            var headers = new Metadata();
            if (!string.IsNullOrEmpty(tokenStore.GetToken()))
            {
                headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
            }

            var client = clientFactory.CreateIncomingClient();
            var response = await client.FindIncomingByIdAsync(new FindIncomingByIdRequest { Id = id }, headers);
            var incoming = IncomingConverter.FromGrpc(response).ToList();
            return incoming.Count != 0 ? incoming.First() : new Common.Models.Incoming();
        }

        public async Task<Common.Models.Incoming> CreateIncomingAsync(Common.Models.Incoming incoming)
        {
            var headers = new Metadata();
            if (!string.IsNullOrEmpty(tokenStore.GetToken()))
            {
                headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
            }
            var client = clientFactory.CreateIncomingClient();
            var result = await client.CreateIncomingAsync(
                new CreateIncomingRequest
                {
                    StockId = incoming.StockId,
                    Date = incoming.Date.ToString(CultureInfo.CurrentCulture),
                    Quantity = (double)incoming.Quantity
                }, headers);
            incoming.Id = result.Id;
            return incoming;
        }

        public async Task<bool> UpdateIncomingAsync(Common.Models.Incoming incoming)
        {
            var headers = new Metadata();
            if (!string.IsNullOrEmpty(tokenStore.GetToken()))
            {
                headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
            }
            var client = clientFactory.CreateIncomingClient();
            var result = await client.UpdateIncomingAsync(
                new UpdateIncomingRequest
                {
                    Id = incoming.Id,
                    StockId = incoming.StockId,
                    Date = incoming.Date.ToString(CultureInfo.CurrentCulture),
                    Quantity = (double)incoming.Quantity
                }, headers);
            return result.Success;
        }

        public async Task<bool> DeleteIncomingAsync(int id)
        {
            var headers = new Metadata();
            if (!string.IsNullOrEmpty(tokenStore.GetToken()))
            {
                headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
            }
            var client = clientFactory.CreateIncomingClient();
            var result = await client.DeleteIncomingAsync(new DeleteIncomingRequest { Id = id }, headers);
            return result.Success;
        }

        public async Task<bool> DeleteIncomingsAsync(List<int> requestIds)
        {
            var headers = new Metadata();
            if (!string.IsNullOrEmpty(tokenStore.GetToken()))
            {
                headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
            }
            var client = clientFactory.CreateIncomingClient();
            var result = await client.DeleteIncomingsAsync(new DeleteIncomingsRequest { Id = { requestIds } }, headers);
            return result.Success;
        }
    }
}
