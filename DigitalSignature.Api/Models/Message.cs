using System.Security.Cryptography;

namespace DigitalSignature.Api.Models
{
    /// <summary>
    /// Подписанное сообщение
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Захэшированное сообщение
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Подписанное сообщения
        /// </summary>
        public string SignedHash { get; set; }

        /// <summary>
        /// Публичный ключ ЭЦП
        /// </summary>
        public PublicKeyDto PublicKey { get; set; }
    }
}
