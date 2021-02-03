using Ardalis.GuardClauses;
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
            Guard.Against.Null(id, nameof(id));
            Guard.Against.Null(name, nameof(name));
            Guard.Against.Null(email, nameof(email));

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
            Guard.Against.Null(name, nameof(name));
            Guard.Against.Null(email, nameof(email));

            this.Name = name;
            this.Email = email;
        }
    }
}
