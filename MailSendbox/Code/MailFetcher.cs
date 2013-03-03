using ePunkt.Utilities;
using MailSendbox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MailSendbox.Code
{
    public class MailFetcher : IDisposable
    {
        public delegate void NewMailsArrivedDelegate(IEnumerable<Mail> allMails, IEnumerable<Mail> newMails);
        public delegate void MailsCheckedDelegate();
        public event NewMailsArrivedDelegate NewMailsArrived;
        public event MailsCheckedDelegate MailsChecked;

        private readonly MailRepository _mailRepository;

        public MailFetcher(MailRepository mailRepository)
        {
            _mailRepository = mailRepository;
        }

        public void Dispose()
        {
            StopAsyncFetching();
        }

        private List<Mail> _cachedMails = new List<Mail>();

        public IEnumerable<Mail> GetMails()
        {
            return _cachedMails;
        }

        private const int FetchIntervalInSeconds = 20;
        private static bool _isStopping;

        public void StartAsyncFetching()
        {
            Fetch();

            var taskFactory = new TaskFactory();
            taskFactory.StartNew(() =>
                {
                    var currentIntervalCount = 0;

                    while (!_isStopping)
                    {
                        if (currentIntervalCount >= FetchIntervalInSeconds)
                        {
                            currentIntervalCount = 0;
                            Fetch();
                        }

                        currentIntervalCount++;
                        Thread.Sleep(1000);
                    }
                });
        }

        public void StopAsyncFetching()
        {
            _isStopping = true;
        }

        private void Fetch()
        {
            try
            {
                const int maxMailsInInbox = 100;
                
                var fetchedMails = _mailRepository.Get().OrderByDescending(x => x.ReceivedDate).ToList();

                if (fetchedMails.Count > maxMailsInInbox)
                {
                    foreach (var mailToDelete in fetchedMails.Skip(maxMailsInInbox))
                        _mailRepository.Delete(mailToDelete.Index);
                    fetchedMails = fetchedMails.Take(maxMailsInInbox).ToList();
                }

                var newMails = fetchedMails.Where(m => !_cachedMails.Any(x => x.Id.Is(m.Id))).ToList();
                if (newMails.Count > 0 && NewMailsArrived != null)
                    NewMailsArrived(fetchedMails, newMails);

                _cachedMails = fetchedMails;

                if (MailsChecked != null)
                    MailsChecked();
            }
            catch
            {
                //do nothing here
            }
        }
    }

    public static class MailFetcherFactory
    {
        public static MailFetcher Current { get; set; }
    }
}