using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICRM.Models
{
    public class FindInvoice
    {
        [Required(ErrorMessage = "El campo factura es obligatorio.")]
        [JsonProperty("Folio")]
        public int DocNum { get; set; }
    }
}
