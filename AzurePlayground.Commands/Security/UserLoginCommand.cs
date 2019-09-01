using AzurePlayground.Models.Security;
using AzurePlayground.Database;
using System.Linq;

namespace AzurePlayground.Commands.Security {
    public class UserLoginCommand : ICommand<UserLogin, UserLoginResult> {
        public UserLoginResult Execute(UserLogin parameter) {
            using (var context = new PlaygroundContext()) {
                var user = context.Users.SingleOrDefault(u => u.Email == parameter.Email);

                return new UserLoginResult() {
                    Identity = user.Email,
                    Success = false
                };
            }
        }
    }
}
