using Newtonsoft.Json;

namespace APICRM.Models
{
    public class Item
    {

        [JsonProperty("CodigoArticulo")]
        public string? ItemCode { get; set; }

        [JsonProperty("Descripcion")]
        public string? Dscription { get; set; }

        [JsonProperty("CodigoBarras")]
        public string? CodeBars { get; set; }

        [JsonProperty("Cantidad")]
        public string? Quantity { get; set; }

        [JsonProperty("Precio")]
        public string? PriceBefDi { get; set; }

        [JsonProperty("Descuento")]
        public string? DiscPrcnt { get; set; }

        [JsonProperty("CodigoFiscal")]
        public string? TaxCode { get; set; }

    }
}
