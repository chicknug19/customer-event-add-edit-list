using System.Threading.Tasks;
using JPP.Models.Event.Request;
using JPP.Models.Shared.Responses;

namespace JPP.Services.Interfaces
{
    public interface IEventAddService
    {
        Task<BaseResult<int>> AddEventAsync(EventRequestDto request);
    }
}
