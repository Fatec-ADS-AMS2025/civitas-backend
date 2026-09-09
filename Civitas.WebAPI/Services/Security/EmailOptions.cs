namespace Civitas.WebAPI.Services.Security
{
    public class EmailOptions
    {
        public const string SectionName = "Email";

        public string Host { get; set; } = string.Empty;

        public int Port { get; set; } = 587;

        public bool EnableSsl { get; set; } = true;

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string FromAddress { get; set; } = "noreply@civitas.local";

        public string FromName { get; set; } = "Civitas";

        public int TimeoutSeconds { get; set; } = 15;
    }
}
