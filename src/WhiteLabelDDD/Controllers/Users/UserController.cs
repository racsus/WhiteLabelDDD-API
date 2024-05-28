using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WhiteLabel.Application.Configuration;
using WhiteLabel.Application.Constants;
using WhiteLabel.Application.DTOs.Generic;
using WhiteLabel.Application.DTOs.Users;
using WhiteLabel.Application.Interfaces.Users;
using WhiteLabel.WebAPI.Controllers.Generic;
using WhiteLabel.WebAPI.Models;
using WhiteLabel.WebAPI.OAuth;

namespace WhiteLabel.WebAPI.Controllers.Users
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController(
        IUserService userService,
        IUserService businessService,
        IConfiguration configuration)
        : WhiteLabelController<IUserService>(userService, businessService)
    {
        private AuthConfiguration AuthConfiguration { get; } = configuration
            .GetSection(AuthConfiguration.Section)
            .Get<AuthConfiguration>();

        /// <summary>
        /// Returns user information for logged in user using token information
        /// </summary>
        /// <returns>Json result with the user model</returns>
        [HttpGet("Me")]
        //[Authorize(Roles = "Administrator")]
        //[AuthorizeWithPermissions("read:dashboard")]
        [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
        public Task<Response<UserInfoDto>> GetMe()
        {
            //var accessToken = Request.Headers[HeaderNames.Authorization];
            var response = new Response<UserInfoDto>()
            {
                //Object = await this.businessService.GetUserInfo(accessToken, this.User)
                Object = UserInfoDto
            };
            return Task.FromResult(response);
        }

        /// <summary>
        /// Only use if Bearear auth is enabled
        /// </summary>
        /// <param name="loginUserRequest">Username and password</param>
        /// <returns></returns>
        [HttpPost("Token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
        public Response<string> GetToken([FromBody] LoginUserRequest loginUserRequest)
        {
            if (AuthConfiguration.AuthType.Equals(AuthConstants.Bearer, StringComparison.CurrentCultureIgnoreCase))
            {
                List<Claim> claims =
                [
                    new Claim("id", "1"),
                    new Claim(ClaimTypes.Email, "test@test.com"),
                    new Claim(ClaimTypes.Name, loginUserRequest.UserName)
                ];

                var token = JwtConfig.Generate(
                    AuthConfiguration.AccessTokenSecret,
                    AuthConfiguration.AccessTokenSecret,
                    AuthConfiguration.Audience,
                    30,
                    claims
                );

                var response = new Response<string>() { Object = token };
                return response;
            }

            return new Response<string>("Token endpoint can only be used with Bearer Auth");
        }

        [HttpPost]
        public async Task<Response<UserDto>> Add([FromBody] UserDto userDto)
        {
            return await BusinessService.Add(userDto);
        }

        [HttpGet]
        [Route("IsEmailAvailable/{email}")]
        public async Task<Response<bool>> IsEmailAvailable(string email)
        {
            var response = new Response<bool>()
            {
                Object = await BusinessService.IsEmailAvailable(email)
            };
            return response;
        }

        [HttpGet("{id:guid}")]
        public async Task<Response<UserDto>> GetById(Guid id)
        {
            return await BusinessService.Get(id);
        }

        [HttpGet("All")]
        public async Task<Response<IEnumerable<UserDto>>> GetAll()
        {
            return await BusinessService.GetAll();
        }

        [HttpPost("Paginated")]
        [ProducesResponseType(typeof(PagedQueryResultDto<UserDto>), StatusCodes.Status200OK)]
        public async Task<Response<PagedQueryResultDto<UserDto>>> GetPaginated(
            [FromBody] PagedListDto pagedModel
        )
        {
            return await BusinessService.GetPaginated(pagedModel);
        }

        [HttpDelete("{id:guid}")]
        public async Task<Response<UserDto>> RemoveById(Guid id)
        {
            var response = new Response<UserDto>();
            await BusinessService.Remove(id);
            return response;
        }

        [HttpPut]
        public async Task<Response<UserDto>> Update([FromBody] UserDto userDto)
        {
            var response = new Response<UserDto>();
            await BusinessService.Update(userDto);
            return response;
        }
    }
}
