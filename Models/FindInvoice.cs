using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICRM.Models
{
    public class FindInvoice
    {
        [Required(ErrorMessage = "El campo DocNum es obligatorio.")]
        [JsonProperty("Folio")]
        public int DocNum { get; set; }
    }
}
