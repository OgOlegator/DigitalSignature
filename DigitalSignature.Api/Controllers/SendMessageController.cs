using DigitalSignature.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace DigitalSignature.Api.Controllers
{    
    /// <summary>
    /// Отправить подписанное сообщение
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SendMessageController : ControllerBase
    {
        /// <summary>
        /// Подписать сообщение
        /// </summary>
        /// <param name="message"></param>
        [HttpGet]
        [Route("Sign/{message}")]
        public Message SignMessage(string message)
        {
            //Хэшируем сообщение
            var alg = SHA256.Create();
            var data = Encoding.ASCII.GetBytes(message);
            var hashMsg = alg.ComputeHash(data);

            RSAParameters publicKey;
            byte[] signedHashMsg;

            //Генерируем ключ и подписываем сообщение
            using (RSA rsa = RSA.Create())
            {
                publicKey = rsa.ExportParameters(false);

                var rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
                
                rsaFormatter.SetHashAlgorithm(nameof(SHA256));

                signedHashMsg = rsaFormatter.CreateSignature(hashMsg);
            }

            return new Message
            {
                Text = message,
                SignedHash = Convert.ToBase64String(signedHashMsg),
                PublicKey = new PublicKeyDto
                {
                    Exponent = Convert.ToBase64String(publicKey.Exponent),
                    Modulus  = Convert.ToBase64String(publicKey.Modulus)
                },
            };
        }

        /// <summary>
        /// Отправить подписанное сообщение и верифицировать
        /// </summary>
        [HttpPost]
        [Route("Verify")]
        public string VerifyMessage([FromBody] Message message)
        {
            var publicKeySender = new RSAParameters
            {
                Exponent = Convert.FromBase64String(message.PublicKey.Exponent),
                Modulus = Convert.FromBase64String(message.PublicKey.Modulus),
            };

            var alg = SHA256.Create();
            var messageByteArr = Encoding.ASCII.GetBytes(message.Text);
            var hashMsg = alg.ComputeHash(messageByteArr);

            using (RSA rsa = RSA.Create())
            {
                rsa.ImportParameters(publicKeySender);

                RSAPKCS1SignatureDeformatter rsaDeformatter = new(rsa);

                rsaDeformatter.SetHashAlgorithm(nameof(SHA256));

                if (rsaDeformatter.VerifySignature(hashMsg, Convert.FromBase64String(message.SignedHash)))
                {
                    return "The signature is valid.";
                }
                else
                {
                    return "The signature is not valid.";
                }
            }
        }

    }
}
