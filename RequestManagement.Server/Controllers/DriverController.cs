using Grpc.Core;
using RequestManagement.Common.Interfaces;

namespace RequestManagement.Server.Controllers;

/// <summary>
/// gRPC-контроллер для работы с оборудованием
/// </summary>
public class DriverController(IDriverService driverService, ILogger<DriverController> logger)
    : DriverService.DriverServiceBase
{
    private readonly IDriverService _requestService = driverService ?? throw new ArgumentNullException(nameof(driverService));
    private readonly ILogger<DriverController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public override async Task<GetAllDriversResponse> GetAllDrivers(GetAllDriversRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }

        _logger.LogInformation("Getting all drivers by filter");

        var equipmentList = await _requestService.GetAllDriversAsync(request.Filter);
        var response = new GetAllDriversResponse();
        response.Drivers.AddRange(equipmentList.Select(e => new Driver
        {
            Id = e.Id,
            FullName = e.FullName,
            ShortName = e.ShortName,
            Position = e.Position,
            Code = e.Code
        }));

        return response;
    }

    public override async Task<CreateDriverResponse> CreateDriver(CreateDriverRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating new driver with full name and position: {Name} - {Position}", request.Driver.FullName, request.Driver.Position);

        var driver = new RequestManagement.Common.Models.Driver
        {
            FullName = request.Driver.FullName,
            ShortName = request.Driver.ShortName,
            Position = request.Driver.Position,
            Code = request.Driver.Code
        };

        var id = await _requestService.CreateDriverAsync(driver);
        return new CreateDriverResponse { Id = id };
    }

    public override async Task<UpdateDriverResponse> UpdateDriver(UpdateDriverRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Updating driver with ID: {Id}", request.Driver.Id);

        var driver = new RequestManagement.Common.Models.Driver
        {
            Id = request.Driver.Id,
            FullName = request.Driver.FullName,
            ShortName = request.Driver.ShortName,
            Position = request.Driver.Position,
            Code = request.Driver.Code
        };

        var success = await _requestService.UpdateDriverAsync(driver);
        return new UpdateDriverResponse { Success = success };
    }

    public override async Task<DeleteDriverResponse> DeleteDriver(DeleteDriverRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Deleting driver with ID: {Id}", request.Id);

        var success = await _requestService.DeleteDriverAsync(request.Id);
        return new DeleteDriverResponse { Success = success };
    }
}