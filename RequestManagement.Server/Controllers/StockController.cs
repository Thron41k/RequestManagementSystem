using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;

namespace RequestManagement.Server.Controllers
{
    public class StockController(IStockService requestService, ILogger<RequestController> logger) : StockService.StockServiceBase
    {
        private readonly IStockService _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
        private readonly ILogger<RequestController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        public override async Task<GetAllStocksResponse> GetAllStock(GetAllStocksRequest request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            if (user.Identity is { IsAuthenticated: false })
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
            }

            _logger.LogInformation("Getting all stock");

            var stockList = await _requestService.GetAllStocksAsync(
                request.WarehouseId, 
                request.Filter,
                request.InitialQuantityFilterType,
                request.InitialQuantity,
                request.ReceivedQuantityFilterType,
                request.ReceivedQuantity,
                request.ConsumedQuantityFilterType,
                request.ConsumedQuantity,
                request.FinalQuantityFilterType,
                request.FinalQuantity
                );
            var response = new GetAllStocksResponse();
            response.Stocks.AddRange(stockList.Select(e => new Stock
            {
                Id = e.Id,
                WarehouseId = e.WarehouseId,
                NomenclatureId = e.NomenclatureId,
                InitialQuantity = (double)e.InitialQuantity,
                ReceivedQuantity = (double)e.ReceivedQuantity,
                ConsumedQuantity = (double)e.ConsumedQuantity,
                Nomenclature = new StockNomenclature{Code = e.Nomenclature.Code,Article = e.Nomenclature.Article, Name = e.Nomenclature.Name, UnitOfMeasure = e.Nomenclature.UnitOfMeasure},
            }));

            return response;
        }
        public override async Task<CreateStockResponse> CreateStock(CreateStockRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Creating new stock");

            var stock = new Common.Models.Stock
            {
                WarehouseId = request.WarehouseId,
                NomenclatureId = request.NomenclatureId,
                InitialQuantity = (decimal)request.InitialQuantity
            };
            var id = await _requestService.CreateStockAsync(stock);
            return new CreateStockResponse { Id = id };
        }

        public override async Task<UpdateStockResponse> UpdateStock(UpdateStockRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Updating stock");
            var stock = new RequestManagement.Common.Models.Stock
            {
                Id = request.Id,
                NomenclatureId = request.NomenclatureId,
                InitialQuantity = (decimal)request.InitialQuantity
            };
            var success = await requestService.UpdateStockAsync(stock);
            return new UpdateStockResponse { Success = success };
        }

        public override async Task<DeleteStockResponse> DeleteStock(DeleteStockRequest request, ServerCallContext context)
        {
            logger.LogInformation("Deleting stock with ID: {Id}", request.Id);
            var success = await requestService.DeleteStockAsync(request.Id);
            return new DeleteStockResponse { Success = success };
        }
    }
}
