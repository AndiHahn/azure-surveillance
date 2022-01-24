namespace Surveillance.Function.Notification
{
    internal class AppConfiguration
    {
        public string SendGridApiKey { get; set; }

        public string SenderName { get; set; }

        public string SenderEmail { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverEmail { get; set; }
    }
}
