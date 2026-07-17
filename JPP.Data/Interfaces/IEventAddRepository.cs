using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPP.Models.Event.Responses;

namespace JPP.Data.Interfaces
{
    public interface IEventAddRepository
    {
        Task<bool> CodeExistsAsync(string code);
        Task<int> CreateEventAsync(EventRequestDto request);
        Task<int> CreateEventAsync(EventDto request);
    }
}
