using AzurePlayground.Database;
using AzurePlayground.Domain.Security;
using AzurePlayground.Models.Security;
using AzurePlayground.Queries.Security;
using AzurePlayground.QueryHandlers.Decorators;
using AzurePlayground.Utilities.Container;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzurePlayground.QueryHandlers.Security {
    [InterfaceInjectable]
    [Audit]
    public sealed class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, IList<UserViewModel>> {
        private readonly IPlaygroundContext _context;

        public GetUsersQueryHandler(IPlaygroundContext context) {
            _context = context;
        }

        public IList<UserViewModel> Execute(GetUsersQuery query) {
            return _context.Users
                .Where(u => u.Status == UserStatus.Active)
                .OrderBy(u => u.Id)
                .Select(u => new UserViewModel() {
                    Id = u.Id,
                    DisplayName = u.DisplayName ?? "Anonymous",
                    Email = u.ShowEmail ? u.Email : "Private",
                    Description = u.Description,
                    StartDate = u.UserEvents.Min(e => (DateTime?)e.Date)
                })
                .ToList();
        }
    }
}