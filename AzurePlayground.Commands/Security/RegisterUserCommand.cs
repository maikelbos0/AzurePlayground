using AzurePlayground.Models.Security;
using AzurePlayground.Domain.Security;
using AzurePlayground.Database;
using System;
using System.Linq;
using System.Security.Cryptography;
using AzurePlayground.Utilities.Container;

namespace AzurePlayground.Commands.Security {
    [Injectable]
    public class RegisterUserCommand : IRegisterUserCommand {
        private readonly IPlaygroundContextFactory _playgroundContextFactory;
        private readonly int passwordHashIterations = 1000;

        public RegisterUserCommand(IPlaygroundContextFactory playgroundContextFactory) {
            _playgroundContextFactory = playgroundContextFactory;
        }

        public CommandResult<UserRegistration> Execute(UserRegistration parameter) {
            var result = new CommandResult<UserRegistration>();
            var user = new User() {
                Email = parameter.Email,
                PasswordSalt = GetNewPasswordSalt(),
                PasswordHashIterations = passwordHashIterations
            };

            user.PasswordHash = GetPasswordHash(parameter.Password, user.PasswordSalt, user.PasswordHashIterations);

            user.UserEvents.Add(new UserEvent() {
                Date = DateTime.UtcNow,
                UserEventType = UserEventType.Registered
            });

            using (var context = _playgroundContextFactory.GetContext()) {
                if (context.Users.Any(u => u.Email.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase))) {
                    result.AddError(p => p.Email, "Email address already exists");
                }
                else {
                    context.Users.Add(user);
                    context.SaveChanges();
                }
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
