using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NewsletterX.Core.Domain.Model;

namespace NewsletterX.Core.Domain.Service
{
    public interface IEmailService
    {
        Task<bool> Send(Email email);
    }
}
