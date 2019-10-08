using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace AzurePlayground.Domain.Security {
    public class Password : ValueObject<Password> {
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

        protected override IEnumerable<object> GetEqualityComponents() {
            yield return Salt.Length;
            foreach (var s in Salt) {
                yield return s;
            }

            yield return Hash.Length;
            foreach (var s in Hash) {
                yield return s;
            }

            yield return HashIterations;
            yield return ExpiryDate;
        }
    }
}