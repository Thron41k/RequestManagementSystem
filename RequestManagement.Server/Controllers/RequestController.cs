using Grpc.Core;
using Microsoft.Extensions.Logging;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using System.Threading.Tasks;
using RequestManagement.Common.Models.Enums;

namespace RequestManagement.Server.Controllers
{
    /// <summary>
    /// gRPC-контроллер для работы с заявками
    /// </summary>
    public class RequestController(IRequestService requestService, ILogger<RequestController> logger)
        : RequestService.RequestServiceBase
    {
        /// <summary>
        /// Создает новую заявку
        /// </summary>
        public override async Task<CreateRequestResponse> CreateRequest(CreateRequestRequest request, ServerCallContext context)
        {
            logger.LogInformation("Creating new request with number: {Number}", request.Number);

            var newRequest = new Request
            {
                Number = request.Number,
                CreationDate = DateTime.SpecifyKind(request.CreationDate.ToDateTime(), DateTimeKind.Utc),
                DueDate = DateTime.SpecifyKind(request.DueDate.ToDateTime(), DateTimeKind.Utc),
                Comment = request.Comment,
                ExecutionComment = request.ExecutionComment,
                Status = (RequestStatus)request.Status,
                EquipmentId = request.EquipmentId
            };

            var requestId = await requestService.CreateRequestAsync(newRequest);
            return new CreateRequestResponse { RequestId = requestId };
        }

        /// <summary>
        /// Удаляет заявку
        /// </summary>
        public override async Task<DeleteRequestResponse> DeleteRequest(DeleteRequestRequest request, ServerCallContext context)
        {
            logger.LogInformation("Deleting request with ID: {RequestId}", request.RequestId);

            var success = await requestService.DeleteRequestAsync(request.RequestId);
            return new DeleteRequestResponse { Success = success };
        }

        /// <summary>
        /// Обновляет заявку
        /// </summary>
        public override async Task<UpdateRequestResponse> UpdateRequest(UpdateRequestRequest request, ServerCallContext context)
        {
            logger.LogInformation("Updating request with ID: {RequestId}", request.RequestId);

            var updatedRequest = new Request
            {
                Id = request.RequestId,
                Number = request.Number,
                CreationDate = DateTime.SpecifyKind(request.CreationDate.ToDateTime(), DateTimeKind.Utc),
                DueDate = DateTime.SpecifyKind(request.DueDate.ToDateTime(), DateTimeKind.Utc),
                Comment = request.Comment,
                ExecutionComment = request.ExecutionComment,
                Status = (RequestStatus)request.Status,
                EquipmentId = request.EquipmentId
            };

            var success = await requestService.UpdateRequestAsync(updatedRequest);
            return new UpdateRequestResponse { Success = success };
        }

        /// <summary>
        /// Получает заявку по ID
        /// </summary>
        public override async Task<GetRequestResponse> GetRequest(GetRequestRequest request, ServerCallContext context)
        {
            logger.LogInformation("Getting request with ID: {RequestId}", request.RequestId);

            var req = await requestService.GetRequestByIdAsync(request.RequestId);
            if (req == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Request with ID {request.RequestId} not found"));
            }

            var response = new GetRequestResponse
            {
                RequestId = req.Id,
                Number = req.Number,
                CreationDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(req.CreationDate),
                DueDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(req.DueDate),
                Comment = req.Comment ?? "",
                ExecutionComment = req.ExecutionComment ?? "",
                Status = (int)req.Status,
                EquipmentId = req.EquipmentId
            };

            return response;
        }

        /// <summary>
        /// Получает все заявки
        /// </summary>
        public override async Task<GetAllRequestsResponse> GetAllRequests(GetAllRequestsRequest request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            if (!user.Identity.IsAuthenticated)
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
            }

            logger.LogInformation("Getting all requests");

            var requests = await requestService.GetAllRequestsAsync();
            var response = new GetAllRequestsResponse();

            foreach (var req in requests)
            {
                response.Requests.Add(new GetRequestResponse
                {
                    RequestId = req.Id,
                    Number = req.Number,
                    CreationDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(req.CreationDate),
                    DueDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(req.DueDate),
                    Comment = req.Comment ?? "",
                    ExecutionComment = req.ExecutionComment ?? "",
                    Status = (int)req.Status,
                    EquipmentId = req.EquipmentId
                });
            }

            return response;
        }
    }
}
