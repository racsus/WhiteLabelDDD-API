using System;
using System.Collections.Generic;
using System.Text;
using WhiteLabel.Domain.Events;
using WhiteLabel.Domain.Generic;

namespace WhiteLabel.Domain.Users
{
    public class User: BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public static User Create(string name, string email)
        {
            return Create(Guid.NewGuid(), name, email);
        }

        public static User Create(Guid id, string name, string email)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("email");

            User user = new User()
            {
                Id = id,
                Name = name,
                Email = email
            };

            user.Events.Add(new UserCreatedEvent(user.Id, user));

            return user;
        }

        public void Update(string name, string email)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("email");

            this.Name = name;
            this.Email = email;
        }
    }
}
