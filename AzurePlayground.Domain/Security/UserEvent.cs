using System;

namespace AzurePlayground.Domain.Security {
    public class UserEvent {
        public int Id { get; set; }
        public User User { get; set; }
        public DateTime Date { get; set; }
        public UserEventType UserEventType { get; private set; }
        public byte UserEventType_Id { get; set; }
    }
}