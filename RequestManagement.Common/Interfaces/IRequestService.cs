using RequestManagement.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RequestManagement.Common.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для работы с заявками
    /// </summary>
    public interface IRequestService
    {
        /// <summary>
        /// Создает новую заявку
        /// </summary>
        /// <param name="request">Модель заявки для создания</param>
        /// <returns>Идентификатор созданной заявки</returns>
        Task<int> CreateRequestAsync(Request request);

        /// <summary>
        /// Удаляет заявку по идентификатору
        /// </summary>
        /// <param name="requestId">Идентификатор заявки</param>
        /// <returns>Признак успешного удаления</returns>
        Task<bool> DeleteRequestAsync(int requestId);

        /// <summary>
        /// Обновляет существующую заявку
        /// </summary>
        /// <param name="request">Обновленная модель заявки</param>
        /// <returns>Признак успешного обновления</returns>
        Task<bool> UpdateRequestAsync(Request request);

        /// <summary>
        /// Получает заявку по идентификатору
        /// </summary>
        /// <param name="requestId">Идентификатор заявки</param>
        /// <returns>Модель заявки или null, если заявка не найдена</returns>
        Task<Request> GetRequestByIdAsync(int requestId);

        /// <summary>
        /// Получает список всех заявок
        /// </summary>
        /// <returns>Список заявок</returns>
        Task<List<Request>> GetAllRequestsAsync();
    }
}
