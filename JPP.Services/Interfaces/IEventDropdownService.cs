using JPP.Data.Entities;
using JPP.Data.Repositories;
using JPP.Models.Event.Responses;
using JPP.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Services.Interfaces
{
    public interface IEventDropdownService
    {
        Task<IEnumerable<EventDropdownDto>> GetDropdownListAsync();
    }
}
