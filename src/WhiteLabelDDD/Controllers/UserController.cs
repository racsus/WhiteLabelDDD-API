using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using WhiteLabel.Application.DTOs.Generic;
using WhiteLabel.Application.DTOs.Users;
using WhiteLabel.Application.Interfaces.Users;

namespace WhiteLabelDDD.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
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
            var accessToken = Request.Headers[HeaderNames.Authorization];
            Response<UserInfoDTO> response = new Response<UserInfoDTO>();
            response.Object = await this.userService.GetUserInfo(accessToken, this.User);
            return response;
        }

        [HttpPost]
        public async Task<Response<UserDTO>> Add([FromBody] UserDTO user)
        {
            return await this.userService.Add(user);
        }

        [HttpGet]
        [Route("IsEmailAvailable/{email}")]
        public async Task<Response<bool>> IsEmailAvailable(string email)
        {
            Response<bool> response = new Response<bool>();
            response.Object = await this.userService.IsEmailAvailable(email);
            return response;
        }

        [HttpGet("{id}")]
        public async Task<Response<UserDTO>> GetById(Guid id)
        {
            return await this.userService.Get(id);
        }

        [HttpGet("All")]
        public async Task<Response<IEnumerable<UserDTO>>> GetAll()
        {
            return await this.userService.GetAll();
        }

        [HttpPost("Paginated")]
        [ProducesResponseType(typeof(PagedQueryResultDTO<UserDTO>), StatusCodes.Status200OK)]
        public async Task<Response<PagedQueryResultDTO<UserDTO>>> GetPaginated([FromBody] PagedListDTO pagedModel)
        {
            return await this.userService.GetPaginated(pagedModel);
        }

        [HttpDelete("{id}")]
        public async Task<Response<UserDTO>> RemoveById(Guid id)
        {
            Response<UserDTO> response = new Response<UserDTO>();
            await this.userService.Remove(id);
            return response;
        }

        [HttpPut]
        public async Task<Response<UserDTO>> Update([FromBody] UserDTO user)
        {
            Response<UserDTO> response = new Response<UserDTO>();
            await this.userService.Update(user);
            return response;
        }

    }
}
