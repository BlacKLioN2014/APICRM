using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICRM.Models
{
    public class Conflic
    {

        [Required]
        [JsonProperty("Codigo")]
        public int code { get; set; }

        [Required]
        [JsonProperty("Descripcion")]
        public string? Description { get; set; }

    }
}
