using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICRM.Models
{
    public class DocumentLines
    {
        [Required]
        [JsonProperty("CodigoDeArticulo")]
        public string? ItemCode { get; set; }

        [Required]
        [JsonProperty("Cantidad")]
        public int Quantity { get; set; }

        [Required]
        [JsonProperty("Descuento")]
        public int DiscountPercent { get; set; }

        [JsonProperty("CodigoDeBarras")]
        public string? BarCode { get; set; }

        [JsonProperty("CodigoFiscal")]
        public string? TaxCode { get; set; }

        [Required]
        [JsonProperty("Precio")]
        public int UnitPrice { get; set; }

        [Required]
        [JsonProperty("MotivoIncidencia")]
        public int ReturnReason { get; set; }

        [Required]
        [JsonProperty("FlujoOAccion")]
        public int ReturnAction { get; set; }


    }
}
