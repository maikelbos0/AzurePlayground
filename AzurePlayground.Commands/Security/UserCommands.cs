using AzurePlayground.Models.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Database;
using System.Security.Cryptography;

namespace AzurePlayground.Commands.Security {
    public class UserCommands : IUserCommands {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;
        private readonly int passwordHashIterations = 1000;

        //public UserCommands(IPlaygroundContextFactory playgroundContextFactory) {
        //_playgroundContextFactory = playgroundContextFactory;
        //}

        public UserCommands() {
            _playgroundContextFactory = new PlaygroundContextFactory();
        }

        public UserRegistration Register(UserRegistration userRegistration) {
            var user = new User() {
                Email = userRegistration.Email,
                PasswordSalt = GetNewPasswordSalt(),
                PasswordHashIterations = passwordHashIterations
            };

            user.PasswordHash = GetPasswordHash(userRegistration.Password, user.PasswordSalt, user.PasswordHashIterations);

            using (var context = _playgroundContextFactory.GetContext()) {
                context.Users.Add(user);
                context.SaveChanges();
            }

            return userRegistration;
        }

        private byte[] GetNewPasswordSalt() {
            byte[] salt = new byte[20];

            new RNGCryptoServiceProvider().GetBytes(salt);

            return salt;
        }

        private byte[] GetPasswordHash(string password, byte[] salt, int iterations) {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);

            // Return 20 bytes because after that it repeats
            return pbkdf2.GetBytes(20);
        }
    }
}
