using Swashbuckle.AspNetCore.Annotations;

namespace WhiteLabel.WebAPI.Models
{
    public class LoginUserRequest
    {
        [SwaggerSchema(Required = new[] { "The User Name" })]
        public string UserName { get; set; }

        [SwaggerSchema(Required = new[] { "The User Password" })]
        public string Password { get; set; }
    }
}
