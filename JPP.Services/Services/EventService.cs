using JPP.Data.Entities;
using JPP.Data.Interfaces;
using JPP.Models.Event.Responses;
using JPP.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Services.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepo;

        public EventService(IEventRepository eventRepo)
        {
            _eventRepo = eventRepo;
        }

        public async Task<IEnumerable<EventDto>> GetDropdownListAsync()
        {            
            return await _eventRepo.GetAllEventsAsync();
        }
    }
}
