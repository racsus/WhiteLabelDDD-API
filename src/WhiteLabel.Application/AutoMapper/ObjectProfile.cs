using AutoMapper;
using WhiteLabel.Application.DTOs.Users;
using WhiteLabel.Domain.Users;

namespace WhiteLabel.Application.AutoMapper
{
    public class ObjectProfile : Profile
    {
        public ObjectProfile()
        {
            //Users
            CreateMap<User, UserDto>();
        }
    }
}
