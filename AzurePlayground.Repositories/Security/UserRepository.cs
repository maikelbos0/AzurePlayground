using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Utilities.Container;
using System;
using System.Linq;

namespace AzurePlayground.Repositories.Security {
    [InterfaceInjectable]
    public sealed class UserRepository : IUserRepository {
        private readonly IPlaygroundContext _context;

        public UserRepository(IPlaygroundContext context) {
            _context = context;
        }

        public User TryGetByEmail(string email) {
            return _context.Users.SingleOrDefault(u => u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
        }

        public User GetByEmail(string email, UserStatus expectedStatus) {
            return _context.Users.Single(u => u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase) && u.Status == expectedStatus);
        }

        public void Add(User user) {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void Update() {
            _context.SaveChanges();
        }
    }
}