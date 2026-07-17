using JPP.Data.Interfaces;
using JPP.Models.Event.Responses;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<BaseResult<int>> AddEventAsync(EventDto request)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(request.Code))
                {
                    bool isCodeExist = await _eventAddRepo.CodeExistsAsync(request.Code);
                    if (isCodeExist)
                    {
                        return BaseResult<int>.Fail($"Event Code '{request.Code}' sudah terdaftar.", 400);
                    }
                }

                var newId = await _eventAddRepo.CreateEventAsync(request);
                return BaseResult<int>.Ok(newId, "Event berhasil ditambahkan.", 200);
            }
            catch (Exception ex)
            {
                return BaseResult<int>.Fail($"Terjadi kesalahan sistem: {ex.Message}", 500);
            }
        }
    }
}
