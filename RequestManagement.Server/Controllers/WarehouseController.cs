using Grpc.Core;
using RequestManagement.Common.Interfaces;

namespace RequestManagement.Server.Controllers
{
    public class WarehouseController(IWarehouseService warehouseService, ILogger<RequestController> logger)
        : WarehouseService.WarehouseServiceBase
    {
        public override async Task<GetAllWarehousesResponse> GetAllWarehouses(GetAllWarehousesRequest request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            if (user.Identity is { IsAuthenticated: false })
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
            }

            logger.LogInformation("Getting all warehouses by filter");

            var warehouseList = await warehouseService.GetAllWarehousesAsync(request.Filter);
            var response = new GetAllWarehousesResponse();
            response.Warehouse.AddRange(warehouseList.Select(e => new Warehouse
            {
                Id = e.Id,
                Name = e.Name,
                LastUpdated = e.LastUpdated.ToString("o")
            }));

            return response;
        }

        public override async Task<GetOrCreateWarehouseResponse> GetOrCreateWarehouse(
            GetOrCreateWarehouseRequest request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            if (user.Identity is { IsAuthenticated: false })
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
            }

            logger.LogInformation("Getting all warehouses by filter");
            var warehouse = await warehouseService.GetOrCreateWarehousesAsync(request.Filter);
            var response = new GetOrCreateWarehouseResponse
            {
                Warehouse = new Warehouse { Id = warehouse.Id, Name = warehouse.Name, LastUpdated = warehouse.LastUpdated.ToString("o") }
            };
            return response;
        }

        public override async Task<CreateWarehouseResponse> CreateWarehouse(CreateWarehouseRequest request, ServerCallContext context)
        {
            logger.LogInformation("Creating new warehouse with name: {Name}", request.Warehouse.Name);

            var warehouse = new RequestManagement.Common.Models.Warehouse
            {
                Name = request.Warehouse.Name,
                LastUpdated = DateTime.Parse(request.Warehouse.LastUpdated)
            };

            var id = await warehouseService.CreateWarehouseAsync(warehouse);
            return new CreateWarehouseResponse { Id = id };
        }

        public override async Task<UpdateWarehouseResponse> UpdateWarehouse(UpdateWarehouseRequest request, ServerCallContext context)
        {
            logger.LogInformation("Updating warehouse with ID: {Id}", request.Warehouse.Id);

            var warehouse = new RequestManagement.Common.Models.Warehouse
            {
                Id = request.Warehouse.Id,
                Name = request.Warehouse.Name,
                LastUpdated = DateTime.Parse(request.Warehouse.LastUpdated)
            };

            var success = await warehouseService.UpdateWarehouseAsync(warehouse);
            return new UpdateWarehouseResponse { Success = success };
        }

        public override async Task<DeleteWarehouseResponse> DeleteWarehouse(DeleteWarehouseRequest request, ServerCallContext context)
        {
            logger.LogInformation("Deleting warehouse with ID: {Id}", request.Id);

            var success = await warehouseService.DeleteWarehouseAsync(request.Id);
            return new DeleteWarehouseResponse { Success = success };
        }
    }
}
