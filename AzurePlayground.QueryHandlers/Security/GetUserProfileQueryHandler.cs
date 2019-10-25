using AzurePlayground.Database;
using AzurePlayground.Models.Security;
using AzurePlayground.Queries.Security;
using AzurePlayground.Utilities.Container;
using System;
using System.Linq;

namespace AzurePlayground.QueryHandlers.Security {
    [InterfaceInjectable]
    public sealed class GetUserProfileQueryHandler : IQueryHandler<GetUserProfileQuery, UserProfileModel> {
        private IPlaygroundContext _context;

        public GetUserProfileQueryHandler(IPlaygroundContext context) {
            _context = context;
        }

        public UserProfileModel Execute(GetUserProfileQuery query) {
            return _context.Users
                .Where(u => u.Email.Equals(query.Email, StringComparison.InvariantCultureIgnoreCase))
                .Select(u => new UserProfileModel() {
                    DisplayName = u.DisplayName,
                    Description = u.Description,
                    ShowEmail = u.ShowEmail
                })
                .Single();
        }
    }
}