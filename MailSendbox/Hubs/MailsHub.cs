using MailSendbox.Code;
using Microsoft.AspNet.SignalR;
using System.Linq;

namespace MailSendbox.Hubs
{
    public class MailsHub : Hub
    {
        public void Init()
        {
            var mails = MailFetcherFactory.Current.GetMails().Take(30).Select(x => new MailsHubEventListener.MailHelper(x)).ToList();
            var result = new
                {
                    AllMails = mails,
                    NewMails = new MailsHubEventListener.MailHelper[0]
                };

            Clients.Caller.newMailsArrived(result);
        }
    }
}