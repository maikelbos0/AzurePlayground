using System.Collections.Generic;
using System.Linq;

namespace AzurePlayground.Domain {
    public abstract class ValueObject<T> where T : ValueObject<T> {
        // Care must be made to only return only value objects as equality components to prevent it using only reference equality
        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object obj) {
            var other = obj as T;

            if (ReferenceEquals(other, null)) {
                return false;
            }

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode() {
            return GetEqualityComponents().Aggregate(1, (current, obj) => current * 23 + (obj?.GetHashCode() ?? 0));
        }

        public static bool operator ==(ValueObject<T> a, ValueObject<T> b) {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null)) {
                return true;
            }

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null)) {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(ValueObject<T> a, ValueObject<T> b) {
            return !(a == b);
        }
    }
}