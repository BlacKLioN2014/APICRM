using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICRM.Models
{
    public class Guide
    {
        [Required]
        [JsonProperty("Guia")]
        public string? NumberOfGuide { get; set; }
    }
}
