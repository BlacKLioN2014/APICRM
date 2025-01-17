using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICRM.Models.Mapper
{
    public class DocumentLines
    {
        [Required]
        public string? ItemCode { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int DiscountPercent { get; set; }

        public string? WarehouseCode { get; set; }

        public string? BarCode { get; set; }

        public string? TaxCode { get; set; }

        [Required]
        public int UnitPrice { get; set; }

        [Required]
        public int ReturnReason { get; set; }

        [Required]
        public int ReturnAction { get; set; }


    }
}
