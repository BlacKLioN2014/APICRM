using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICRM.Models
{
    public class ItemInfo
    {
        [Required]
        [JsonProperty("Folio")]
        public int DocNum { get; set; }

        [Required]
        [JsonProperty("CodigoArticulo")]
        public string ItemCode { get; set; }

        [Required]
        [JsonProperty("Cantidad")]
        public decimal Quantity { get; set; }

        [Required]
        [JsonProperty("Precio")]
        public decimal PriceBefdi { get; set; }

        [Required]
        [JsonProperty("CodigoFiscal")]
        public string TaxCode { get; set; }

        [Required]
        [JsonProperty("Lote")]
        public string BatchNum { get; set; }

        [JsonProperty("Descuento")]
        public decimal DiscPrcnt { get; set; }

        [Required]
        [JsonProperty("Descripcion")]
        public string Dscription { get; set; }

    }
}
