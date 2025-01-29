using Swashbuckle.AspNetCore.Filters;

namespace APICRM.Models
{
    public class RequestExample : IExamplesProvider<returnRequest>
    {
        public returnRequest GetExamples()
        {
            return new returnRequest
            {
                CardCode = "100729",
                DocDate = DateTime.Parse("2025-01-16T23:08:24.125Z"),
                U_Sucursal = "01",
                U_RFCondiciones = "03",
                NumAtCard = "OOSP950629K58",
                Series = "77",
                PaymentGroupCode = "2",
                PaymentMethod = "99",
                Comments = "Solicitud de devolución generada desde Fresh.",
                SalesPersonCode = "32",
                U_B1SYS_MainUsage = "S01",
                U_CIncidencia = "Ventas10",
                U_TimeInc = "2025-01-16T23:08:24",
                U_StatusInc = "Pendiente",
                DocumentReferences = new List<DocumentReferences>
                {
                    new DocumentReferences
                    {
                        LineNumber = 1,
                        RefDocEntr = 120690,
                        RefDocNum = 90430,
                        RefObjType = "rot_SalesInvoice"
                    }
                },
                DocumentLines = new List<DocumentLines>
                {
                new DocumentLines
                {
                    ItemCode = "BIO-ANTD0001",
                    Quantity = 4,
                    DiscountPercent = 10,
                    //WarehouseCode = "01",
                    BarCode = "3461029800007",
                    TaxCode = "IVAV16",
                    UnitPrice = 100,
                    ReturnReason = 5,
                    U_MotCancel = "5"
                }
            }
            };
        }
    }
}
