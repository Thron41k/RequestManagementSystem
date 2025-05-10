using OfficeOpenXml;
using System.IO;
using RequestManagement.Common.Models;
using WpfClient.Models;
using WpfClient.Services.Interfaces;
using System;

namespace WpfClient.Services
{
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
                const int startRow = 12;
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
                    var code = worksheet.Cells[row, 6].Value?.ToString()?.Trim() ?? string.Empty;
                    var article = worksheet.Cells[row, 7].Value?.ToString()?.Trim() ?? string.Empty;
                    var unit = worksheet.Cells[row, 8].Value?.ToString()?.Trim() ?? string.Empty;
                    var quantity = worksheet.Cells[row, 9].Value?.ToString()?.Trim() ?? string.Empty;
                    if (code == string.Empty && article == string.Empty && unit == string.Empty &&
                        quantity == string.Empty)
                    {
                        var splited = name.Split(',');
                        if (splited.Length == 0) continue;
                        if (splited[1] == string.Empty && splited[2] == string.Empty && splited[3] == string.Empty &&
                            splited[4] == string.Empty)
                        {
                            expenseNumber = "";
                            driverName = "";
                            driverCode = "";
                            equipmentName = "";
                            equipmentCode = "";
                            continue;
                        }
                        var numberAndDate = splited[0].Split(' ');
                        expenseNumber = numberAndDate[2].Trim();
                        expenseDate = DateTime.Parse(numberAndDate[4]);
                        driverName = splited[1].Trim();
                        driverCode = splited[2].Trim();
                        equipmentName = splited[3].Trim();
                        equipmentCode = splited[4].Trim();
                    }
                    else
                    {
                        if (expenseNumber == string.Empty && driverName == string.Empty && driverCode == string.Empty ||
                            equipmentName == string.Empty && equipmentCode == string.Empty)
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
                        FinalBalance = double.TryParse(worksheet.Cells[row, 9].Value?.ToString()?.Trim(), out var balance) ? balance : 0
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
                materialIncoming.WarehouseName = worksheet.Cells[11, 1].Value.ToString()?.Trim();
                const int startRow = 12;
                MaterialIncomingItem? newMaterialIncoming = null;
                for (var row = startRow; row < rowCount; row++)
                {
                    var name = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? string.Empty;
                    var code = worksheet.Cells[row, 6].Value?.ToString()?.Trim() ?? string.Empty;
                    var article = worksheet.Cells[row, 7].Value?.ToString()?.Trim() ?? string.Empty;
                    var unit = worksheet.Cells[row, 8].Value?.ToString()?.Trim() ?? string.Empty;
                    var quantity = worksheet.Cells[row, 9].Value?.ToString()?.Trim() ?? string.Empty;
                    if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(article) && string.IsNullOrEmpty(unit) &&
                        string.IsNullOrEmpty(quantity))
                    {
                        if (newMaterialIncoming != null)
                        {
                            materialIncoming.Items.Add(newMaterialIncoming);
                        }
                        var splited = name.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        if (splited.Length == 0)
                        {
                            newMaterialIncoming = null;
                            continue;
                        }
                        newMaterialIncoming = new MaterialIncomingItem();
                        var registrator = splited[0].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        newMaterialIncoming.RegistratorType = registrator[0];
                        var index = newMaterialIncoming.RegistratorType == "Перемещение" ? 4 : 2;
                        newMaterialIncoming.RegistratorNumber = registrator[index];
                        newMaterialIncoming.RegistratorDate = registrator[index + 2];
                        if (splited.Length > 1 && !string.IsNullOrEmpty(splited[1]))
                        {
                            var receiptOrderCleared = splited[1];
                            var receiptOrder = receiptOrderCleared.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                            newMaterialIncoming.ReceiptOrderNumber = receiptOrder[2];
                            newMaterialIncoming.ReceiptOrderDate = receiptOrder[4];
                        }
                        if (splited.Length > 2 && !string.IsNullOrEmpty(splited[2]))
                        {
                            var applicationCleared = splited[2];
                            var application = applicationCleared.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                            newMaterialIncoming.ApplicationNumber = application[3];
                            newMaterialIncoming.ApplicationDate = application[5];
                        }
                        if (splited.Length > 3 && !string.IsNullOrEmpty(splited[3]))
                        {
                            newMaterialIncoming.ApplicationResponsibleName = splited[3];
                        }
                        if (splited.Length > 4 && !string.IsNullOrEmpty(splited[4]))
                        {
                            newMaterialIncoming.ApplicationEquipmentName = splited[4];
                        }
                        if (splited.Length > 5 && !string.IsNullOrEmpty(splited[5]))
                        {
                            newMaterialIncoming.ApplicationEquipmentCode = splited[5];
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

                return materialIncoming;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return new MaterialIncoming();
        }
    }
}
