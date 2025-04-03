using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Common.Models.Enums;
using RequestManagement.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RequestManagement.Server.Services
{
    /// <summary>
    /// Сервис для работы с заявками
    /// </summary>
    public class RequestService : IRequestService
    {
        private readonly ApplicationDbContext _dbContext;

        public RequestService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        /// <summary>
        /// Создает новую заявку
        /// </summary>
        public async Task<int> CreateRequestAsync(Request request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Проверяем, что все NomenclatureId существуют
            foreach (var item in request.Items)
            {
                if (!await _dbContext.Nomenclature.AnyAsync(n => n.Id == item.NomenclatureId))
                    throw new ArgumentException($"Nomenclature with ID {item.NomenclatureId} not found.");
            }

            _dbContext.Requests.Add(request);
            await _dbContext.SaveChangesAsync();
            return request.Id;
        }

        /// <summary>
        /// Удаляет заявку по идентификатору
        /// </summary>
        public async Task<bool> DeleteRequestAsync(int requestId)
        {
            var request = await _dbContext.Requests
                .Include(r => r.Items) // Подгружаем связанные элементы
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
                return false;

            _dbContext.Requests.Remove(request);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Обновляет существующую заявку
        /// </summary>
        public async Task<bool> UpdateRequestAsync(Request request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var existingRequest = await _dbContext.Requests
                .Include(r => r.Items) // Подгружаем связанные элементы
                .FirstOrDefaultAsync(r => r.Id == request.Id);

            if (existingRequest == null)
                return false;

            // Обновляем поля заявки
            existingRequest.Number = request.Number;
            existingRequest.CreationDate = request.CreationDate;
            existingRequest.DueDate = request.DueDate;
            existingRequest.Comment = request.Comment;
            existingRequest.ExecutionComment = request.ExecutionComment;
            existingRequest.Status = request.Status;
            existingRequest.EquipmentId = request.EquipmentId;

            // Обновляем элементы (удаляем старые и добавляем новые)
            _dbContext.Items.RemoveRange(existingRequest.Items);
            foreach (var item in request.Items)
            {
                if (!await _dbContext.Nomenclature.AnyAsync(n => n.Id == item.NomenclatureId))
                    throw new ArgumentException($"Nomenclature with ID {item.NomenclatureId} not found.");
                item.RequestId = existingRequest.Id; // Устанавливаем связь с заявкой
            }
            existingRequest.Items = request.Items.ToList();

            await _dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Получает заявку по идентификатору
        /// </summary>
        public async Task<Request> GetRequestByIdAsync(int requestId)
        {
            return await _dbContext.Requests
                .Include(r => r.Items)
                    .ThenInclude(i => i.Nomenclature) // Подгружаем номенклатуру
                .Include(r => r.Equipment)        // Подгружаем технику
                .FirstOrDefaultAsync(r => r.Id == requestId);
        }

        /// <summary>
        /// Получает список всех заявок
        /// </summary>
        public async Task<List<Request>> GetAllRequestsAsync()
        {
            return await _dbContext.Requests
                .Include(r => r.Items)
                    .ThenInclude(i => i.Nomenclature) // Подгружаем номенклатуру
                .Include(r => r.Equipment)        // Подгружаем технику
                .ToListAsync();
        }

        // Дополнительный метод для примера из Excel (опционально)
        public async Task<int> AddRequestFromExcel()
        {
            var equipment = await _dbContext.Equipments
                .FirstOrDefaultAsync(e => e.StateNumber == "Н 507 СН");
            if (equipment == null)
            {
                equipment = new Equipment { Name = "КАМАЗ 53215-15", StateNumber = "Н 507 СН" };
                _dbContext.Equipments.Add(equipment);
                await _dbContext.SaveChangesAsync();
            }

            var request = new Request
            {
                Number = "БПТР0001043",
                CreationDate = new DateTime(2025, 4, 2, 0, 0, 0, DateTimeKind.Utc),
                DueDate = new DateTime(2025, 4, 14, 0, 0, 0, DateTimeKind.Utc),
                Comment = "АКБ И турбина КАМАЗ 53215-15 г.н. Н 507 СН",
                ExecutionComment = "",
                Status = RequestStatus.Created,
                EquipmentId = equipment.Id,
                Items = new List<Item>
                {
                    new Item
                    {
                        NomenclatureId = 1, // Турбокомпрессор
                        Quantity = 1,
                        Note = "",
                        Status = ItemStatus.Pending
                    },
                    new Item
                    {
                        NomenclatureId = 2, // Аккумулятор 6СТ-190
                        Quantity = 2,
                        Note = "",
                        Status = ItemStatus.Pending
                    }
                }
            };

            _dbContext.Requests.Add(request);
            await _dbContext.SaveChangesAsync();
            return request.Id;
        }
    }
}