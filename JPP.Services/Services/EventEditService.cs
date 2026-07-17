using JPP.Data.Interfaces;
using JPP.Models.Event.Request;
using JPP.Models.Event.Responses;
using JPP.Models.Shared.Responses;
using JPP.Services.Interfaces;

namespace JPP.Services.Services
{
    public class EventEditService : IEventEditService
    {
        private readonly IEventEditRepository _eventEditRepository;

        public EventEditService(IEventEditRepository eventEditRepository)
        {
            _eventEditRepository = eventEditRepository;
        }

        public async Task<EventDetailViewModel?> BuildEditViewModelAsync(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var eventData = await _eventEditRepository.GetEventByIdAsync(id);

            if (eventData == null)
            {
                return null;
            }

            return new EventDetailViewModel
            {
                Form = new EventRequestDto
                {
                    Id = eventData.Id,
                    Name = eventData.Name,
                    Code = eventData.Code,
                    Description = eventData.Description
                },
                IsReadOnly = false
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
