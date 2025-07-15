using RequestManagement.Common.Models;

namespace RequestManagement.Common.Interfaces
{
    public interface IIncomingService
    {
        Task<Incoming> CreateIncomingAsync(Incoming incoming);
        Task<bool> UpdateIncomingAsync(Incoming expense);
        Task<bool> DeleteIncomingAsync(int id);
        Task<bool> DeleteIncomingsAsync(List<int> requestId);
        Task<List<Incoming>> GetAllIncomingsAsync(string requestFilter, int requestWarehouseId, string requestFromDate, string requestToDate);
        Task<bool> UploadIncomingsAsync(MaterialIncoming incoming);
        Task<Incoming> FindIncomingByIdAsync(int id);
    }
}
