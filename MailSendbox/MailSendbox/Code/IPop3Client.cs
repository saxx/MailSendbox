using System.Collections.Generic;
using OpenPop.Pop3;

namespace MailSendbox.Code
{
    public interface IPop3Client
    {
        void Connect(string host, int port, bool useSsl);
        void Authenticate(string user, string password, AuthenticationMethod authMethod);
        void Disconnect();
        int GetMessageCount();
        OpenPop.Mime.Message GetMessage(int messageId);
        List<string> GetMessageUids();
        void DeleteMessage(int messageId);
        bool Connected { get; }
    }

    public class Pop3Client : OpenPop.Pop3.Pop3Client, IPop3Client { }
}