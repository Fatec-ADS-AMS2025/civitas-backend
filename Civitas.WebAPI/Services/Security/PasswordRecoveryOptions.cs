namespace Civitas.WebAPI.Services.Security
{
    public class PasswordRecoveryOptions
    {
        public const string SectionName = "PasswordRecovery";
        public string FrontendBaseUrl { get; set; } = "http://localhost:3000";
        public int TokenExpirationMinutes { get; set; } = 60;
    }
}
