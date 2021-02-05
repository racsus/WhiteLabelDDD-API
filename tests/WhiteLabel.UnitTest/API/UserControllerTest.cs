using Moq;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WhiteLabel.Application.DTOs.Users;
using WhiteLabel.Application.Interfaces.Users;
using WhiteLabelDDD.Controllers;

/// <summary>
/// https://docs.microsoft.com/en-us/aspnet/web-api/overview/testing-and-debugging/unit-testing-controllers-in-web-api
/// </summary>
namespace WhiteLabel.UnitTest.API
{
    public class UserControllerTest
    {
        [Test]
        public async Task PostAdd_InsertElement_ReturnOkAndObject()
        {
            // This version uses a mock UrlHelper.
            var userDto = new UserDTO();
            userDto.Id = Guid.NewGuid();
            Mock<IUserService> userService = new Mock<IUserService>();
            userService.Setup(x => x.Add(userDto)).Returns(Task.FromResult(userDto));

            // Arrange
            UserController controller = new UserController(userService.Object);
            var res = await controller.Add(new UserDTO());

            // Assert
            Assert.AreEqual(res.Succeeded, true);
        }
    }
}
