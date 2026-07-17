using JPP.Models.Event.Request;
using JPP.Models.Event.Responses;
using JPP.Models.Shared.Responses;

namespace JPP.Services.Interfaces
{
    public interface IEventEditService
    {
        Task<EventDetailViewModel?> BuildEditViewModelAsync(int id);
        Task<BaseResult<int>> SaveEventAsync(EventRequestDto form);
    }
}
