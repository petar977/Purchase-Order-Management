using Po.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Po.Common.Interfaces
{
    public interface ISenderEmail
    {
        void SendEmail(Message message);
        Task SendEmailAsync(Message message);
    }
}
