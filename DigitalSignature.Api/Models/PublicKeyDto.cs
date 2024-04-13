namespace DigitalSignature.Api.Models
{
    /// <summary>
    /// Модель передачи публичного ключа ЭЦП
    /// </summary>
    public class PublicKeyDto
    {
        public string Exponent { get; set; }

        public string Modulus { get; set; }
    }
}
