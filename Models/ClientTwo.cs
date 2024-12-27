using Newtonsoft.Json;

namespace APICRM.Models
{
    public class ClientTwo
    {
        [JsonProperty("CodigoSAP")]
        public string? CardCode { get; set; }

        [JsonProperty("Nombre")]
        public string? CardName { get; set; }

        [JsonProperty("NombreExtranjero")]
        public string? CardFName { get; set; }

        [JsonProperty("Teléfono")]
        public string? Phone1 { get; set; }

        [JsonProperty("Correo")]
        public string? E_Mail { get; set; }

        [JsonProperty("Contado")]
        public string? counted { get; set; }

        [JsonProperty("Crédito")]
        public string? credit { get; set; }
    }
}
