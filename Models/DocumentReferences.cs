using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICRM.Models
{
    public class DocumentReferences
    {

        [JsonProperty("NumeroDeLinea")]
        public int LineNumber { get; set; }

        [JsonProperty("CodigoInternoFactura")]
        public int RefDocEntr { get; set; }

        [Required]
        [JsonProperty("NumeroDeFactura")]
        public int RefDocNum { get; set; }

        [JsonProperty("TipoDeReferencia")]
        public string? RefObjType { get; set; }

    }
}
