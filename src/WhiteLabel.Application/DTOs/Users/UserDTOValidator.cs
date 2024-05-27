using FluentValidation;

namespace WhiteLabel.Application.DTOs.Users
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(m => m.Name).NotEmpty();
            RuleFor(m => m.Email).NotEmpty();
            RuleFor(m => m.Email)
                .Matches("^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$")
                .WithMessage("Incorrect email format");
        }
    }
}
