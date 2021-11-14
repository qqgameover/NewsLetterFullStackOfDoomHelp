namespace NewsletterX.Core.Domain.Model
{
    public class Email
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }

        public Email(string to, string @from, string subject, string text = null)
        {
            To = to;
            From = @from;
            Subject = subject;
            Text = text;
        }

        public Email()
        {
            
        }
    }
}
