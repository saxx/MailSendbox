using ePunkt.Utilities;
using MailSendbox.Models;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MailSendbox.Code
{
    public class MailRepository : IMailRepository
    {
        private readonly IPop3Client _pop3Client;

        public MailRepository(IPop3Client pop3Client)
        {
            _pop3Client = pop3Client;
        }

        public IEnumerable<Mail> Get()
        {
            var result = new List<Mail>();

            try
            {
                ConnectAndAuthenticate();

                var messageCount = _pop3Client.GetMessageCount();
                for (var i = messageCount; i > 0; i--)
                {
                    var message = _pop3Client.GetMessage(i);

                    var plainText = message.FindFirstPlainTextVersion();
                    var mail = new Mail
                        {
                            Uid = message.Headers.MessageId,
                            Body = plainText == null ? string.Empty : plainText.GetBodyAsText(),
                            ReceivedDate = message.Headers.DateSent,
                            From = message.Headers.From.Address,
                            To = message.Headers.To.Aggregate("", (seed, recipient) => seed += ", " + recipient.Address).Trim(' ', ','),
                            Subject = message.Headers.Subject
                        };

                    result.Add(mail);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to fetch mails.", ex);
            }
            finally
            {
                Disconnect();
            }

            return result;
        }

        public void Delete(string uid)
        {
            try
            {
                ConnectAndAuthenticate();

                var allUids = _pop3Client.GetMessageUids();

                var index = 0;
                foreach (var u in allUids)
                {
                    index++;
                    if (u.Is(uid))
                    {
                        _pop3Client.DeleteMessage(index);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unable to delete mail.", ex);
            }
            finally
            {
                Disconnect();
            }
        }

        private void ConnectAndAuthenticate()
        {
            var server = Settings.Get("Pop3Server", "");
            var port = Settings.Get("Pop3Port", 110);
            var user = Settings.Get("Pop3Username", "");
            var password = Settings.Get("Pop3Password", "");
            var useSsl = Settings.Get("Pop3UseSsl", false);

            if (server.IsNoW())
                throw new ApplicationException("No POP3 host specified in AppSettings.");

            _pop3Client.Connect(server, port, useSsl);

            if (user.HasValue())
                _pop3Client.Authenticate(user, password, AuthenticationMethod.UsernameAndPassword);
        }

        private void Disconnect()
        {
            if (_pop3Client.Connected)
                _pop3Client.Disconnect();
        }
    }
}