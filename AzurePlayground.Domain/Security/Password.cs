using System;
using System.Linq;
using System.Security.Cryptography;

namespace AzurePlayground.Domain.Security {
    public class Password {
        protected const int _hashIterations = 1000;

        public static Password None => new Password();

        public byte[] Salt { get; private set; } = new byte[0];
        public byte[] Hash { get; private set; } = new byte[0];
        public int HashIterations { get; private set; }
        public DateTime? ExpiryDate { get; private set; }

        private Password() {
        }

        public Password(string password) : this() {
            Salt = GetNewSalt();
            HashIterations = _hashIterations;
            Hash = GetHash(password);
        }

        public Password(string password, DateTime expiryDate) : this(password) {
            ExpiryDate = expiryDate;
        }

        private byte[] GetNewSalt() {
            using (var rng = new RNGCryptoServiceProvider()) {
                byte[] salt = new byte[20];

                rng.GetBytes(salt);

                return salt;
            }
        }

        protected byte[] GetHash(string password) {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, Salt, HashIterations)) {

                // Return 20 bytes because after that it repeats
                return pbkdf2.GetBytes(20);
            }
        }

        // TODO make general lift to base class for value objects
        public override bool Equals(object obj) {
            var password = obj as Password;

            if (ReferenceEquals(password, null))
                return false;

            return Salt.SequenceEqual(password.Salt)
                && Hash.SequenceEqual(password.Hash)
                && HashIterations.Equals(password.HashIterations)
                && Equals(ExpiryDate, password.ExpiryDate);
        }

        // TODO make general lift to base class for value objects
        public override int GetHashCode() {
            var hashCode = Salt.GetHashCode();

            hashCode = 23 * hashCode + Hash.GetHashCode();
            hashCode = 23 * hashCode + HashIterations.GetHashCode();
            hashCode = 23 * hashCode + ExpiryDate.GetHashCode();

            return hashCode;
        }

        public static bool operator ==(Password a, Password b) {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
                return true;

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Password a, Password b) {
            return !(a == b);
        }
    }
}