using Newtonsoft.Json;

namespace APICRM.Models
{
    public class Invoice
    {
        [JsonProperty("CodigoCliente")]
        public string? CardCode { get; set; }

        [JsonProperty("Nombre")]
        public string? CardName { get; set; }

        [JsonProperty("Fecha")]
        public string? DocDate { get; set; }

        [JsonProperty("Almacen")]
        public string? U_Sucursal { get; set; }

        [JsonProperty("Paqueteria")]
        public string? Parcels { get; set; }

        [JsonProperty("Guias")]
        public List<string>? Guides { get; set; }

        [JsonProperty("Artículos")]
        public List<Item>? Items { get; set; }

    }
}
