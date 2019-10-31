using AzurePlayground.Database;
using AzurePlayground.Queries.Security;
using AzurePlayground.Utilities.Container;
using System;
using System.Linq;

namespace AzurePlayground.QueryHandlers.Security {
    [InterfaceInjectable]
    public sealed class GetUserNameQueryHandler : IQueryHandler<GetUserNameQuery, string> {
        private readonly IPlaygroundContext _context;

        public GetUserNameQueryHandler(IPlaygroundContext context) {
            _context = context;
        }

        public string Execute(GetUserNameQuery query) {
            var user = _context.Users
                .Single(u => u.Email.Equals(query.Email, StringComparison.InvariantCultureIgnoreCase));

            return user.DisplayName ?? user.Email;
        }
    }
}