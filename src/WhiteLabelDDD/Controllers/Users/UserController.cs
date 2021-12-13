using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using WhiteLabel.Application.DTOs.Generic;
using WhiteLabel.Application.DTOs.Users;
using WhiteLabel.Application.Interfaces.Users;
using WhiteLabel.WebAPI.Controllers.Generic;

namespace WhiteLabelDDD.Controllers.Users
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : WhiteLabelController<IUserService>
    {
        public UserController(IUserService userService, IUserService businessService)
        : base(userService, businessService)
        {
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
