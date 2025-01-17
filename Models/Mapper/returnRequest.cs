using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace APICRM.Models.Mapper
{
    public class returnRequest
    {


        [Required]
        public string? CardCode { get; set; }

        public DateTime DocDate { get; set; }

        public string? U_Sucursal { get; set; }

        public string? U_RFCondiciones { get; set; }

        public string? NumAtCard { get; set; }

        public string? Series { get; set; }

        public string? PaymentGroupCode { get; set; }

        public string? PaymentMethod { get; set; }

        public string? Comments { get; set; }

        public string? SalesPersonCode { get; set; }

        public string? U_B1SYS_MainUsage { get; set; }

        public string? U_CIncidencia { get; set; }

        public DateTime U_TimeInc { get; set; }

        public string? U_StatusInc { get; set; }



        public List<DocumentReferences>? DocumentReferences { get; set; }




        public List<DocumentLines>? DocumentLines { get; set; }

    }
}
