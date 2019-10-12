using System;

namespace AzurePlayground.Domain.Security {
    public class UserEvent : Entity {
        public User User { get; set; }
        public DateTime Date { get; set; }
        public UserEventType Type { get; set; }
    }
}