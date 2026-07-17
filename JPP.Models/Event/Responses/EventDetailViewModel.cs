using JPP.Models.Event.Request;

namespace JPP.Models.Event.Responses
{
    public class EventDetailViewModel
    {
        public EventRequestDto Form { get; set; } = new();
        public bool IsReadOnly { get; set; }
        public bool IsEditMode => Form.Id > 0;

        public string PageTitle => IsEditMode ? "Edit Event" : "Add New Event";
        public string PageSubtitle => IsEditMode ? "Update event master data" : "Create new event master data";
    }
}
