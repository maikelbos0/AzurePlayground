using AzurePlayground.Database;
using AzurePlayground.Queries.Security;
using AzurePlayground.Utilities.Container;
using System;
using System.Linq;

namespace AzurePlayground.QueryHandlers.Security {
    [InterfaceInjectable]
    public sealed class GetUserNewEmailQueryHandler : IQueryHandler<GetUserNewEmailQuery, string> {
        private readonly IPlaygroundContext _context;

        public GetUserNewEmailQueryHandler(IPlaygroundContext context) {
            _context = context;
        }

        public string Execute(GetUserNewEmailQuery query) {
            var user = _context.Users
                .Single(u => u.Email.Equals(query.Email, StringComparison.InvariantCultureIgnoreCase));

            return user.NewEmail;
        }
    }
}