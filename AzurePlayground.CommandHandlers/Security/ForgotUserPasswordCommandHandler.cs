using AzurePlayground.Commands.Security;
using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Mail;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class ForgotUserPasswordCommandHandler : ICommandHandler<ForgotUserPasswordCommand> {
        private readonly IPlaygroundContext _context;
        private readonly IMailClient _mailClient;
        private readonly IMailTemplate<PasswordResetMailTemplateParameters> _template;

        public ForgotUserPasswordCommandHandler(IPlaygroundContext context, IMailClient mailClient, IMailTemplate<PasswordResetMailTemplateParameters> template) {
            _context = context;
            _mailClient = mailClient;
            _template = template;
        }

        public CommandResult<ForgotUserPasswordCommand> Execute(ForgotUserPasswordCommand parameter) {
            var result = new CommandResult<ForgotUserPasswordCommand>();
            var user = _context.Users.SingleOrDefault(u => u.Email.Equals(parameter.Email, StringComparison.InvariantCultureIgnoreCase));

            // There is no error reporting to prevent information leaking
            if (user != null && user.Status == UserStatus.Active) {
                var token = user.GeneratePasswordResetToken();

                _context.SaveChanges();
                _mailClient.Send(_template.GetMessage(new PasswordResetMailTemplateParameters(user.Email, token), user.Email));
            }

            return result;
        }
    }
}
