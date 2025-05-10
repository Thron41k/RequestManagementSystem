using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Controllers;
using WpfClient.Services.Interfaces;
using Nomenclature = RequestManagement.Common.Models.Nomenclature;

namespace WpfClient.Services
{
    internal class GrpcNomenclatureAnalogService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : INomenclatureAnalogService
    {
        public async Task<List<Nomenclature>> GetAllNomenclatureAnalogsAsync(int filter)
        {
            var headers = new Metadata();
            if (!string.IsNullOrEmpty(tokenStore.GetToken()))
            {
                headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
            }
            var client = clientFactory.CreateNomenclatureAnalogClient();
            var response = await client.GetAllNomenclatureAnalogsAsync(new GetAllNomenclatureAnalogsRequest {Id = filter }, headers);
            return response.Nomenclature.Select(
                nomenclatureAnalog => new Nomenclature
                {
                    Id = nomenclatureAnalog.Id, 
                    Name = nomenclatureAnalog.Name, 
                    Article = nomenclatureAnalog.Article,
                    Code = nomenclatureAnalog.Code
                }).ToList();
        }

        public async Task<int> AddNomenclatureAnalogAsync(NomenclatureAnalog nomenclatureAnalog)
        {
            var headers = new Metadata();
            if (!string.IsNullOrEmpty(tokenStore.GetToken()))
            {
                headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
            }
            var client = clientFactory.CreateNomenclatureAnalogClient();
            var response = await client.AddNomenclatureAnalogAsync(
                new AddNomenclatureAnalogRequest
                {
                    Original = nomenclatureAnalog.OriginalId,
                    Analog = nomenclatureAnalog.AnalogId
                }, headers);
            return response.Id;
        }

        public async Task<bool> DeleteNomenclatureAnalogAsync(int originalId, int analogId)
        {
            var headers = new Metadata();
            if (!string.IsNullOrEmpty(tokenStore.GetToken()))
            {
                headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
            }
            var client = clientFactory.CreateNomenclatureAnalogClient();
            var response = await client.DeleteNomenclatureAnalogAsync(
                new DeleteNomenclatureAnalogRequest
                {
                    Original = originalId,
                    Analog = analogId
                }, headers);
            return response.Success;
        }
    }
}
