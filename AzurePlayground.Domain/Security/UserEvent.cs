using System;

namespace AzurePlayground.Domain.Security {
    public class UserEvent : Entity {
        public User User { get; protected set; }
        public DateTime Date { get; protected set; }
        public UserEventType Type { get; protected set; }

        protected UserEvent() {
        }

        public UserEvent(User user, UserEventType type) {
            User = user;
            Date = DateTime.UtcNow;            
            Type = type;
        }
    }
}