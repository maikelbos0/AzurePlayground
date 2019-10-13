using AzurePlayground.Domain.Security;
using System.Collections.Generic;

namespace AzurePlayground.Database.ReferenceEntities {
    internal class UserStatusEntity : BaseReferenceEntity<UserStatus> {
        public virtual ICollection<User> Users { get; set; }
    }
}