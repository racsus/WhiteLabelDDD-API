using Moq;
using NUnit.Framework;
using System;
using WhiteLabel.Domain.Users;

namespace WhiteLabel.UnitTest.DomainModel
{
    public class UserTest
    {
        [Test]
        public void Add_WithoutEmail_ThrowsException()
        {
            Assert.That(() => User.Create(Guid.NewGuid(), It.IsAny<String>(), null),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Add_WithoutName_ThrowsException()
        {
            Assert.That(() => User.Create(Guid.NewGuid(), null, It.IsAny<String>()),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Add_WithoutId_ThrowsException()
        {
            Assert.That(() => User.Create(Guid.Empty, It.IsAny<String>(), It.IsAny<String>()),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Update_WithoutEmail_ThrowsException()
        {
            var user = new User();
            Assert.That(() => user.Update(It.IsAny<String>(), null),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Update_WithoutName_ThrowsException()
        {
            var user = new User();
            Assert.That(() => user.Update(null, It.IsAny<String>()),
                Throws.TypeOf<ArgumentNullException>());
        }
    }
}