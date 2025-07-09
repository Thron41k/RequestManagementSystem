using Grpc.Core;
using RequestManagement.Common.Interfaces;
using System.Security.Claims;

namespace RequestManagement.Server.Controllers;

public class ExpenseController(IExpenseService expenseService, ILogger<ExpenseController> logger) : ExpenseService.ExpenseServiceBase
{
    private readonly IExpenseService _expenseService = expenseService ?? throw new ArgumentNullException(nameof(expenseService));
    private readonly ILogger<ExpenseController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public override async Task<GetNomenclatureMapingResponse> GetNomenclatureMaping(
        GetNomenclatureMapingRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }

        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var response = new GetNomenclatureMapingResponse();
        if (value != null)
        {
            var userId = int.Parse(value);
            var mapping = await _expenseService.GetLastNomenclatureDefectMappingAsync(userId, request.NomenclatureId);
            if (mapping != null)
            {
                response.Term = mapping.Term;
                response.Defect = new ExpenseDefect
                {
                    Id = mapping.Defect.Id,
                    Name = mapping.Defect.Name,
                };
            }
        }
        return response;
    }

    public override async Task<GetLastSelectionResponse> GetLastSelection(GetLastSelectionRequest request,
        ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }

        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var response = new GetLastSelectionResponse();
        if (value != null)
        {
            var userId = int.Parse(value);
            var lastSelection = await _expenseService.GetUserLastSelectionAsync(userId);
            if (lastSelection != null)
            {
                if (lastSelection is { DriverId: not null, Driver: not null })
                    response.Driver = new ExpenseDriver
                    {
                        Id = lastSelection.Driver.Id,
                        FullName = lastSelection.Driver.FullName,
                        ShortName = lastSelection.Driver.ShortName,
                        Position = lastSelection.Driver.Position
                    };
                if (lastSelection is { EquipmentId: not null, Equipment: not null })
                    response.Equipment = new ExpenseEquipment
                    {
                        Id = lastSelection.Equipment.Id,
                        Name = lastSelection.Equipment.Name,
                        LicensePlate = lastSelection.Equipment.StateNumber
                    };
            }
            var mapping = await _expenseService.GetLastNomenclatureDefectMappingAsync(userId, request.NomenclatureId);
            if (mapping != null)
            {
                response.Term = mapping.Term;
                response.Defect = new ExpenseDefect
                {
                    Id = mapping.Defect.Id,
                    Name = mapping.Defect.Name,
                };
            }
        }
        return response;
    }

    public override async Task<GetAllExpensesResponse> GetAllExpenses(GetAllExpensesRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }

        _logger.LogInformation("Getting all expenses");

        var expenseList = await _expenseService.GetAllExpensesAsync(request.Filter, request.WarehouseId, request.EquipmentId, request.DriverId, request.DefectId, request.FromDate, request.ToDate);
        var response = new GetAllExpensesResponse();
        response.Expenses.AddRange(expenseList.Select(e => new Expense
        {
            Id = e.Id,
            Code = e.Code,
            Term = e.Term ?? 0,
            Stock = new ExpenseStock
            {
                Id = e.StockId,
                Warehouse = new ExpenseWarehouse
                {
                    Id = e.Stock.Warehouse.Id,
                    Name = e.Stock.Warehouse.Name,
                },
                Nomenclature = new ExpenseNomenclature
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
            Equipment = new ExpenseEquipment
            {
                Id = e.Equipment.Id,
                Name = e.Equipment.Name,
                Code = e.Equipment.Code,
                LicensePlate = e.Equipment.StateNumber
            },
            Driver = new ExpenseDriver
            {
                Id = e.Driver.Id,
                Code = e.Driver.Code,
                FullName = e.Driver.FullName,
                ShortName = e.Driver.ShortName,
                Position = e.Driver.Position
            },
            Defect = new ExpenseDefect
            {
                Id = e.Defect.Id,
                Name = e.Defect.Name,
                DefectGroup = new ExpenseDefectGroup
                {
                    Id = e.Defect.DefectGroupId,
                    Name = e.Defect.DefectGroup.Name
                }
            },
            Date = e.Date.ToString("o") // ISO 8601 format
        }));

        return response;
    }

    public override async Task<CreateExpenseResponse> CreateExpense(CreateExpenseRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }
        var userId = 0;
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (value != null)
        {
            userId = int.Parse(value);
        }
        _logger.LogInformation("Creating new expense");
        var expense = new RequestManagement.Common.Models.Expense
        {
            StockId = request.StockId,
            Quantity = (decimal)request.Quantity,
            EquipmentId = request.EquipmentId,
            DriverId = request.DriverId,
            DefectId = request.DefectId,
            Term = request.Term,
            Date = DateTime.Parse(request.Date)
        };
        var newExpense = await _expenseService.CreateExpenseAsync(expense);
        await _expenseService.SaveUserLastSelectionAsync(userId, request.DriverId, request.EquipmentId);
        await _expenseService.SaveNomenclatureDefectMappingAsync(userId, newExpense.StockId, request.DefectId, request.Term);
        return new CreateExpenseResponse { Id = newExpense.Id };
    }

    public override async Task<UpdateExpenseResponse> UpdateExpense(UpdateExpenseRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }
        _logger.LogInformation("Updating expense with ID: {Id}", request.Id);
        var userId = 0;
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (value != null)
        {
            userId = int.Parse(value);
        }
        var expense = new Common.Models.Expense
        {
            Id = request.Id,
            StockId = request.StockId,
            Quantity = (decimal)request.Quantity,
            EquipmentId = request.EquipmentId,
            DriverId = request.DriverId,
            DefectId = request.DefectId,
            Date = DateTime.Parse(request.Date),
            Term = request.Term
        };

        var success = await _expenseService.UpdateExpenseAsync(expense);
        await _expenseService.SaveUserLastSelectionAsync(userId, request.DriverId, request.EquipmentId);
        await _expenseService.SaveNomenclatureDefectMappingAsync(userId, expense.StockId, request.DefectId, request.Term);
        return new UpdateExpenseResponse { Success = success };
    }

    public override async Task<DeleteExpenseResponse> DeleteExpense(DeleteExpenseRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Deleting expense with ID: {Id}", request.Id);

        var success = await _expenseService.DeleteExpenseAsync(request.Id);
        return new DeleteExpenseResponse { Success = success };
    }
    public override async Task<DeleteExpenseResponse> DeleteExpenses(DeleteExpensesRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Deleting expense with IDs : {Id}", request.Id.ToList());

        var success = await _expenseService.DeleteExpensesAsync(request.Id.ToList());
        return new DeleteExpenseResponse { Success = success };
    }

    public override async Task<UploadMaterialExpenseResponse> UploadMaterialExpense(
        UploadMaterialExpenseRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Uploading expense with excel file");
        var materialExpense =
            request.MaterialExpenses.Select(x => new RequestManagement.Common.Models.MaterialExpense
            {
                Number = x.Number,
                Date = DateTime.Parse(x.Date),
                DriverFullName = x.DriverFullName,
                EquipmentCode = x.EquipmentCode,
                NomenclatureArticle = x.NomenclatureArticle,
                NomenlatureUnitOfMeasure = x.NomenlatureUnitOfMeasure,
                DriverCode = x.DriverCode,
                NomenclatureName = x.NomenclatureName,
                EquipmentName = x.EquipmentName,
                NomenclatureCode = x.NomenclatureCode,
                Quantity = (decimal)x.Quantity,
            }).ToList();
        var success = await _expenseService.UploadMaterialsExpenseAsync(materialExpense, request.WarehouseId);
        return new UploadMaterialExpenseResponse
        {
            Success = success.Item1, MaterialExpenses =
            {
                success.Item2.Select(s => new MaterialExpense
                {
                    Number = s.Number,
                    Date = s.Date.ToString("o"), // ISO 8601 format
                    DriverFullName = s.DriverFullName,
                    EquipmentCode = s.EquipmentCode,
                    NomenclatureArticle = s.NomenclatureArticle,
                    NomenlatureUnitOfMeasure = s.NomenlatureUnitOfMeasure,
                    DriverCode = s.DriverCode,
                    NomenclatureName = s.NomenclatureName,
                    EquipmentName = s.EquipmentName,
                    NomenclatureCode = s.NomenclatureCode,
                    Quantity = (double)s.Quantity
                })
            }
        };
    }
}