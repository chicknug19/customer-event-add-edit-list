using JPP.Data.Entities;
using JPP.Models.Event.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JPP.Data.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<EventDto>> GetAllEventsAsync();
    }
}
