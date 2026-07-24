using JPP.Models.Event.Request;
using JPP.Services.Interfaces;
using JPP.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JPP.Web.Areas.Event.Controllers
{
    [Area("Event")]
    public class EventListController : BaseController
    {
        protected override bool RequireLogin => true;

        private readonly IEventListService _eventListService;

        public EventListController(IEventListService eventListService)
        {
            _eventListService = eventListService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetEventList([FromQuery] EventListFilterRequest filter)
        {
            try
            {
                var result = await _eventListService.GetEventListAsync(filter);

                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        Success = false,
                        Message = $"Failed to load event data. {ex.Message}",
                        StatusCode = StatusCodes.Status500InternalServerError
                    });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            try
            {
                var result = await _eventListService.DeleteEventAsync(id);

                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        Success = false,
                        Message = $"Failed to delete event. {ex.Message}",
                        StatusCode = StatusCodes.Status500InternalServerError
                    });
            }
        }
    }
}