using Microsoft.EntityFrameworkCore;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Data;
using System;
using System.Collections.Generic;
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
                .Include(r => r.Items) // Подгружаем связанные наименования
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
                .Include(r => r.Items) // Подгружаем связанные наименования
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

            // Обновляем наименования (примерная логика, может быть расширена)
            existingRequest.Items.Clear();
            existingRequest.Items.AddRange(request.Items);

            await _dbContext.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Получает заявку по идентификатору
        /// </summary>
        public async Task<Request> GetRequestByIdAsync(int requestId)
        {
            return await _dbContext.Requests
                .Include(r => r.Items)      // Подгружаем наименования
                .Include(r => r.Equipment)  // Подгружаем назначение (технику)
                .FirstOrDefaultAsync(r => r.Id == requestId);
        }

        /// <summary>
        /// Получает список всех заявок
        /// </summary>
        public async Task<List<Request>> GetAllRequestsAsync()
        {
            return await _dbContext.Requests
                .Include(r => r.Items)      // Подгружаем наименования
                .Include(r => r.Equipment)  // Подгружаем назначение (технику)
                .ToListAsync();
        }
    }
}