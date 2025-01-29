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
        public int Quantity { get; set; }

        [Required]
        [JsonProperty("Precio")]
        public float PriceBefdi { get; set; }

        [Required]
        [JsonProperty("CodigoFiscal")]
        public string TaxCode { get; set; }

        [Required]
        [JsonProperty("Lote")]
        public string BatchNum { get; set; }

        [JsonProperty("Descuento")]
        public float DiscPrcnt { get; set; }



    }
}
