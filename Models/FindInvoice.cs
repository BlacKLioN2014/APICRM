using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICRM.Models
{
    public class FindInvoice
    {
        [Required(ErrorMessage = "El campo DocNum es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El valor debe ser un folio de factura.")]
        [JsonProperty("Folio")]
        public int DocNum { get; set; }
    }
}
