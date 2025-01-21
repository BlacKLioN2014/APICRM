using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace APICRM.Models
{
    public class returnRequest
    {

        #region Campos de Cabecera

        [Required]
        [JsonProperty("CodigoDeCliente")]
        public string? CardCode { get; set; }

        [JsonProperty("FechaDeCreación")]
        public DateTime DocDate { get; set; }

        [JsonProperty("Sucursal")]
        public string? U_Sucursal { get; set; }

        [JsonProperty("RFCGenerico")]
        public string? U_RFCondiciones { get; set; }

        [JsonProperty("RFCliente")]
        public string? NumAtCard { get; set; }

        [JsonProperty("Folio")]
        public string? Series { get; set; }

        [JsonProperty("CondicionDePago")]
        public string? PaymentGroupCode { get; set; }

        [JsonProperty("FormaDePago")]
        public string? PaymentMethod { get; set; }

        [JsonProperty("Comentarios")]
        public string? Comments { get; set; }

        [JsonProperty("EjecutivoDeVentas")]
        public string? SalesPersonCode { get; set; }

        [JsonProperty("UsoPrincipal")]
        public string? U_B1SYS_MainUsage { get; set; }

        [JsonProperty("Usuario")]
        public string? U_CIncidencia { get; set; }

        [JsonProperty("HoraIncidencia")]
        public string? U_TimeInc { get; set; }

        [JsonProperty("EstatusIncidencia")]
        public string? U_StatusInc { get; set; }

        #endregion


        #region Referencias

        public List<DocumentReferences>? DocumentReferences { get; set; }

        #endregion


        #region Partidas

        public List<DocumentLines>? DocumentLines { get; set; }

        #endregion
    }
}
