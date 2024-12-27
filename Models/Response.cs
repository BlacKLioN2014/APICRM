using Newtonsoft.Json;

namespace APICRM.Models
{
    public class Response<T>
    {

        [JsonProperty("Éxito")]
        public bool success { get; set; }

        [JsonProperty("Respuesta")]
        public T? answer { get; set; }

    }
}
