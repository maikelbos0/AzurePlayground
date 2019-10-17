using System;
using System.Security.Cryptography;
using System.Text;

namespace AzurePlayground.CommandHandlers.Security {
    public abstract class BaseUserCommandHandler {
        private readonly char[] _tokenCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

        protected int GetNewActivationCode() {
            return new Random().Next(10000, int.MaxValue);
        }

        protected string GetNewPasswordResetToken() {
            using (var rng = new RNGCryptoServiceProvider()) {
                // Establish a maximum based on the amount of characters to prevent bias
                var maximumNumber = (byte.MaxValue / _tokenCharacters.Length) * _tokenCharacters.Length;
                var tokenBuilder = new StringBuilder();
                byte[] buffer = new byte[1];

                for (var i = 0; i < 20; i++) {
                    // Get a new number as long as we're at or over the maximum number
                    do {
                        rng.GetBytes(buffer);
                    }
                    while (buffer[0] >= maximumNumber);

                    tokenBuilder.Append(_tokenCharacters[buffer[0] % _tokenCharacters.Length]);
                }

                return tokenBuilder.ToString();
            }
        }
    }
}