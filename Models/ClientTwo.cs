using Newtonsoft.Json;

namespace APICRM.Models
{
    public class ClientTwo
    {
        [JsonProperty("CodigoCliente")]
        public string? CardCode { get; set; }

        [JsonProperty("Nombre")]
        public string? CardName { get; set; }

        [JsonProperty("NombreExtranjero")]
        public string? CardFName { get; set; }

        [JsonProperty("Telefono")]
        public string? Phone1 { get; set; }

        [JsonProperty("Correo")]
        public string? E_Mail { get; set; }

        [JsonProperty("Contado")]
        public string? counted { get; set; }

        [JsonProperty("Credito")]
        public string? credit { get; set; }

        [JsonProperty("CodigoCondicionDePago")]
        public string? GroupNum { get; set; }
        [JsonProperty("CondicionDePago")]
        public string? PymntGroup { get; set; }
    }
}
