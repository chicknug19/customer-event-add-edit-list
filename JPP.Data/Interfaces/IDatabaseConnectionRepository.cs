using JPP.Models.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Data.Interfaces
{
    public interface IDatabaseConnectionRepository
    {
        Task<BaseResult> CheckAiConnectionAsync();
        Task<BaseResult> CheckCrmConnectionAsync();
        Task<BaseResult> CheckAllConnectionsAsync();
    }
}
