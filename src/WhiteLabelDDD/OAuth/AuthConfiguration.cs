namespace WhiteLabelDDD.OAuth
{
    internal class AuthConfiguration
    {
        public const string Section = "Authentication";

        public bool IsEnabled { get; set; }

        public string Application { get; set; }

        public string Authority { get; set; }

        public string Scope { get; set; }
    }
}
