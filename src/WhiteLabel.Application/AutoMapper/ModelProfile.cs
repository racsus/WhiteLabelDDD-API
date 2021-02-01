using AutoMapper;
using WhiteLabel.Application.DTOs.Users;
using WhiteLabel.Application.Interfaces.Generic;
using WhiteLabel.Domain.Users;

namespace WhiteLabel.Application.AutoMapper
{
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            //Users
            CreateMap<UserDTO, User>();
        }
    }
}
