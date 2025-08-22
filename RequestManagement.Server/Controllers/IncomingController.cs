using Azure;
using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using WpfClient.Models;

namespace RequestManagement.Server.Controllers;

public class IncomingController(IIncomingService incomingService, ILogger<IncomingController> logger) : IncomingService.IncomingServiceBase
{
    private readonly IIncomingService _incomingService = incomingService ?? throw new ArgumentNullException(nameof(incomingService));
    private readonly ILogger<IncomingController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public override async Task<UploadMaterialIncomingResponse> UploadMaterialIncoming(UploadMaterialIncomingRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }

        _logger.LogInformation("Upload incomings");
        var result = await _incomingService.UploadIncomingsAsync(new MaterialIncoming
        {
            WarehouseName = request.WarehouseName,
            Items = request.Items.Select(x=>new MaterialIncomingItem
            {
                RegistratorNumber = x.RegistratorNumber,
                RegistratorType = x.RegistratorType,
                RegistratorDate = x.RegistratorDate,
                InWarehouseName = x.InWarehouseName,
                InWarehouseCode = x.InWarehouseCode,
                ReceiptOrderNumber = x.ReceiptOrderNumber,
                ReceiptOrderDate = x.ReceiptOrderDate,
                ApplicationNumber = x.ApplicationNumber,
                ApplicationDate = x.ApplicationDate,
                ApplicationResponsibleName = x.ApplicationResponsibleName,
                ApplicationEquipmentName = x.ApplicationEquipmentName,
                ApplicationEquipmentCode = x.ApplicationEquipmentCode,
                Items = x.Items.Select(c=>new MaterialStock
                {
                    ItemName = c.Name,
                    Code = c.Code,
                    Article = c.Article,
                    Unit = c.Unit,
                    FinalBalance = c.FinalBalance
                }).ToList()
            }).ToList()
        });
        return new UploadMaterialIncomingResponse { Success = result };
    } 

    public override async Task<GetAllIncomingsResponse> GetAllIncomings(GetAllIncomingsRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }

        _logger.LogInformation("Getting all incomings");

        var incomingList = await _incomingService.GetAllIncomingsAsync(request.Filter,request.WarehouseId,request.FromDate,request.ToDate);
        var response = Converters.IncomingConverter.ToGrpc(incomingList);
        foreach (var incoming in incomingList)
        {
            _logger.LogInformation(incoming.Date.ToShortDateString());
        }
        return response;
    }

    public override async Task<CreateIncomingResponse> CreateIncoming(CreateIncomingRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating new incoming");
        var incoming = new RequestManagement.Common.Models.Incoming
        {
            StockId = request.StockId,
            Quantity = (decimal)request.Quantity,
            Date = DateTime.Parse(request.Date),
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

    public override async Task<GetAllIncomingsResponse> FindIncomingById(FindIncomingByIdRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Finding incoming with ID: {Id}", request.Id);
        var incoming = await _incomingService.FindIncomingByIdAsync(request.Id);
        return Converters.IncomingConverter.ToGrpc([incoming]);
    }
}