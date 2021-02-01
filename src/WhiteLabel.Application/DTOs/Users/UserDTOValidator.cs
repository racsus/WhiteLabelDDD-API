using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteLabel.Application.DTOs.Users
{
    public class UserDTOValidator: AbstractValidator<UserDTO>
    {
        public UserDTOValidator()
        {
            RuleFor(m => m.Name).NotEmpty();
            RuleFor(m => m.Email).NotEmpty();
            RuleFor(m => m.Email).Matches("^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$").WithMessage("Incorrect email format");
        }
    }
}
