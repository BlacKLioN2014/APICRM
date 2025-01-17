using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICRM.Models.Mapper
{
    public class DocumentReferences
    {

        public int LineNumber { get; set; }

        public int RefDocEntr { get; set; }

        [Required]
        public int RefDocNum { get; set; }

        public string? RefObjType { get; set; }
    }
}
