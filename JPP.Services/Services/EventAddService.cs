using JPP.Data.Interfaces;
using JPP.Models.Event.Request;
using JPP.Models.Event.Responses;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace JPP.Services.Services
{
    public class EventAddService : IEventAddService
    {
        private readonly IEventAddRepository _eventAddRepo;

        public EventAddService(IEventAddRepository eventAddRepo)
        {
            _eventAddRepo = eventAddRepo;
        }

        public async Task<BaseResult<int>> AddEventAsync(EventRequestDto request)
        {
            try
            {
                if (request == null)
                {
                    return BaseResult<int>.Fail("Invalid event data.", 400);
                }

                if (!string.IsNullOrWhiteSpace(request.Code))
                {
                    bool isCodeExist = await _eventAddRepo.CodeExistsAsync(request.Code);
                    if (isCodeExist)
                    {
                        return BaseResult<int>.Fail($"Event Code '{request.Code}' already registered.", 400);
                    }
                }

                var newId = await _eventAddRepo.CreateEventAsync(new EventDto
                {
                    Id = request.Id,
                    Name = request.Name,
                    Code = request.Code,
                    Description = request.Description,
                    Location = request.Location,
                    DatabaseName = request.DatabaseName,
                    Brand = request.Brand,
                    EventOrganizer = request.EventOrganizer,
                    EventDateTime = request.EventDateTime.Value,
                    Duration = request.Duration.Value

                });

                return BaseResult<int>.Ok(newId, "The event was successfully added.", 200);
            }
            catch (Exception ex)
            {
                return BaseResult<int>.Fail($"A system error occurred: {ex.Message}", 500);
            }
        }
    }
}
