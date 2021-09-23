using NUnit.Framework;
using System;
using Moq;
using WhiteLabel.Application.Interfaces.Generic;
using WhiteLabel.Domain.Users;
using WhiteLabel.Application.Services.Users;
using AutoMapper;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Specification.Users;
using WhiteLabel.Application.DTOs.Users;
using System.Threading.Tasks;
using System.Threading;
using WhiteLabel.UnitTest.Builders;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace WhiteLabel.UnitTest.Application
{
    public class UserServiceTest
    {
        [Test]
        public async Task Add_WithEmailAlreadyInDatabase_ThrowsException()
        {
            var service = GetService(new UserBuilder().WithTestValues().Build());

            var response = await service.Add(new UserDTO());
            Assert.AreEqual("User with this email already exists", response.Errors.FirstOrDefault().Description);
        }

        [Test]
        public void Update_WithOutId_ThrowsException()
        {
            var service = GetService(null);

            Assert.That(() => service.Update(new UserDTO()),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Update_NoUserInDatabase_ThrowsException()
        {
            var service = GetService(null);

            var userDto = new UserDTO()
            {
                Id = Guid.NewGuid()
            };
            Assert.That(() => service.Update(userDto),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Remove_NoUserInDatabase_ThrowsException()
        {
            var service = GetService(null);

            Assert.That(() => service.Remove(Guid.NewGuid()),
                Throws.TypeOf<ArgumentException>());
        }

        private static UserService GetService(User user)
        {
            Mock<IGenericRepository<Guid>> genericRepository = new Mock<IGenericRepository<Guid>>();
            Mock<IUnitOfWork> unitOfWork = new Mock<IUnitOfWork>();
            Mock<IMapper> mapper = new Mock<IMapper>();

            unitOfWork.Setup(x => x.Commit());
            genericRepository.Setup(x => x.FindOneAsync(It.IsAny<ISpecification<User>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(user));

            //Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"Authentication", ""},
                //...populate as needed for the test
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var res = new UserService(genericRepository.Object, unitOfWork.Object, mapper.Object, configuration);

            return res;
        }
    }
}
