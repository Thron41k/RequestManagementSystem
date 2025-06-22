using OfficeOpenXml;
using System.IO;
using RequestManagement.Common.Models;
using WpfClient.Models;
using WpfClient.Services.Interfaces;

namespace WpfClient.Services;

public class ExcelReaderService : IExcelReaderService
{
    public ExcelReaderService()
    {
        ExcelPackage.License.SetNonCommercialPersonal("Thron41k");
    }

    public (List<MaterialExpense> materialStocks, string? warehouse) ReadExpenses(string filePath)
    {
        var materialExpenses = new List<MaterialExpense>();
        var warehouse = "";
        try
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Excel file not found.", filePath);
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension?.Rows + 2 ?? 0;
            const int startRow = 13;
            if(worksheet.Cells[startRow - 1, 1].Value == null) return (materialExpenses, warehouse);
            warehouse = worksheet.Cells[startRow - 1, 1].Value.ToString()?.Trim();
            var expenseNumber = "";
            var expenseDate = DateTime.Now;
            var driverName = "";
            var driverCode = "";
            var equipmentName = "";
            var equipmentCode = "";
            for (var row = startRow; row < rowCount; row++)
            {
                var name = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? string.Empty;
                var code = worksheet.Cells[row, 10].Value?.ToString()?.Trim() ?? string.Empty;
                var article = worksheet.Cells[row, 11].Value?.ToString()?.Trim() ?? string.Empty;
                var unit = worksheet.Cells[row, 12].Value?.ToString()?.Trim() ?? string.Empty;
                var quantity = worksheet.Cells[row, 13].Value?.ToString()?.Trim() ?? string.Empty;
                if (name == "Штуцер топливной трубки угловой d-12мм")
                {
                    Console.WriteLine(name);
                }
                if (code == string.Empty && article == string.Empty && unit == string.Empty)
                {
                    var numberAndDate = name.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    expenseNumber = numberAndDate[2];
                    expenseDate = DateTime.Parse(numberAndDate[4]);
                    driverName = worksheet.Cells[row, 4].Value?.ToString()?.Trim() ?? string.Empty;
                    driverCode = worksheet.Cells[row, 7].Value?.ToString()?.Trim() ?? string.Empty;
                    equipmentName = worksheet.Cells[row, 8].Value?.ToString()?.Trim() ?? string.Empty;
                    equipmentCode = worksheet.Cells[row, 9].Value?.ToString()?.Trim() ?? string.Empty;
                }
                else
                {
                    if (expenseNumber == string.Empty)
                        continue;
                    materialExpenses.Add(new MaterialExpense
                    {
                        Number = expenseNumber,
                        Date = expenseDate,
                        DriverFullName = driverName,
                        DriverCode = driverCode,
                        EquipmentName = equipmentName,
                        EquipmentCode = equipmentCode,
                        NomenclatureName = name,
                        NomenclatureCode = code,
                        NomenclatureArticle = article,
                        NomenlatureUnitOfMeasure = unit,
                        Quantity = decimal.Parse(quantity)

                    });
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error reading Excel file", ex);
        }


        return (materialExpenses, warehouse);
    }
    public (List<MaterialStock> materialStocks, string? date, string? warehouse) ReadMaterialStock(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Excel file not found.", filePath);
            var materialStocks = new List<MaterialStock>();
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension?.Rows + 2 ?? 0;
            const int startRow = 11;
            var date = worksheet.Cells[4, 3].Value.ToString()?.Split('-')[1].Trim();
            var warehouse = worksheet.Cells[10, 1].Value.ToString()?.Trim();
            for (var row = startRow; row < rowCount; row++)
            {
                if (worksheet.Cells[row, 1].Value == null && worksheet.Cells[row, 2].Value == null)
                    continue;
                try
                {
                    var material = new MaterialStock
                    {
                        ItemName = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? string.Empty,
                        Code = worksheet.Cells[row, 4].Value?.ToString()?.Trim() ?? string.Empty,
                        Article = worksheet.Cells[row, 7].Value?.ToString()?.Trim() ?? string.Empty,
                        Unit = worksheet.Cells[row, 8].Value?.ToString()?.Trim() ?? string.Empty,
                        FinalBalance =
                            double.TryParse(worksheet.Cells[row, 9].Value?.ToString()?.Trim(), out var balance)
                                ? balance
                                : 0
                    };
                    materialStocks.Add(material);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing row {row}: {ex.Message}");
                }
            }

            return (materialStocks, date, warehouse);
        }
        catch (Exception ex)
        {
            return ([], null, null);
        }
    }

    public MaterialIncoming ReadMaterialIncoming(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Excel file not found.", filePath);
            var materialIncoming = new MaterialIncoming
            {
                Items = []
            };
            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets[0];
            var rowCount = worksheet.Dimension?.Rows + 2 ?? 0;
            const int startRow = 13;
            if(worksheet.Cells[startRow - 1, 1].Value == null)return new MaterialIncoming();
            materialIncoming.WarehouseName = worksheet.Cells[startRow - 1, 1].Value.ToString()?.Trim();
            MaterialIncomingItem? newMaterialIncoming = null;
            for (var row = startRow; row < rowCount; row++)
            {
                var name = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? string.Empty;
                var code = worksheet.Cells[row, 11].Value?.ToString()?.Trim() ?? string.Empty;
                var article = worksheet.Cells[row, 12].Value?.ToString()?.Trim() ?? string.Empty;
                var unit = worksheet.Cells[row, 13].Value?.ToString()?.Trim() ?? string.Empty;
                var quantity = worksheet.Cells[row, 14].Value?.ToString()?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(article) && string.IsNullOrEmpty(unit))
                {
                    if (newMaterialIncoming != null)
                    {
                        materialIncoming.Items.Add(newMaterialIncoming);
                    }
                    if (string.IsNullOrEmpty(name))
                    {
                        newMaterialIncoming = null;
                        continue;
                    }
                    newMaterialIncoming = new MaterialIncomingItem();
                    var registrator = name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    newMaterialIncoming.RegistratorType = registrator[0];
                    var index = newMaterialIncoming.RegistratorType == "Перемещение" ? 4 : 2;
                    newMaterialIncoming.RegistratorNumber = registrator[index];
                    newMaterialIncoming.RegistratorDate = registrator[index + 2];
                    var receiptOrderCleared = worksheet.Cells[row, 4].Value?.ToString()?.Trim() ?? string.Empty;
                    if (!string.IsNullOrEmpty(receiptOrderCleared))
                    {
                        var receiptOrder = receiptOrderCleared.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        newMaterialIncoming.ReceiptOrderNumber = receiptOrder[2];
                        newMaterialIncoming.ReceiptOrderDate = receiptOrder[4];
                    }
                    var applicationCleared = worksheet.Cells[row, 7].Value?.ToString()?.Trim() ?? string.Empty;
                    if (!string.IsNullOrEmpty(applicationCleared))
                    {
                        var application = applicationCleared.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        newMaterialIncoming.ApplicationNumber = application[3];
                        newMaterialIncoming.ApplicationDate = application[5];
                    }
                    var applicationResponsibleName = worksheet.Cells[row, 8].Value?.ToString()?.Trim() ?? string.Empty;
                    if (!string.IsNullOrEmpty(applicationResponsibleName))
                    {
                        newMaterialIncoming.ApplicationResponsibleName = applicationResponsibleName;
                    }
                    var applicationEquipmentName = worksheet.Cells[row, 9].Value?.ToString()?.Trim() ?? string.Empty;
                    if (!string.IsNullOrEmpty(applicationEquipmentName))
                    {
                        newMaterialIncoming.ApplicationEquipmentName = applicationEquipmentName;
                    }
                    var applicationEquipmentCode = worksheet.Cells[row, 10].Value?.ToString()?.Trim() ?? string.Empty;
                    if (!string.IsNullOrEmpty(applicationEquipmentCode))
                    {
                        newMaterialIncoming.ApplicationEquipmentCode = applicationEquipmentCode;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(newMaterialIncoming?.RegistratorType) && string.IsNullOrEmpty(newMaterialIncoming?.RegistratorNumber) && string.IsNullOrEmpty(newMaterialIncoming?.RegistratorDate)) continue;
                    newMaterialIncoming.Items ??= [];
                    newMaterialIncoming.Items.Add(new MaterialStock
                    {
                        ItemName = name,
                        Code = code,
                        Article = article,
                        Unit = unit,
                        FinalBalance = double.TryParse(quantity, out var balance) ? balance : 0
                    });
                }
            }
            if (newMaterialIncoming != null)
            {
                materialIncoming.Items.Add(newMaterialIncoming);
            }
            return materialIncoming;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return new MaterialIncoming();
    }
}