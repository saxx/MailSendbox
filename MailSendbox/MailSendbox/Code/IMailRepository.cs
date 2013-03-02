using MailSendbox.Models;
using System.Collections.Generic;

namespace MailSendbox.Code
{
    public interface IMailRepository
    {
        IEnumerable<Mail> Get();
        void Delete(string uid);
    }
}