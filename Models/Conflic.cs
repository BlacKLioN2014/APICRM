using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICRM.Models
{
    public class Conflic
    {

        [Required]
        [JsonProperty("Código")]
        public int code { get; set; }

        [Required]
        [JsonProperty("Descripción")]
        public string? Description { get; set; }

    }
}
