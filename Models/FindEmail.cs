using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICRM.Models
{
    public class FindEmail
    {
        [Required(ErrorMessage = "El campo Correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El campo Correo no tiene un formato válido.")]
        [JsonProperty("CorreoElectronico")]
        public string Email { get; set; }
    }
}
