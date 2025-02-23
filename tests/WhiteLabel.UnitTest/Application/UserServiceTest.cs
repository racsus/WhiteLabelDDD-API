using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Moq;
using WhiteLabel.Application.DTOs.Users;
using WhiteLabel.Application.Interfaces.Generic;
using WhiteLabel.Application.Services.Users;
using WhiteLabel.Domain.Generic;
using WhiteLabel.Domain.Users;
using WhiteLabel.UnitTest.Builders;
using Xunit;

namespace WhiteLabel.UnitTest.Application
{
    public class UserServiceTest
    {
        [Fact]
        public async Task Add_WithEmailAlreadyInDatabase_ThrowsException()
        {
            var service = GetService(new UserBuilder().WithTestValues().Build());

            var response = await service.Add(new UserDto());
            Assert.Equal(
                "User with this email already exists",
                response.Errors.FirstOrDefault()!.Description
            );
        }

        [Fact]
        public async Task Update_WithOutId_ThrowsException()
        {
            var service = GetService(null);

            await Assert.ThrowsAnyAsync<ArgumentException>(() => service.Update(new UserDto()));
        }

        [Fact]
        public async Task Update_NoUserInDatabase_ThrowsException()
        {
            var service = GetService(null);

            await Assert.ThrowsAnyAsync<ArgumentException>(() => service.Update(new UserDto()));
        }

        [Fact]
        public async Task Remove_NoUserInDatabase_ThrowsException()
        {
            var service = GetService(null);

            await Assert.ThrowsAnyAsync<ArgumentException>(() => service.Update(new UserDto()));
        }

        private static UserService GetService(User user)
        {
            var genericRepository = new Mock<IGenericRepository<Guid>>();
            var unitOfWork = new Mock<IUnitOfWork>();
            var mapper = new Mock<IMapper>();

            unitOfWork.Setup(x => x.Commit());
            genericRepository
                .Setup(x =>
                    x.FindOneAsync(It.IsAny<ISpecification<User>>(), It.IsAny<CancellationToken>())
                )
                .Returns(Task.FromResult(user));

            //Arrange
            var inMemorySettings = new Dictionary<string, string>
            {
                { "Authentication", "" },
                //...populate as needed for the test
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var res = new UserService(
                genericRepository.Object,
                unitOfWork.Object,
                mapper.Object,
                configuration
            );

            return res;
        }
    }
}
