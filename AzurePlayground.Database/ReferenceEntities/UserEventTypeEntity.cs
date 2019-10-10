using AzurePlayground.Domain.Security;
using System.Collections.Generic;

namespace AzurePlayground.Database.ReferenceEntities {
    internal class UserEventTypeEntity : BaseReferenceEntity<UserEventType> {
        public virtual ICollection<UserEvent> UserEvents { get; set; }
    }
}