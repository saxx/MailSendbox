using System;
using ePunkt.Utilities;

namespace MailSendbox.Models
{
    public class Mail
    {
        public string Id { get; set; }
        public int Index { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime ReceivedDate { get; set; }

        private string GetBodySnippet()
        {
            const int maxLength = 300;

            if (Body.IsNoW())
                return "";

            if (Body.Length <= maxLength)
                return Body;

            return Body.Substring(0, maxLength) + " [...]";
        }

        public string GetBodySnippetAsHtml()
        {
            var snippet = GetBodySnippet();
            snippet = snippet.Replace("\r", "").Replace("\n", "<br />");
            return snippet;
        }
    }
}