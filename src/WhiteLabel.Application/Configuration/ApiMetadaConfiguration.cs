namespace WhiteLabel.Application.Configuration
{
    public class ApiMetadataConfiguration
    {
        /// <summary>
        /// Name of the config section
        /// </summary>
        public const string Section = "ApiMetadata";

        /// <summary>
        /// Gets or sets Version data
        /// </summary>
        public ApiVersionData[] ApiVersions { get; set; }

        public ApiContactData ApiContactData { get; set; }
    }
}
