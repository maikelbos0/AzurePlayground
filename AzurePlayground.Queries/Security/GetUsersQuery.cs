using AzurePlayground.Models.Security;
using System.Collections.Generic;

namespace AzurePlayground.Queries.Security {
    public sealed class GetUsersQuery : IQuery<IEnumerable<UserViewModel>> {
    }
}