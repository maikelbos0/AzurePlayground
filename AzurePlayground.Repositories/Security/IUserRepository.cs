using AzurePlayground.Domain.Security;

namespace AzurePlayground.Repositories.Security {
    public interface IUserRepository {
        User TryGetByEmail(string email);
        void Add(User user);
        void Update();
    }
}