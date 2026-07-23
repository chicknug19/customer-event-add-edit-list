using JPP.Data.Interfaces;
using JPP.Models.Event.Request;
using JPP.Models.Event.Responses;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JPP.Services.Services
{
    public class EventEditService : IEventEditService
    {
        private readonly IEventEditRepository _eventEditRepository;

        public EventEditService(IEventEditRepository eventEditRepository)
        {
            _eventEditRepository = eventEditRepository;
        }

        private List<SelectListItem> GenerateDurationList()
{
            var durationList = new List<SelectListItem>();
            for (decimal i = 0.5m; i <= 4.0m; i += 0.5m)
            {
                durationList.Add(new SelectListItem
                {
                    Value = i.ToString("0.0"),
                    Text = $"{i:0.0} hours"
                });
            }
            return durationList;
        }

        public async Task<EventDetailViewModel?> BuildEditViewModelAsync(int id)
{
            if (id <= 0) return null;

            var eventData = await _eventEditRepository.GetEventByIdAsync(id);
            if (eventData == null) return null;

            return new EventDetailViewModel
            {
                Form = new EventRequestDto
                {
                    Id = eventData.Id,
                    Name = eventData.Name,
                    Code = eventData.Code,
                    Location = eventData.Location,
                    DatabaseName = eventData.DatabaseName,
                    Brand = eventData.Brand,
                    EventOrganizer = eventData.EventOrganizer,
                    EventDateTime = eventData.EventDateTime,
                    Duration = eventData.Duration,
                    Description = eventData.Description
                },
                IsReadOnly = false,
                DurationOptions = GenerateDurationList() // <- added
            };
        }

        public async Task<BaseResult<int>> SaveEventAsync(EventRequestDto form)
        {
            if (form == null)
            {
                return BaseResult<int>.Fail("Data event tidak valid.", 400);
            }

            if (form.Id <= 0)
            {
                return BaseResult<int>.Fail("Event ID tidak valid.", 400);
            }

            if (string.IsNullOrWhiteSpace(form.Code))
            {
                return BaseResult<int>.Fail("Kode event wajib diisi.", 400);
            }

            if (string.IsNullOrWhiteSpace(form.Name))
            {
                return BaseResult<int>.Fail("Nama event wajib diisi.", 400);
            }

            if (await _eventEditRepository.CodeExistsAsync(form.Code, form.Id))
            {
                return BaseResult<int>.Fail($"Kode event '{form.Code}' sudah terdaftar.", 400);
            }

            var isUpdated = await _eventEditRepository.UpdateEventAsync(form);

            if (!isUpdated)
            {
                return BaseResult<int>.Fail("Event gagal diperbarui.", 400);
            }

            return BaseResult<int>.Ok(form.Id, "Event berhasil diperbarui.", 200);
        }
    }
}
