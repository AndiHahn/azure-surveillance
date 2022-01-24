using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Function.Notification.Infrastructure
{
    internal class EmailModel
    {
        public string SenderName { get; set; } = string.Empty;

        public string SenderEmail { get; set; }

        public string ReceiverName { get; set; } = string.Empty;

        public string ReceiverEmail { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public IDictionary<string, string> Substitutions { get; set; }
    }
}
