using Grpc.Core;
using Microsoft.Extensions.Logging;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Services;

namespace RequestManagement.Server.Controllers
{
    public class ExpenseController(IExpenseService expenseService, ILogger<RequestController> logger) : ExpenseService.ExpenseServiceBase
    {
        private readonly IExpenseService _expenseService = expenseService ?? throw new ArgumentNullException(nameof(expenseService));
        private readonly ILogger<RequestController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        public override async Task<GetAllExpensesResponse> GetAllExpenses(GetAllExpensesRequest request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            if (user.Identity is { IsAuthenticated: false })
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
            }

            _logger.LogInformation("Getting all expenses");

            var expenseList = await _expenseService.GetAllExpensesAsync(request.Filter);
            var response = new GetAllExpensesResponse();
            response.Expenses.AddRange(expenseList.Select(e => new Expense
            {
                Id = e.Id,
                Stock = new ExpenseStock
                {
                    Id = e.StockId,
                    Warehouse = new ExpenseWarehouse
                    {
                        Id = e.Stock.Warehouse.Id,
                        Name = e.Stock.Warehouse.Name
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
                    LicensePlate = e.Equipment.StateNumber
                },
                Driver = new ExpenseDriver
                {
                    Id = e.Driver.Id,
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
            _logger.LogInformation("Creating new expense");

            var expense = new RequestManagement.Common.Models.Expense
            {
                StockId = request.StockId,
                Quantity = (decimal)request.Quantity,
                EquipmentId = request.EquipmentId,
                DriverId = request.DriverId,
                DefectId = request.DefectId,
                Date = DateTime.Parse(request.Date)
            };

            var id = await _expenseService.CreateExpenseAsync(expense);
            return new CreateExpenseResponse { Id = id };
        }

        public override async Task<UpdateExpenseResponse> UpdateExpense(UpdateExpenseRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Updating expense with ID: {Id}", request.Id);

            var expense = new RequestManagement.Common.Models.Expense
            {
                Id = request.Id,
                StockId = request.StockId,
                Quantity = (decimal)request.Quantity,
                EquipmentId = request.EquipmentId,
                DriverId = request.DriverId,
                DefectId = request.DefectId,
                Date = DateTime.Parse(request.Date)
            };

            var success = await _expenseService.UpdateExpenseAsync(expense);
            return new UpdateExpenseResponse { Success = success };
        }

        public override async Task<DeleteExpenseResponse> DeleteExpense(DeleteExpenseRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Deleting expense with ID: {Id}", request.Id);

            var success = await _expenseService.DeleteExpenseAsync(request.Id);
            return new DeleteExpenseResponse { Success = success };
        }
    }
}
