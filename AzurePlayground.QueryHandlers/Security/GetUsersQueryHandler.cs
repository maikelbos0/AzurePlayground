using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Queries.Security;
using AzurePlayground.Utilities.Container;
using System.Collections.Generic;
using System.Linq;

namespace AzurePlayground.QueryHandlers.Security {
    [InterfaceInjectable]
    public sealed class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, IEnumerable<UserViewModel>> {
        private IPlaygroundContext _context;

        public GetUsersQueryHandler(IPlaygroundContext context) {
            _context = context;
        }

        public IEnumerable<UserViewModel> Execute(GetUsersQuery query) {
            return _context.Users
                .Where(u => u.Status == UserStatus.Active)
                .Select(u => new UserViewModel() {
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    Email = u.ShowEmail ? u.Email : null,
                    Description = u.Description,
                    StartDate = u.UserEvents.Min(e => e.Date)
                })
                .ToList();
        }
    }
}