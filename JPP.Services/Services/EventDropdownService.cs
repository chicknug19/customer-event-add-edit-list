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
    public class EventDropdownService : IEventDropdownService
    {
        private readonly IEventDropdownRepository _eventRepo;

        public EventDropdownService(IEventDropdownRepository eventRepo)
        {
            _eventRepo = eventRepo;
        }

        public async Task<IEnumerable<EventDropdownDto>> GetDropdownListAsync()
        {            
            return await _eventRepo.GetAllEventsAsync();
        }
    }
}
