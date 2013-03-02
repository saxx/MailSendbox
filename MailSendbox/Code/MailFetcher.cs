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
        public delegate void MailsDelegate(IEnumerable<Mail> mails);

        public event MailsDelegate NewMailsArrived;
        public event MailsDelegate MailsChecked;

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

        private const int FetchIntervalInSeconds = 10;
        private static bool _isStopping;
        public void StartAsyncFetching()
        {
            var taskFactory = new TaskFactory();
            taskFactory.StartNew(() =>
                {
                    var currentIntervalCount = 0;

                    while (!_isStopping)
                    {
                        if (currentIntervalCount >= FetchIntervalInSeconds)
                        {
                            currentIntervalCount = 0;

                            var fetchedMails = _mailRepository.Get().OrderByDescending(x => x.ReceivedDate).ToList();

                            var newMails = fetchedMails.Where(m => !_cachedMails.Any(x => x.Uid.Is(m.Uid))).ToList();
                            if (/*newMails.Count > 0 &&*/ NewMailsArrived != null)
                                NewMailsArrived(newMails);

                            _cachedMails = fetchedMails;

                            if (MailsChecked != null)
                                MailsChecked(_cachedMails);
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
    }

    public static class MailFetcherFactory
    {
        public static MailFetcher Current { get; set; }
    }
}