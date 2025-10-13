namespace Plantpedia.Settings
{
    public class EmailSettings
    {
        public string SenderEmail { get; set; } = default!;
        public string SenderName { get; set; } = "Plantpedia";
        public string Password { get; set; } = default!;
        public string Host { get; set; } = "smtp.gmail.com";
        public int Port { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
    }
}
