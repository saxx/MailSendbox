using MailSendbox.Code;
using MailSendbox.Models;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using System.Linq;

namespace MailSendbox.Hubs
{
    public class MailsHubEventListener
    {
        #region Singleton
        private static MailsHubEventListener _current;

        public static MailsHubEventListener Current
        {
            get { return _current ?? (_current = new MailsHubEventListener()); }
        }
        #endregion

        public void Init()
        {
            MailFetcherFactory.Current.MailsChecked += Current_MailsChecked;
            MailFetcherFactory.Current.NewMailsArrived += Current_NewMailsArrived;
        }

        void Current_NewMailsArrived(IEnumerable<Mail> newMails)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<MailsHub>();

            var result = new
            {
                AllMails = MailFetcherFactory.Current.GetMails().Take(30).Select(x => new MailHelper(x)),
                NewMails = newMails.Select(x => new MailHelper(x))
            };
            hub.Clients.All.newMailsArrived(result);
        }

        void Current_MailsChecked(IEnumerable<Mail> allMails)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<MailsHub>();
            hub.Clients.All.mailsChecked();
        }

        public class MailHelper
        {
            public MailHelper(Mail mail)
            {
                Subject = mail.Subject;
                Body = mail.GetBodySnippetAsHtml();
                From = mail.From;
                To = mail.To;
                Date = mail.ReceivedDate.ToShortDateString();
            }

            // ReSharper disable MemberCanBePrivate.Local
            public string Subject { get; set; }
            public string Body { get; set; }
            public string From { get; set; }
            public string To { get; set; }
            public string Date { get; set; }
            // ReSharper restore MemberCanBePrivate.Local
        }
    }
}