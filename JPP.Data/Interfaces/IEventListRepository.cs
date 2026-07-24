using JPP.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JPP.Models.Event.Request;
using JPP.Models.Event.Responses;

namespace JPP.Data.Interfaces
{
    public interface IEventListRepository
    {
    Task<List<EventListDto>> GetEventListAsync(EventListFilterRequest filter);
    Task<bool> DeleteEventAsync(int id);
    }
    
}