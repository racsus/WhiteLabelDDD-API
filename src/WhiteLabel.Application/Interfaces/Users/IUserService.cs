using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WhiteLabel.Application.DTOs.Generic;
using WhiteLabel.Application.DTOs.Users;
using WhiteLabel.Application.Interfaces.Generic;
using WhiteLabel.Domain.Pagination;

namespace WhiteLabel.Application.Interfaces.Users
{
    public interface IUserService : IBusinessService
    {
        Task<UserInfoDto> GetUserInfo(string token, ClaimsPrincipal principal);
        Task<Response<UserDto>> Add(UserDto userDto);
        Task Update(UserDto userDto);
        Task Remove(Guid userId);
        Task<Response<UserDto>> Get(Guid userId);
        Task<Response<IEnumerable<UserDto>>> GetAll();
        Task<bool> IsEmailAvailable(string email);
        Task<Response<PagedQueryResultDto<UserDto>>> GetPaginated(IPageOption pageDescriptor);
        Task<Response<IEnumerable<GroupDto>>> GetGrouped(string fieldToGroup);
    }
}
