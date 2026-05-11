using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class User : IAppEntity
    {
        private readonly List<IDomainEvent> _events = new List<IDomainEvent>();

        public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

        public Guid Id { get; private set; } 

        public string Email { get; private set; }

        public string PasswordHash { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private User() { }

        private User( string email, string passwordHash)
        {
         
            Id = Guid.NewGuid();
            Email = email;
            PasswordHash = passwordHash;
            CreatedAt = DateTime.UtcNow;
        }

        public static Result<User, EntityError> Create(string email, string passwordHash)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Result<User, EntityError>.Failure(EntityError.InvalidEmail);
            }
            if (string.IsNullOrEmpty(passwordHash))
            {
                return Result<User, EntityError>.Failure(EntityError.InvalidPassword);
            }
            var user = new User(email, passwordHash);

            return Result<User, EntityError>.Success(user);
        }

        public void ClearEvents()
        {
            _events.Clear();
        }
    }
}
