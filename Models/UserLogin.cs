using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICRM.Models
{
    public class UserLogin
    {

        [Required(ErrorMessage = "El campo Usuario es obligatorio.")]
        [JsonProperty("Usuario")]
        public string User { get; set; }

        [Required(ErrorMessage = "El campo Contraseña es obligatorio.")]
        [JsonProperty("Contraseña")]
        public string Password { get; set; }

    }
}
