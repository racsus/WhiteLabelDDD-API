﻿using Moq;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using WhiteLabel.Application.DTOs.Generic;
using WhiteLabel.Application.DTOs.Users;
using WhiteLabel.Application.Interfaces.Users;
using WhiteLabelDDD.Controllers;
using WhiteLabelDDD.Controllers.Users;

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
            userService.Setup(x => x.Add(userDto)).Returns(Task.FromResult(new Response<UserDTO>(userDto)));

            // Arrange
            UserController controller = new UserController(userService.Object, userService.Object, null);
            var res = await controller.Add(userDto);

            // Assert
            Assert.AreEqual(res.Succeeded, true);
        }
    }
}
