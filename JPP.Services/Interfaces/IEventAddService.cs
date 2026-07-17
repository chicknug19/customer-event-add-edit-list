using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPP.Models.Event.Responses;
using JPP.Models.Shared.Responses;

namespace JPP.Services.Interfaces
{
    public interface IEventAddService
    {
<<<<<<< HEAD
        Task<BaseResult<int>> AddEventAsync(EventRequestDto request);
=======
        Task<BaseResult<int>> AddEventAsync(EventDto request);
>>>>>>> 99d27c86a2ad63032b8978fefbfac2e9ce4e7788
    }
}
