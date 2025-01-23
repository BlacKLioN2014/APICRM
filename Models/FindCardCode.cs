using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICRM.Models
{
    public class FindCardCode
    {

        [Required(ErrorMessage = "El campo CardCode es obligatorio.")]
        [JsonProperty("CodigoDeCliente")]
        public string? CardCode { get; set; }

    }
}
