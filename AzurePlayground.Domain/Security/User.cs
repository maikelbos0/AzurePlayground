using System;
using System.Collections.Generic;

namespace AzurePlayground.Domain.Security {
    public class User : Entity {
        public UserStatus Status { get; set; }
        public string Email { get; set; }
        public int? ActivationCode { get; set; }
        public Password Password { get; set; }
        public TemporaryPassword PasswordResetToken { get; set; } = TemporaryPassword.None;
        public ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();

        public void AddEvent(UserEventType userEventType) {            
            UserEvents.Add(new UserEvent() {
                Date = DateTime.UtcNow,
                Type = userEventType
            });
        }
    }
}