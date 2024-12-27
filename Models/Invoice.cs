using Newtonsoft.Json;

namespace APICRM.Models
{
    public class Invoice
    {
        [JsonProperty("CodigoSAP")]
        public string? CardCode { get; set; }

        [JsonProperty("Nombre")]
        public string? CardName { get; set; }

        [JsonProperty("Fecha")]
        public string? DocDate { get; set; }

        [JsonProperty("Almacén")]
        public string? U_Sucursal { get; set; }

        [JsonProperty("Artículos")]
        public List<Item>? Items { get; set; }

    }
}
