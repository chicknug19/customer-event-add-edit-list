using JPP.Models.Event.Responses;
using JPP.Models.Event.Request;

namespace JPP.Services.Interfaces
{
    public interface IEventListService
    {
        Task<EventListServiceResult> GetEventListAsync(EventListFilterRequest filter);
        Task<EventListServiceResult> DeleteEventAsync(int id);
    }
}