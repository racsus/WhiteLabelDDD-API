using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WhiteLabel.Application.DTOs.Generic;
using WhiteLabel.Application.DTOs.Users;
using WhiteLabel.Application.Interfaces.Generic;
using WhiteLabel.Domain.Pagination;

namespace WhiteLabel.Application.Interfaces.Users
{
    public interface IUserService: IBusinessService
    {
        Task<UserInfoDTO> GetUserInfo(string token, ClaimsPrincipal principal);
        Task<Response<UserDTO>> Add(UserDTO userDto);
        Task Update(UserDTO userDto);
        Task Remove(Guid userId);
        Task<Response<UserDTO>> Get(Guid userId);
        Task<Response<IEnumerable<UserDTO>>> GetAll();
        Task<bool> IsEmailAvailable(string email);
        Task<Response<PagedQueryResultDTO<UserDTO>>> GetPaginated(IPageOption pageDescriptor);
    }
}
