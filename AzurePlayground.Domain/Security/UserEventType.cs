using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AzurePlayground.Domain.Security {
    public class UserEventType {
        private static IReadOnlyList<UserEventType> _values;

        public static UserEventType LoggedIn { get; } = new UserEventType(1, "Logged in");
        public static UserEventType FailedLogIn { get; } = new UserEventType(2, "Failed log in");
        public static UserEventType Registered { get; } = new UserEventType(3, "Registered");
        public static UserEventType Activated { get; } = new UserEventType(4, "Activated");
        public static UserEventType FailedActivation { get; } = new UserEventType(5, "Failed activation");
        public static UserEventType ActivationCodeSent { get; } = new UserEventType(6, "Activation code sent");
        public static UserEventType LoggedOut { get; } = new UserEventType(7, "Logged out");
        public static UserEventType PasswordChanged { get; } = new UserEventType(8, "Password changed");
        public static UserEventType FailedPasswordChange { get; } = new UserEventType(9, "Failed password change");
        public static UserEventType PasswordResetRequested { get; } = new UserEventType(10, "Password reset requested");
        public static UserEventType PasswordReset { get; } = new UserEventType(11, "Password reset");
        public static UserEventType FailedPasswordReset { get; } = new UserEventType(12, "Failed password reset");
        
        public static IReadOnlyList<UserEventType> GetValues() {
            if (_values == null) {
                _values = typeof(UserEventType).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty)
                    .Where(p => p.PropertyType.Equals(typeof(UserEventType)))
                    .Select(p => (UserEventType)p.GetValue(null))
                    .ToList()
                    .AsReadOnly();
            }

            return _values;
        }

        public UserEventType() {
        }

        public UserEventType(byte id, string name) : base() {
            Id = id;
            Name = name;
        }

        public byte Id { get; private set; }
        public string Name { get; private set; }
        public virtual ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
    }
}