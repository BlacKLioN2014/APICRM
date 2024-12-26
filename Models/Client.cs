using Newtonsoft.Json;

namespace APICRM.Models
{
    public class Client
    {

        [JsonProperty("CodigoSAP")]
        public string? CardCode { get; set; }

        [JsonProperty("Nombre")]
        public string? CardName { get; set; }

        [JsonProperty("NombreExtranjero")]
        public string? CardFName { get; set; }

        [JsonProperty("Telefono")]
        public string? Phone1 { get; set; }

        [JsonProperty("Correo")]
        public string? E_Mail { get; set; }

        //[JsonProperty("CodigoEmpleado")]
        //public string? SlpCode { get; set; }

        [JsonProperty("InformacionCrediticia")]
        public CreditInformation? CreditInformation { get; set; }

    }
}
