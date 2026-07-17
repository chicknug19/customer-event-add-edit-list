using JPP.Models.Event.Request;

namespace JPP.Data.Interfaces
{
    public interface IEventEditRepository
    {
        Task<EventRequestDto?> GetEventByIdAsync(int id);
        Task<bool> CodeExistsAsync(string code, int excludeId = 0);
        Task<bool> UpdateEventAsync(EventRequestDto request);
    }
}
