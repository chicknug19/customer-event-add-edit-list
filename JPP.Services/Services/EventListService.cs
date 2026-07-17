using JPP.Commons.Extensions;
using JPP.Data.Interfaces;
using JPP.Models.Event.Request;   // Assuming Event namespace
using JPP.Models.Event.Responses; // Assuming Event namespace
using JPP.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace JPP.Services.Services
{
    public class EventListService : IEventListService
    {
        private readonly IEventListRepository _eventListRepository;
        private readonly ILogger<EventListService> _logger;

        public EventListService(
            IEventListRepository eventListRepository,
            ILogger<EventListService> logger)
        {
            _eventListRepository = eventListRepository;
            _logger = logger;
        }

        public async Task<EventListServiceResult> GetEventListAsync(EventListFilterRequest filter)
        {
            try
            {
                filter.NormalizeFilter();

                var data = await _eventListRepository.GetEventListAsync(filter);

                return EventListServiceResult.Ok(data, "Success");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching event list");
                return EventListServiceResult.Fail("An error occurred while fetching events", 500);
            }
        }
    }
}