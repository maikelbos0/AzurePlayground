using AzurePlayground.Domain.Security;

namespace AzurePlayground.Repositories.Security {
    public interface IUserRepository {
        User TryGetByEmail(string email);
        User GetByEmail(string email, UserStatus expectedStatus);
        void Add(User user);
        void Update();
    }
}