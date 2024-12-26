using Newtonsoft.Json;

namespace APICRM.Models
{
    public class CreditInformation
    {

        [JsonProperty("SaldoVencido")]
        public decimal Balance { get; set; }

        [JsonProperty("Limite")]
        public decimal Limit { get; set; }

        [JsonProperty("Disponible")]
        public decimal available { get; set; }

    }
}
