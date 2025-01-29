using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICRM.Models
{
    public class InfoLine
    {
        [Required(ErrorMessage = "El campo Folio es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El valor debe ser un folio de factura.")]
        [JsonProperty("Folio")]
        public int DocNum { get; set; }

        [Required(ErrorMessage = "El campo CodigoArticulo es obligatorio.")]
        [JsonProperty("CodigoArticulo")]
        public string ItemCode { get; set; }
    }
}
