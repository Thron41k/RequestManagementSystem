using Grpc.Core;
using RequestManagement.Common.Interfaces;

namespace RequestManagement.Server.Controllers
{
    public class IncomingController(IIncomingService incomingService, ILogger<RequestController> logger) : IncomingService.IncomingServiceBase
    {
        private readonly IIncomingService _incomingService = incomingService ?? throw new ArgumentNullException(nameof(incomingService));
        private readonly ILogger<RequestController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

       

      

        public override async Task<GetAllIncomingsResponse> GetAllIncomings(GetAllIncomingsRequest request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            if (user.Identity is { IsAuthenticated: false })
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
            }

            _logger.LogInformation("Getting all incomings");

            var incomingList = await _incomingService.GetAllIncomingsAsync(request.Filter,request.WarehouseId,request.FromDate,request.ToDate);
            var response = new GetAllIncomingsResponse();
            response.Incoming.AddRange(incomingList.Select(e => new Incoming
            {
                Id = e.Id,
                Stock = new IncomingStock
                {
                    Id = e.StockId,
                    Warehouse = new IncomingWarehouse
                    {
                        Id = e.Stock.Warehouse.Id,
                        Name = e.Stock.Warehouse.Name
                    },
                    Nomenclature = new IncomingNomenclature
                    {
                        Id = e.Stock.Nomenclature.Id,
                        Name = e.Stock.Nomenclature.Name,
                        Code = e.Stock.Nomenclature.Code,
                        UnitOfMeasure = e.Stock.Nomenclature.UnitOfMeasure,
                        Article = e.Stock.Nomenclature.Article
                    },
                    InitialQuantity = (double)e.Stock.InitialQuantity,
                    ReceivedQuantity = (double)e.Stock.ReceivedQuantity,
                    ConsumedQuantity = (double)e.Stock.ConsumedQuantity
                },
                Quantity = (double)e.Quantity,
                Date = e.Date.ToString("o") // ISO 8601 format
            }));

            return response;
        }

        public override async Task<CreateIncomingResponse> CreateIncoming(CreateIncomingRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Creating new incoming");
            var incoming = new RequestManagement.Common.Models.Incoming
            {
                StockId = request.StockId,
                Quantity = (decimal)request.Quantity,
                Date = DateTime.Parse(request.Date)
            };
            var newIncoming = await _incomingService.CreateIncomingAsync(incoming);
            return new CreateIncomingResponse { Id = newIncoming.Id };
        }

        public override async Task<UpdateIncomingResponse> UpdateIncoming(UpdateIncomingRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Updating incoming with ID: {Id}", request.Id);

            var incoming = new RequestManagement.Common.Models.Incoming
            {
                Id = request.Id,
                StockId = request.StockId,
                Quantity = (decimal)request.Quantity,
                Date = DateTime.Parse(request.Date)
            };
            var success = await _incomingService.UpdateIncomingAsync(incoming);
            return new UpdateIncomingResponse { Success = success };
        }

        public override async Task<DeleteIncomingResponse> DeleteIncoming(DeleteIncomingRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Deleting incoming with ID: {Id}", request.Id);

            var success = await _incomingService.DeleteIncomingAsync(request.Id);
            return new DeleteIncomingResponse { Success = success };
        }
        public override async Task<DeleteIncomingResponse> DeleteIncomings(DeleteIncomingsRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Deleting incoming with IDs : {Id}", request.Id.ToList());

            var success = await _incomingService.DeleteIncomingsAsync(request.Id.ToList());
            return new DeleteIncomingResponse { Success = success };
        }
    }
}
