using RequestManagement.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RequestManagement.Common.Interfaces
{
    /// <summary>
    /// Интерфейс для работы с заявками
    /// </summary>
    public interface IRequestService
    {
        /// <summary>
        /// Создает новую заявку
        /// </summary>
        Task<int> CreateRequestAsync(Request request);

        /// <summary>
        /// Удаляет заявку по идентификатору
        /// </summary>
        Task<bool> DeleteRequestAsync(int requestId);

        /// <summary>
        /// Обновляет существующую заявку
        /// </summary>
        Task<bool> UpdateRequestAsync(Request request);

        /// <summary>
        /// Получает заявку по идентификатору
        /// </summary>
        Task<Request> GetRequestByIdAsync(int requestId);

        /// <summary>
        /// Получает список всех заявок
        /// </summary>
        Task<List<Request>> GetAllRequestsAsync();
    }
}