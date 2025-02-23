using System;
using System.Threading.Tasks;
using Moq;
using WhiteLabel.Application.DTOs.Generic;
using WhiteLabel.Application.DTOs.Users;
using WhiteLabel.Application.Interfaces.Users;
using WhiteLabel.WebAPI.Controllers.Users;
using Xunit;

namespace WhiteLabel.UnitTest.API
{
    public class UserControllerTest
    {
        [Fact]
        public async Task PostAdd_InsertElement_ReturnOkAndObject()
        {
            // This version uses a mock UrlHelper.
            var userDto = new UserDto { Id = Guid.NewGuid() };
            var userService = new Mock<IUserService>();
            userService
                .Setup(x => x.Add(userDto))
                .Returns(Task.FromResult(new Response<UserDto>(userDto)));

            // Arrange
            var controller = new UserController(userService.Object, userService.Object, null);
            var res = await controller.Add(userDto);

            // Assert
            Assert.True(res.Succeeded);
        }
    }
}
