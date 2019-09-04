using AzurePlayground.Models.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Database;
using System.Security.Cryptography;

namespace AzurePlayground.Commands.Security {
    public class RegisterUserCommand : IRegisterUserCommand {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;
        private readonly int passwordHashIterations = 1000;

        //public UserCommands(IPlaygroundContextFactory playgroundContextFactory) {
        //_playgroundContextFactory = playgroundContextFactory;
        //}

        public RegisterUserCommand() {
            _playgroundContextFactory = new PlaygroundContextFactory();
        }

        public CommandResult Execute(UserRegistration parameter) {
            var result = new CommandResult();

            var user = new User() {
                Email = parameter.Email,
                PasswordSalt = GetNewPasswordSalt(),
                PasswordHashIterations = passwordHashIterations
            };

            user.PasswordHash = GetPasswordHash(parameter.Password, user.PasswordSalt, user.PasswordHashIterations);

            using (var context = _playgroundContextFactory.GetContext()) {
                context.Users.Add(user);
                context.SaveChanges();
            }

            return result;
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
