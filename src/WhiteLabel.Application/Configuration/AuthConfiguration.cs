namespace WhiteLabel.Application.Configuration
{
    public class AuthConfiguration
    {
        public const string Section = "Authentication";

        public bool IsEnabled { get; set; }

        public string Application { get; set; }

        public string Authority { get; set; }

        public string Scope { get; set; }

        public string AuthType { get; set; }

        public string Audience { get; set; }

        public string Namespace { get; set; }
    }
}
