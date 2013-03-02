using MailSendbox.Code;
using Microsoft.AspNet.SignalR;
using System.Linq;
using System.Threading;

namespace MailSendbox.Hubs
{
    public class MailsHub : Hub
    {
        public void Init()
        {
            var mails = MailFetcherFactory.Current.GetMails().Take(30).Select(x => new MailsHubEventListener.MailHelper(x)).ToList();

            //this helps when the cache is still empty, usually happens when the application has just started.
            var maxLoopCount = 4 * 60;
            while (maxLoopCount > 0 && !mails.Any())
            {
                Thread.Sleep(250);
                maxLoopCount--;
                mails = MailFetcherFactory.Current.GetMails().Take(30).Select(x => new MailsHubEventListener.MailHelper(x)).ToList();
            }

            var result = new
                {
                    AllMails = mails,
                    NewMails = new MailsHubEventListener.MailHelper[0]
                };

            Clients.Caller.newMailsArrived(result);
        }
    }
}