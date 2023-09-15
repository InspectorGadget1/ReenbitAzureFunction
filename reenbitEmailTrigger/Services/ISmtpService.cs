using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace reenbitEmailTrigger.Services
{
    public interface ISmtpService
    {
        void SendEmail(string to, string subject, string body);
    }
}
