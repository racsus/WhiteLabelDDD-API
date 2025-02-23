using System;
using Moq;
using WhiteLabel.Domain.Users;
using Xunit;

namespace WhiteLabel.UnitTest.Domain
{
    public class UserTest
    {
        [Fact]
        public void Add_WithoutEmail_ThrowsException()
        {
            Assert.ThrowsAny<ArgumentNullException>(
                () => User.Create(Guid.NewGuid(), It.IsAny<string>(), null)
            );
        }

        [Fact]
        public void Add_WithoutName_ThrowsException()
        {
            Assert.ThrowsAny<ArgumentNullException>(
                () => User.Create(Guid.NewGuid(), It.IsAny<string>(), null)
            );
        }

        [Fact]
        public void Add_WithoutId_ThrowsException()
        {
            Assert.ThrowsAny<ArgumentNullException>(
                () => User.Create(Guid.NewGuid(), It.IsAny<string>(), null)
            );
        }
    }
}
