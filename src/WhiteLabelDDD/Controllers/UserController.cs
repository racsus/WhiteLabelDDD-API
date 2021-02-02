using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhiteLabel.Application.DTOs.Generic;
using WhiteLabel.Application.DTOs.Users;
using WhiteLabel.Application.Interfaces.Users;
using WhiteLabel.WebAPI.Models;

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

        [HttpPost]
        public async Task<Response<UserDTO>> Add([FromBody] UserDTO user)
        {
            Response<UserDTO> response = new Response<UserDTO>();
            response.Object = await this.userService.Add(user);
            return response;
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
            Response<UserDTO> response = new Response<UserDTO>();
            response.Object = await this.userService.Get(id);
            return response;
        }

        [HttpGet("All")]
        public async Task<Response<IEnumerable<UserDTO>>> GetAll()
        {
            Response<IEnumerable<UserDTO>> response = new Response<IEnumerable<UserDTO>>();
            response.Object = await this.userService.GetAll();
            return response;
        }

        [HttpPost("Paginated")]
        [ProducesResponseType(typeof(PagedQueryResultDTO<UserDTO>), StatusCodes.Status200OK)]
        public async Task<Response<PagedQueryResultDTO<UserDTO>>> GetPaginated([FromBody] PagedListDTO pagedModel)
        {
            Response<PagedQueryResultDTO<UserDTO>> response = new Response<PagedQueryResultDTO<UserDTO>>();
            response.Object = await this.userService.GetPaginated(pagedModel);
            return response;
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
