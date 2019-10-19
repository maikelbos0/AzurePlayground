using AzurePlayground.Commands.Security;
using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Utilities.Container;
using AzurePlayground.Utilities.Mail;
using System;
using System.Linq;

namespace AzurePlayground.CommandHandlers.Security {
    [Injectable]
    public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand> {
        private readonly IPlaygroundContext _context;
        private readonly IMailClient _mailClient;
        private readonly IMailTemplate<ActivationMailTemplateParameters> _template;

        public RegisterUserCommandHandler(IPlaygroundContext context, IMailClient mailClient, IMailTemplate<ActivationMailTemplateParameters> template) {
            _context = context;
            _mailClient = mailClient;
            _template = template;
        }

        public CommandResult<RegisterUserCommand> Execute(RegisterUserCommand parameter) {
            var result = new CommandResult<RegisterUserCommand>();
            var user = new User(parameter.Email, parameter.Password);

            if (_context.Users.Any(u => u.Email.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase))) {
                result.AddError(p => p.Email, "Email address already exists");
            }

            if (result.Success) {
                _context.Users.Add(user);
                _context.SaveChanges();

                _mailClient.Send(_template.GetMessage(new ActivationMailTemplateParameters(user.Email, user.ActivationCode.Value), user.Email));
            }

            return result;
        }
    }
}