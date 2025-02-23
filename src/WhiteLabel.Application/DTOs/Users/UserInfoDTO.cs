using System.Collections.Generic;
using Newtonsoft.Json;

namespace WhiteLabel.Application.DTOs.Users
{
    public class UserInfoDto
    {
        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }
        public string NickName { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
        public string Locale { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<string> Permissions { get; set; }
    }
}
