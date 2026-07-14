using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Services.Interfaces
{
    public interface IAccountEmailService
    {
        Task SendForgotPasswordEmailAsync(string emailAddress, string newPassword);
    }
}
