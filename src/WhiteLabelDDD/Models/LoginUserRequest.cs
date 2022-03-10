using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace almacen.WebAPI.Models
{
    public class LoginUserRequest
    {
        [SwaggerSchema(Required = new[] { "The User Name" })]
        public string UserName { get; set; }

        [SwaggerSchema(Required = new[] { "The User Password" })]
        public string Password { get; set; }
    }
}
