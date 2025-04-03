using Grpc.Core;
using RequestManagement.Common.Models;
using RequestManagement.Common.Models.Enums;
using RequestManagement.Server.Controllers;

namespace WpfClient.Services
{
    public class GrpcRequestService
    {
        private readonly RequestService.RequestServiceClient _client;

        public GrpcRequestService(RequestService.RequestServiceClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        private Metadata GetAuthHeaders()
        {
            var token = AuthTokenStore.JwtToken;
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("JWT token not found.");
            }
            return new Metadata { { "Authorization", $"Bearer {token}" } };
        }

        public async Task<List<Request>> GetAllRequestsAsync()
        {
            var response = await _client.GetAllRequestsAsync(new GetAllRequestsRequest(), GetAuthHeaders());
            return response.Requests.Select(r => new Request
            {
                Id = r.RequestId,
                Number = r.Number,
                CreationDate = r.CreationDate.ToDateTime(),
                DueDate = r.DueDate.ToDateTime(),
                Comment = r.Comment,
                ExecutionComment = r.ExecutionComment,
                Status = (RequestStatus)r.Status,
                EquipmentId = r.EquipmentId,
                Items = r.Items.Select(i => new RequestManagement.Common.Models.Item
                {
                    Id = i.Id,
                    NomenclatureId = i.NomenclatureId,
                    Quantity = i.Quantity,
                    Note = i.Note,
                    Status = (ItemStatus)i.Status
                }).ToList()
            }).ToList();
        }

        public async Task<Request> GetRequestByIdAsync(int id)
        {
            var request = new GetRequestRequest { RequestId = id };
            var response = await _client.GetRequestAsync(request, GetAuthHeaders());
            return new Request
            {
                Id = response.RequestId,
                Number = response.Number,
                CreationDate = response.CreationDate.ToDateTime(),
                DueDate = response.DueDate.ToDateTime(),
                Comment = response.Comment,
                ExecutionComment = response.ExecutionComment,
                Status = (RequestStatus)response.Status,
                EquipmentId = response.EquipmentId,
                Items = response.Items.Select(i => new RequestManagement.Common.Models.Item
                {
                    Id = i.Id,
                    NomenclatureId = i.NomenclatureId,
                    Quantity = i.Quantity,
                    Note = i.Note,
                    Status = (ItemStatus)i.Status
                }).ToList()
            };
        }

        public async Task CreateRequestAsync(Request request)
        {
            var grpcRequest = new CreateRequestRequest
            {
                Number = request.Number,
                CreationDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(request.CreationDate.ToUniversalTime()),
                DueDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(request.DueDate.ToUniversalTime()),
                Comment = request.Comment ?? "",
                ExecutionComment = request.ExecutionComment ?? "",
                Status = (int)request.Status,
                EquipmentId = request.EquipmentId
            };
            grpcRequest.Items.AddRange(request.Items.Select(i => new RequestManagement.Server.Controllers.Item
            {
                NomenclatureId = i.NomenclatureId,
                Quantity = i.Quantity,
                Note = i.Note ?? "",
                Status = (int)i.Status
            }));

            await _client.CreateRequestAsync(grpcRequest, GetAuthHeaders());
        }

        public async Task UpdateRequestAsync(Request request)
        {
            var grpcRequest = new UpdateRequestRequest
            {
                RequestId = request.Id,
                Number = request.Number,
                CreationDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(request.CreationDate.ToUniversalTime()),
                DueDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(request.DueDate.ToUniversalTime()),
                Comment = request.Comment ?? "",
                ExecutionComment = request.ExecutionComment ?? "",
                Status = (int)request.Status,
                EquipmentId = request.EquipmentId
            };
            grpcRequest.Items.AddRange(request.Items.Select(i => new RequestManagement.Server.Controllers.Item
            {
                Id = i.Id,
                NomenclatureId = i.NomenclatureId,
                Quantity = i.Quantity,
                Note = i.Note ?? "",
                Status = (int)i.Status
            }));

            await _client.UpdateRequestAsync(grpcRequest, GetAuthHeaders());
        }

        public async Task DeleteRequestAsync(int id)
        {
            var request = new DeleteRequestRequest { RequestId = id };
            await _client.DeleteRequestAsync(request, GetAuthHeaders());
        }
    }
}