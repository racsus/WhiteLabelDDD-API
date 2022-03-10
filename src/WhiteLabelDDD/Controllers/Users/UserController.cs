using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using almacen.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using WhiteLabel.Application.Configuration;
using WhiteLabel.Application.Constants;
using WhiteLabel.Application.DTOs.Generic;
using WhiteLabel.Application.DTOs.Users;
using WhiteLabel.Application.Interfaces.Users;
using WhiteLabel.WebAPI.Controllers.Generic;
using WhiteLabelDDD.OAuth;

namespace WhiteLabelDDD.Controllers.Users
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : WhiteLabelController<IUserService>
    {
        private AuthConfiguration AuthConfiguration { get; }

        public UserController(IUserService userService, IUserService businessService, IConfiguration configuration)
        : base(userService, businessService)
        {
            this.AuthConfiguration = configuration.GetSection(AuthConfiguration.Section).Get<AuthConfiguration>();
        }

        /// <summary>
        /// Returns user information for logged in user using token information
        /// </summary>
        /// <returns>Json result with the user model</returns>
        [HttpGet("Me")]
        //[Authorize(Roles = "Administrator")]
        //[AuthorizeWithPermissions("read:dashboard")]
        [ProducesResponseType(typeof(UserInfoDTO), StatusCodes.Status200OK)]
        public async Task<Response<UserInfoDTO>> GetMe()
        {
            //var accessToken = Request.Headers[HeaderNames.Authorization];
            Response<UserInfoDTO> response = new Response<UserInfoDTO>()
            {
                //Object = await this.businessService.GetUserInfo(accessToken, this.User)
                Object = this.user
            };
            return response;
        }

        /// <summary>
        /// Only use if Bearear auth is enabled
        /// </summary>
        /// <param name="loginUserRequest">Username and password</param>
        /// <returns></returns>
        [HttpPost("Token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserInfoDTO), StatusCodes.Status200OK)]
        public Response<string> GetToken([FromBody] LoginUserRequest loginUserRequest)
        {
            if (this.AuthConfiguration.AuthType.ToUpper() == AuthConstants.Bearer)
            {
                List<Claim> claims = new()
                {
                    new Claim("id", "1"),
                    new Claim(ClaimTypes.Email, "test@test.com"),
                    new Claim(ClaimTypes.Name, loginUserRequest.UserName),
                };

                var token = JwtConfig.Generate(this.AuthConfiguration.AccessTokenSecret, this.AuthConfiguration.AccessTokenSecret,
                    this.AuthConfiguration.Audience, 30, claims);

                Response<string> response = new Response<string>()
                {
                    Object = token
                };
                return response;
            } else
            {
                return new Response<string>("Token endpoint can only be used with Bearear Auth");
            }
        }

        [HttpPost]
        public async Task<Response<UserDTO>> Add([FromBody] UserDTO user)
        {
            return await this.businessService.Add(user);
        }

        [HttpGet]
        [Route("IsEmailAvailable/{email}")]
        public async Task<Response<bool>> IsEmailAvailable(string email)
        {
            Response<bool> response = new Response<bool>()
            {
                Object = await this.businessService.IsEmailAvailable(email)
            };
            return response;
        }

        [HttpGet("{id}")]
        public async Task<Response<UserDTO>> GetById(Guid id)
        {
            return await this.businessService.Get(id);
        }

        [HttpGet("All")]
        public async Task<Response<IEnumerable<UserDTO>>> GetAll()
        {
            return await this.businessService.GetAll();
        }

        [HttpPost("Paginated")]
        [ProducesResponseType(typeof(PagedQueryResultDTO<UserDTO>), StatusCodes.Status200OK)]
        public async Task<Response<PagedQueryResultDTO<UserDTO>>> GetPaginated([FromBody] PagedListDTO pagedModel)
        {
            return await this.businessService.GetPaginated(pagedModel);
        }

        [HttpDelete("{id}")]
        public async Task<Response<UserDTO>> RemoveById(Guid id)
        {
            Response<UserDTO> response = new Response<UserDTO>();
            await this.businessService.Remove(id);
            return response;
        }

        [HttpPut]
        public async Task<Response<UserDTO>> Update([FromBody] UserDTO user)
        {
            Response<UserDTO> response = new Response<UserDTO>();
            await this.businessService.Update(user);
            return response;
        }

    }
}
