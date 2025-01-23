using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICRM.Models
{
    public class LastInvoices
    {
        [Required(ErrorMessage = "El campo DocNum es obligatorio.")]
        [JsonProperty("Folio")]
        public string? DocNum  { get; set; }

        [Required(ErrorMessage = "El campo DocEntry es obligatorio.")]
        [JsonProperty("FolioInterno")]
        public string? DocEntry  { get; set; }

        [Required(ErrorMessage = "El campo DocDate es obligatorio.")]
        [JsonProperty("FechaDeFolio")]
        public DateTime DocDate { get; set; }
    }
}
