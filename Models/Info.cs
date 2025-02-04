using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICRM.Models
{
    public class Info
    {
        [Required]
        [JsonProperty("InformacionDeLinea")]
        public List<ItemInfo> InfoItemCode { get; set; }
    }
}
