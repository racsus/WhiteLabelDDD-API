using System;
using System.Collections.Generic;
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
        Task<UserDTO> Add(UserDTO userDto);
        Task Update(UserDTO userDto);
        Task Remove(Guid userId);
        Task<UserDTO> Get(Guid userId);
        Task<IEnumerable<UserDTO>> GetAll();
        Task<bool> IsEmailAvailable(string email);
        Task<PagedQueryResultDTO<UserDTO>> GetPaginated(IPageOption pageDescriptor);
    }
}
