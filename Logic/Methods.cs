using APICRM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Sap.Data.Hana;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using AutoMapper;
using APICRM.Models.Mapper;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Mail;
using System.Net;
using System.Xml.Linq;

namespace APICRM.Logic
{
    public class Methods
    {

        private readonly  string Secreta = string.Empty;
        public readonly  string UserApi = string.Empty;
        public readonly string PasswordApi = string.Empty;
        private readonly string HanaConec = string.Empty;
        private readonly string DBYes = string.Empty;
        private readonly string DBNo = string.Empty;
        private readonly string productive = string.Empty;
        private readonly string UserSAP = string.Empty;
        private readonly string PassWordSAP = string.Empty;
        private readonly string Mail = string.Empty;
        private readonly string Path = $@"\\172.16.21.249\b1_shf\Companies\SB1CSL\anexos\BOVEDA FACTURAS MEPIEL\";

        IConfiguration _config;
        private readonly IMapper _mapper;

        public Methods(IConfiguration config, IMapper mapper)
        {

            _config = config;
            _mapper = mapper;
            Secreta = _config.GetValue<string>("ApiSettings:Secreta");
            UserApi = _config.GetValue<string>("ApiSettings:UserApi");
            PasswordApi = _config.GetValue<string>("ApiSettings:PasswordApi");
            HanaConec = _config.GetValue<string>("ApiSettings:HanaConec");
            DBYes = _config.GetValue<string>("ApiSettings:DBYes");
            DBNo = _config.GetValue<string>("ApiSettings:DBNo");
            productive = _config.GetValue<string>("ApiSettings:Productive");
            UserSAP = _config.GetValue<string>("ApiSettings:UserSAP");
            PassWordSAP = _config.GetValue<string>("ApiSettings:PassWordSAP");
            Mail = _config.GetValue<string>("ApiSettings:MailRemitente");
        }

        public  string generarToKen(UserLogin UserLogin)
        {

            string Token = string.Empty;

            try
            {

                var manejadorToken = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(Secreta);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, UserLogin.User),
                    new Claim(ClaimTypes.Role, UserLogin.Password)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(30),
                    SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = manejadorToken.CreateToken(tokenDescriptor);

                Token = manejadorToken.WriteToken(token);

            }
            catch (Exception x)
            {

                Token = $@" Error: {x.ToString()}";

            }

            return Token;

        }

        public UserLogin desifrarToken(string token)
        {
            var manejadorToken = new JwtSecurityTokenHandler();
            var jwtToken = manejadorToken.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                return null;

            // Asegúrate de que estos ClaimTypes coincidan con los que usaste al crear el token
            var usuario = jwtToken?.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;
            var rol = jwtToken?.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            return new UserLogin
            {
                User = usuario,
                Password = rol // Usando rol como ejemplo
            };
        }

        public async Task<List<Client>>  GetClients()
        {
            List<Client> Lista = new List<Client>();

            string DB = string.Empty;

            string StrSql = string.Empty;

            try
            {

                using (var Con = new HanaConnection(HanaConec))
                {
                    await Con.OpenAsync();
                    DB = productive == "YES" ? DBYes : DBNo;

                    StrSql = $@"
                                SELECT
                                    T0.""CardCode"",
                                    T0.""CardName"",
                                    T0.""CardFName"",
                                    T0.""Phone1"",
                                    T0.""E_Mail"",
                                    T1.""PymntGroup"",

                                    CASE 
                                        WHEN T0.""U_CodigoAgrupador"" IS NULL THEN IFNULL(T0.""CreditLine"", 0) 
                                        WHEN T0.""U_CodigoAgrupador"" IS NOT NULL THEN (
                                            SELECT 
                                                SUM(IFNULL(T100.""CreditLine"", 0))
                                            FROM 
                                                {DB}.OCRD T100 
                                            WHERE 
                                                T100.""U_CodigoAgrupador"" = T0.""U_CodigoAgrupador""
                                        )
                                    END AS ""Limite de credito"",

                                    CASE 
                                        WHEN T0.""U_CodigoAgrupador"" IS NULL THEN IFNULL(T0.""Balance"", 0) 
                                        WHEN T0.""U_CodigoAgrupador"" IS NOT NULL THEN (
                                            SELECT 
                                                SUM(IFNULL(T100.""Balance"", 0))
                                            FROM
                                                {DB}.OCRD T100 
                                            WHERE 
                                                T100.""U_CodigoAgrupador"" = T0.""U_CodigoAgrupador""
                                        )
                                    END AS ""Saldo"",

                                    CASE
                                        WHEN T0.""U_CodigoAgrupador"" IS NULL THEN (IFNULL(T0.""CreditLine"", 0) - T0.""Balance"")
                                        WHEN T0.""U_CodigoAgrupador"" IS NOT NULL THEN (
                                            SELECT
                                                (SUM(IFNULL(T100.""CreditLine"", 0)) - SUM(T100.""Balance""))
                                            FROM 
                                                {DB}.OCRD T100
                                            WHERE 
                                                T100.""U_CodigoAgrupador"" = T0.""U_CodigoAgrupador""
                                        )
                                    END AS ""Disponible""

                                FROM  
                                    {DB}.OCRD T0  
                                    INNER JOIN {DB}.OCTG T1 ON T0.""GroupNum"" = T1.""GroupNum""

                                WHERE  
                                    T0.""validFor"" = 'Y'
                                    AND T0.""CardType"" = 'C' 

                                ORDER BY 
                                    T0.""CardCode"" ASC
";

                    using (var cmd = new HanaCommand(StrSql, Con))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            var client = new Client()
                            {
                                CardCode = reader.GetString(0),
                                CardName = reader.IsDBNull(1) ? "No data" : reader.GetString(1),
                                CardFName = reader.IsDBNull(2) ? "No data" : reader.GetString(2),
                                Phone1 = reader.IsDBNull(3) ? "No data" : reader.GetString(3),
                                E_Mail = reader.IsDBNull(4) ? "No data" : reader.GetString(4),
                            };

                            var CreditInformation = new CreditInformation()
                            {
                                GroupNum = reader.IsDBNull(5) ? "No data" : reader.GetString(5),
                                Limit = reader.GetDecimal(6),
                                Balance = reader.GetDecimal(7),
                                available = reader.GetDecimal(8),
                            };
                            client.CreditInformation = CreditInformation;

                            Lista.Add(client);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string error = "Error.  Al obtener listado de clientes. " + ex.Message.ToString();
                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");

                return new List<Client>();

            }
            return Lista;

        }

        public async Task<List<ClientTwo>> GetClient(string email)
        {
            List<ClientTwo> Lista = new List<ClientTwo>();

            string DB = string.Empty;

            string StrSql = string.Empty;

            try
            {

                using (var Con = new HanaConnection(HanaConec))
                {
                    await Con.OpenAsync();
                    DB = productive == "YES" ? DBYes : DBNo;

                    StrSql = $@"
                                SELECT
                                    T0.""CardCode"",
                                    T0.""CardName"",
                                    T0.""CardFName"",
                                    T0.""Phone1"",
                                    T0.""E_Mail"",
                                    CASE WHEN T0.""QryGroup1"" = 'Y' THEN 'SI' ELSE 'NO' END AS ""Contado"", 
                                    CASE WHEN T0.""QryGroup2"" = 'Y' THEN 'SI' ELSE 'NO' END AS ""Credito"",
                                    T0.""GroupNum"",
                                    T1.""PymntGroup""
                                    

                                FROM  
                                    {DB}.OCRD T0  
                                    INNER JOIN {DB}.OCTG T1 ON T0.""GroupNum"" = T1.""GroupNum""

                                WHERE  
                                    T0.""validFor"" = 'Y'
                                    AND T0.""CardType"" = 'C' 
                                    AND T0.""E_Mail"" = '{email}' 

                                ORDER BY 
                                    T0.""CardCode"" ASC
";

                    using (var cmd = new HanaCommand(StrSql, Con))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            var client = new ClientTwo()
                            {
                                CardCode = reader.GetString(0),
                                CardName = reader.IsDBNull(1) ? "No data" : reader.GetString(1),
                                CardFName = reader.IsDBNull(2) ? "No data" : reader.GetString(2),
                                Phone1 = reader.IsDBNull(3) ? "No data" : reader.GetString(3),
                                E_Mail = reader.IsDBNull(4) ? "No data" : reader.GetString(4),
                                counted = reader.GetString(5),
                                credit = reader.GetString(6),
                                GroupNum = reader.IsDBNull(7) ? "No data" : reader.GetString(7),
                                PymntGroup = reader.IsDBNull(8) ? "No data" : reader.GetString(8),
                            };

                            Lista.Add(client);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string error = "Error. Al obtener cliente. " + ex.Message.ToString();
                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");

                return new List<ClientTwo>();

            }
            return Lista;

        }

        public async Task<List<LastInvoices>> Invoices(string CardCode)
        {
            var Lista = new List<LastInvoices>();

            string DB = string.Empty;

            string StrSql = string.Empty;

            try
            {

                using (var Con = new HanaConnection(HanaConec))
                {
                    await Con.OpenAsync();
                    DB = productive == "YES" ? DBYes : DBNo;

                    StrSql = $@"
                                SELECT 
	                                T0.""DocNum"",
	                                T0.""DocEntry"",
	                                T0.""DocDate""
                                FROM 
	                                {DB}.OINV T0
                                WHERE
	                                T0.""CardCode"" = '{CardCode}'
                                ORDER BY
	                                T0.""DocDate"" DESC
                                LIMIT 5
";

                    using (var cmd = new HanaCommand(StrSql, Con))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            var invoice = new LastInvoices()
                            {
                                DocNum = reader.GetString(0),
                                DocEntry = reader.GetString(1),
                                DocDate = reader.GetDateTime(2),
                            };

                            Lista.Add(invoice);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string error = "Error. Al obtener listado de facturas. " + ex.Message.ToString();
                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");

                return new List<LastInvoices>();

            }
            return Lista;

        }

        public async Task<Invoice> GetInvoice(int DocNum)
        {

            string guias = string.Empty;

            var client = new Invoice();

            string DB = string.Empty;

            string StrSql = string.Empty;

            try
            {

                using (var Con = new HanaConnection(HanaConec))
                {
                    await Con.OpenAsync();
                    DB = productive == "YES" ? DBYes : DBNo;

                    StrSql = $@"
                                SELECT
	                                T0.""CardCode"",
	                                T0.""CardName"",
	                                TO_VARCHAR(T0.""DocDate"", 'YYYY-DD-MM') AS ""FechaFormateada"",
	                                CASE 
		                                WHEN T0.""U_Sucursal"" = '01' THEN 'Guadalajara' 
		                                WHEN T0.""U_Sucursal"" = '02' THEN 'CDMX'
		                                WHEN T0.""U_Sucursal"" = '03' THEN 'Monterrey'
		                                ELSE 'Salto' 
	                                END AS ""Almacen"",
                                    CASE 
		                                WHEN T0.""U_TypeGuide"" = '01' THEN 'DHL' 
		                                WHEN T0.""U_TypeGuide"" = '02' THEN 'ESTAFETA'
		                                ELSE 'Logistica propia' 
	                                END AS ""Paqueria"",
                                    CASE 
		                                WHEN T0.""U_TypeGuide"" = '01' THEN T0.""U_DHLGuia""
		                                WHEN T0.""U_TypeGuide"" = '02' THEN T0.""U_EstafetaGuia""
		                                ELSE 'NA' 
	                                END AS ""Guias""


	
                                FROM
	                                {DB}.OINV T0
	
                                WHERE
	                                T0.""DocNum"" = '{DocNum}'";

                    using (var cmd = new HanaCommand(StrSql, Con))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            client = new Invoice()
                            {
                                CardCode = reader.GetString(0),
                                CardName = reader.IsDBNull(1) ? "No data" : reader.GetString(1),
                                DocDate = reader.GetString(2),
                                U_Sucursal = reader.GetString(3),
                                Parcels = reader.GetString(4),
                            };

                            guias = reader.GetString(5);

                        }
                    }


                    if (guias != "NA")
                    {
                        //Si termina en " -" quitarlos
                        if (guias.EndsWith(" - "))
                        {
                            guias = guias.Substring(0, guias.Length - 2);
                        }

                        //Si inicia en " -" quitarlos
                        if (guias.StartsWith(" -"))
                        {
                            guias = guias.Substring(2);  // Retirar los primeros dos caracteres (espacio y guion)
                        }

                        List<string> Guias = new List<string>();
                        // Dividir el string usando el guion como delimitador
                        string[] partes = guias.Split(" - ");

                        foreach (var parte in partes)
                        {
                            //var Guide = new Guide()
                            //{
                            //    NumberOfGuide = parte,
                            //};
                            Guias.Add(parte);
                        }
                        client.Guides = Guias;
                    }

                    var Items = new List<Item>();

                    StrSql = $@"
                                SELECT
	                                T1.""ItemCode"",
	                                T1.""Dscription"",
	                                T1.""CodeBars"",
	                                T1.""Quantity"",
	                                T1.""PriceBefDi"",
	                                T1.""DiscPrcnt"",
                                    T1.""TaxCode""
	
                                FROM
	                                {DB}.OINV T0  
	                                INNER JOIN {DB}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry""
	
                                WHERE
	                                T0.""DocNum"" = '{DocNum}'";

                    using (var cmd = new HanaCommand(StrSql, Con))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            var Item = new Item()
                            {
                                ItemCode = reader.GetString(0),
                                Dscription = reader.GetString(1),
                                CodeBars = reader.GetString(2),
                                Quantity = Convert.ToInt32( reader.GetString(3).Replace(".000000", "")),
                                PriceBefDi = reader.GetDecimal(4),
                                DiscPrcnt = reader.GetDecimal(5),
                                TaxCode = reader.GetString(6),
                            };

                            Items.Add(Item);

                        }
                    }
                    client.Items = Items;
                }
            }
            catch (Exception ex)
            {
                string error = "Error. Al obtener cliente. " + ex.Message.ToString();
                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");

                return new Invoice();

            }
            return client;

        }

        public async Task<string> CreatingReturnRequest(Models.returnRequest Request)
        {

            try
            {
                var Session = await ConexionSAPSL();

                //Completar datos en cabesera
                Request.DocDate = DateTime.Now;
                Request.Comments = "Solicitud de devolución generada desde Fresh.";
                Request.U_StatusInc = "Pendiente";
                Request.U_TimeInc = DateTime.Now.ToString();
                Request.Series = "77";

                //Consulta para obtener, datos faltantes
                var query = await GetQuery(Request.DocumentReferences[0].RefDocNum);

                //Completar datos faltantes en cabesera
                Request.U_Sucursal = query.U_Sucursal;
                Request.U_RFCondiciones = query.U_RFCondiciones;
                Request.NumAtCard = query.LicTradNum;
                Request.SalesPersonCode = query.SlpCode;
                Request.U_CIncidencia = query.USER_CODE;
                Request.PaymentMethod = query.PeyMethod;
                Request.PaymentGroupCode = query.GroupNum;

                //Logica para completar datos faltantes en cabesera
                if(query.U_RFCondiciones == "01" || query.U_RFCondiciones == "03")
                {
                    Request.U_B1SYS_MainUsage = "S01";
                }
                else
                {
                    Request.U_B1SYS_MainUsage = "G02";
                }

                //Completar datos faltantes en documentos referenciados
                Request.DocumentReferences[0].LineNumber = 1;
                Request.DocumentReferences[0].RefObjType = "rot_SalesInvoice";
                Request.DocumentReferences[0].RefDocEntr = query.DocEntry;

                foreach(var a in Request.DocumentLines)
                {
                    a.U_MotCancel = Convert.ToString(a.ReturnReason);
                }

                var returnRequest = await ReturnRequest(Request, Session);
                return returnRequest;

            }
            catch (Exception ex)
            {
                string error = "Error. No fue posible crear una solicitud de devolucion " + ex.Message.ToString();
                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");
            }
            return "";
        }

        public async Task<string> ConexionSAPSL()
        {
            string result = "";
            try
            {
                string DB = productive == "YES" ? DBYes : DBNo;


                string loginData = JsonConvert.SerializeObject(new
                {
                    CompanyDB = DB,
                    Password = PassWordSAP,
                    UserName = UserSAP
                });

                using (var httpClient = new HttpClient())
                {
                    var url = "http://172.16.21.249:50001/b1s/v1/Login";

                    // Configurar encabezados de solicitud
                    httpClient.DefaultRequestHeaders.Add("B1S-WCFCompatible", "true");
                    httpClient.DefaultRequestHeaders.Add("B1S-MetadataWithoutSession", "true");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));

                    var loginContent = new StringContent(loginData, Encoding.UTF8, "application/json");

                    // Realizar solicitud POST
                    var loginResponse = await httpClient.PostAsync(url, loginContent);

                    if (loginResponse.IsSuccessStatusCode)
                    {
                        // Leer el contenido de la respuesta como una cadena
                        string responseContent = await loginResponse.Content.ReadAsStringAsync();

                        // Parsear el JSON
                        JObject jsonResponse = JObject.Parse(responseContent);

                        // Leer los valores de los campos
                        string odataMetadata = jsonResponse["odata.metadata"].ToString();
                        string sessionId = jsonResponse["SessionId"].ToString();
                        string version = jsonResponse["Version"].ToString();
                        int sessionTimeout = jsonResponse["SessionTimeout"].ToObject<int>();

                        return sessionId;
                    }
                    else
                    {
                    }
                }
            }
            catch (HttpRequestException ex)
            {
            }
            catch (Exception ex)
            {
            }

            return result;
        }

        public async Task<string> ReturnRequest(Models.returnRequest returnRequest, string session)
        {
            string result = "";
            try
            {
                var jsonBody = _mapper.Map<APICRM.Models.Mapper.returnRequest>(returnRequest);
                string body = JsonConvert.SerializeObject(jsonBody);

                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true // Solo para pruebas
                };

                using (var httpClient = new HttpClient(handler))
                {
                    var url = "https://172.16.21.249:50000/b1s/v1/ReturnRequest";

                    // Configurar encabezados de solicitud
                    httpClient.DefaultRequestHeaders.Add("Cookie", "B1SESSION=" + session + "; ROUTEID=.node2");
                    httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    httpClient.DefaultRequestHeaders.Add("B1S-WCFCompatible", "true");
                    httpClient.DefaultRequestHeaders.Add("B1S-MetadataWithoutSession", "true");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                    httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                    httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                    httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));

                    var loginContent = new StringContent(body, Encoding.UTF8, "application/json");

                    // Realizar solicitud POST
                    var loginResponse = httpClient.PostAsync(url, loginContent).GetAwaiter().GetResult(); // Bloquea hasta que termine

                    if (loginResponse.IsSuccessStatusCode)
                    {

                        // Verificar si la respuesta está comprimida
                        var contentEncoding = loginResponse.Content.Headers.ContentEncoding.FirstOrDefault();
                        string responseContent = string.Empty;

                        if (contentEncoding == "gzip" || contentEncoding == "deflate")
                        {
                            // Descomprimir la respuesta
                            using (var stream = await loginResponse.Content.ReadAsStreamAsync())
                            using (var decompressedStream = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Decompress))
                            using (var reader = new System.IO.StreamReader(decompressedStream))
                            {
                                responseContent = await reader.ReadToEndAsync();
                            }
                        }
                        else
                        {
                            // Si no está comprimido, leer directamente
                            responseContent = await loginResponse.Content.ReadAsStringAsync();
                        }

                        // Ahora que tienes el contenido descomprimido, intenta parsearlo como JSON
                        try
                        {
                            JObject jsonResponse = JObject.Parse(responseContent);

                            
                            JsonContent json = JsonContent.Create(
                                new 
                                {
                                    FolioInterno = jsonResponse["DocEntry"],
                                    Folio = jsonResponse["DocNum"],
                                }
                                ); // Asigna un objeto vacío

                            result = JsonConvert.SerializeObject(json.Value);

                            return result;
                        }
                        catch (JsonException ex)
                        {
                            result = "Error. al parsear JSON: " + ex.Message;
                        }
                    }
                    else
                    {
                        result = "Error. IsSuccessStatusCode false";
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                result = "Error. En la solicitud HTTP: " + ex.Message;
            }
            catch (TimeoutException ex)
            {
                result = "Error. La solicitud ha expirado: " + ex.Message;
            }
            catch (Exception ex)
            {
                result = "Error. General: " + ex.Message;
            }

            return result;
        }

        public async Task<InvoiceQuery> GetQuery(int DocNum)
        {

            string DB = string.Empty;
            string StrSql = string.Empty;
            var Query = new InvoiceQuery();

            try
            {

                using (var Con = new HanaConnection(HanaConec))
                {
                    await Con.OpenAsync();
                    DB = productive == "YES" ? DBYes : DBNo;

                    StrSql = $@"
                            SELECT DISTINCT
	                            T0.""DocEntry"",
	                            T0.""U_Sucursal"",
	                            T0.""U_RFCondiciones"",
	                            T0.""SlpCode"",
	                            T1.""LicTradNum"",
	                            T4.""USER_CODE"",
                                T0.""GroupNum"",
                                T0.""PeyMethod""
	
                            FROM
	                            {DB}.OINV T0
	                            INNER JOIN {DB}.OCRD T1 ON T0.""CardCode""  = T1.""CardCode""
	                            INNER JOIN {DB}.INV1 T2 ON T0.""DocEntry"" = T2.""DocEntry""
	                            INNER JOIN {DB}.ORDR T3 ON T2.""BaseEntry"" = T3.""DocEntry""
	                            INNER JOIN {DB}.OUSR T4 ON T3.""UserSign"" = T4.""USERID""
	                            INNER JOIN {DB}.OSLP T5 ON T0.""SlpCode"" = T5.""SlpCode""

                            WHERE
	                            T0.""DocNum"" = '{DocNum}'
      
";

                    using (var cmd = new HanaCommand(StrSql, Con))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            Query = new InvoiceQuery()
                            {
                                DocEntry = reader.GetInt32(0),
                                U_Sucursal = reader.IsDBNull(1) ? "05" : reader.GetString(1),
                                U_RFCondiciones = reader.GetString(2),
                                SlpCode = reader.GetString(3),
                                LicTradNum = reader.GetString(4),
                                USER_CODE = reader.GetString(5),
                                GroupNum = reader.GetString(6),
                                PeyMethod = reader.GetString(7),
                            };

                        }
                    }
                    return Query;
                }
            }
            catch (Exception ex)
            {
                string error = "Error. Al obtener query. " + ex.Message.ToString();
                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");

                return new InvoiceQuery();

            }

        }

        public  async Task<string> SearchReporte(int Documento)
        {
            string msj = string.Empty;
            string DBName = string.Empty;
            DBName = (productive == "YES" ? DBYes : DBNo);

            var Getdata = await GetData(Documento, DBName);

            if (string.IsNullOrEmpty(Getdata.ReportID))
            {
                JsonContent JsonFalse =
                JsonContent.Create
                (
                        new
                        {
                            success = false,
                            answer = "No se ha encontrado la factura. Por favor, inténtalo de nuevo más tarde.",
                        }
                );
                msj = JsonConvert.SerializeObject(JsonFalse.Value);
                return msj;
            }

            var data = new Data()
            {
                DocNum = Documento,
                CardName = Getdata.CardName,
                Email = Getdata.Email,
                PdfPath = Path,
                ReportID = Getdata.ReportID,
            };

            var validacion =  await SendEmail(data);

            if (validacion)
            {
                JsonContent jsonTrue =
                JsonContent.Create
                (
                        new
                        {
                            success = true,
                            answer = "La factura ha sido enviada por correo exitosamente.",
                        }
                );

                msj = JsonConvert.SerializeObject(jsonTrue.Value);
                return msj;
            }
            else
            {
                JsonContent JsonFalse =
                JsonContent.Create
                (
                        new
                        {
                            success = false,
                            answer = "No se ha encontrado la factura. Por favor, inténtalo de nuevo más tarde.",
                        }
                );
                msj = JsonConvert.SerializeObject(JsonFalse.Value);
                return msj;
            }
        }

        public  async Task<GetData> GetData(int DocNum, string DBName)
        {
            var getData = new GetData();

            string StrSql = string.Empty;

            try
            {

                using (var Con = new HanaConnection(HanaConec))
                {
                    await Con.OpenAsync();

                    StrSql = $@"
                                SELECT
	                                T0.""CardName"",
	                                T0.""U_Sucursal"",
	                                T1.""E_Mail"",
                                    T2.""ReportID""
                                FROM 
	                                {DBName}.OINV T0
	                                INNER JOIN {DBName}.OCRD T1 ON T0.""CardCode"" = T1.""CardCode""
                                    LEFT JOIN {DBName}.ECM2 T2 ON T2.""SrcObjType"" = T0.""ObjType"" 
	 	                                   AND T2.""SrcObjAbs"" = T0.""DocEntry""
                                WHERE
	                                T0.""DocNum"" = '{DocNum}'
                                ";

                    using (var cmd = new HanaCommand(StrSql, Con))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            getData = new GetData()
                            {
                                CardName = reader.GetString(0),
                                WhsCode = reader.GetString(1),
                                Email = reader.GetString(2),
                                ReportID = reader.GetString(3),
                            };

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string error = "Error. Al obtener objeto GetData. " + ex.Message.ToString();
                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");

                return new GetData();

            }
            return getData;

        }

        public async Task<bool> SendEmail(Data data)
        {
            string path = string.Empty;

            DateTime dateTime = DateTime.Now;
            string fechaFormateada = dateTime.ToString("yyyyMMdd");

            try
            {
                MailMessage msg = new MailMessage();

                msg.Subject = $@"Factura: {data.DocNum}";
                msg.From = new MailAddress(Mail);

                // Aquí agregamos el destinatario
                msg.To.Add(new MailAddress($@"{data.Email}"));  //abraham.jimenez@mepiel.com.mx
                msg.To.Add(new MailAddress("servicioalcliente@mepiel.com.mx"));
                //{data.Email}

                #region Busqueda manual de factura

                path = $@"{data.PdfPath}{data.CardName.ToUpperInvariant()}\I\{data.ReportID}.pdf";
                data.PdfPath = path;

                #endregion


                // Adjuntar PDF
                if (!string.IsNullOrEmpty(data.PdfPath) && File.Exists(data.PdfPath))
                {
                    Attachment attachment = new Attachment(data.PdfPath);
                    msg.Attachments.Add(attachment);
                }
                else
                {
                    return false;
                }

                msg.Body = $@"
                         <h4>Estimado/a {data.CardName}</h4>
                        Le enviamos adjunta la factura correspondiente a su compra:
                        <br>
                        <br>
                                <li> <b> N&uacutemero de Factura: </b> {data.DocNum}</li>    

                        <h4>Atentamente,</h4>
                        MEPIEL DISTRIBUIDORES ESPECIALIZADOS";

                msg.IsBodyHtml = true;

                using (SmtpClient client = new SmtpClient())
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(Mail, "giIb3OTlMq;+");
                    client.Host = "svgp364.serverneubox.com.mx";
                    client.Port = 587;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Timeout = 100000;

                    await client.SendMailAsync(msg);  // Versión async recomendada
                }
                return true;
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"Error de SMTP: {smtpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                string msj = $"Error: Al enviar correo, {ex.Message}";
                return false;
            }
        }

        public async Task<List<ItemInfo>> LineInfo(string DocNum, string ItemCodes)
        {
            //List<Info> list = new List<Info>();
            var infoList = new List<ItemInfo>();
            string StrSql = string.Empty;

            //string[] codigos = ItemCodes.Trim('[', ']').Split(',');

            // Eliminar los corchetes y luego separar por coma
            string[] codigos = ItemCodes.Trim('[', ']').Split(',');

            // Limpiar los espacios extra alrededor de cada código
            var codigosLimpiados = codigos.Select(codigo => codigo.Trim()).ToArray();

            // Crear la cadena de consulta en formato adecuado para SQL IN
            string consultaIn = $"IN ('{string.Join("', '", codigosLimpiados)}')";

            var DBName = (productive == "YES" ? DBYes : DBNo);

            try
            {

                using (var Con = new HanaConnection(HanaConec))
                {
                    await Con.OpenAsync();

                    //foreach (var item in codigos)
                    //{

                        StrSql = $@"
                                SELECT 
	                                T0.""DocNum"", 
	                                T1.""ItemCode"",
	                                T1.""Quantity"",
	                                T1.""PriceBefDi"",
	                                T1.""TaxCode"",
	                                T2.""BatchNum"",
	                                T1.""DiscPrcnt"",
                                    T1.""Dscription""
                                FROM 
	                                {DBName}.OINV T0
	                                INNER JOIN {DBName}.INV1 T1 ON T0.""DocEntry"" = T1.""DocEntry"" 
	                                INNER JOIN {DBName}.IBT1  T2 ON T1.""DocEntry"" = T2.""BaseEntry"" AND T1.""ItemCode"" = T2.""ItemCode"" AND T1.""LineNum"" = T2.""BaseLinNum""
                                WHERE 
	                                T0.""DocNum"" = '{DocNum}' AND  T1.""ItemCode"" {consultaIn}
                                ORDER BY 
	                                T1.""LineNum"" ASC 
                                ";

                        using (var cmd = new HanaCommand(StrSql, Con))
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {

                            while (await reader.ReadAsync())
                            {
                                var info = new ItemInfo()
                                {
                                    DocNum = Convert.ToInt32(reader.GetString(0)),
                                    ItemCode = reader.GetString(1),
                                    Quantity = Convert.ToInt32(reader.GetString(2).Replace(".000000","")),
                                    PriceBefdi = reader.GetDecimal(3),
                                    TaxCode = reader.GetString(4),
                                    BatchNum = reader.GetString(5),
                                    DiscPrcnt = reader.GetDecimal(6),
                                    Dscription = reader.GetString(7),
                                };

                                infoList.Add(info);

                                
                            }

                            //var GetInfo = new Info()
                            //{
                            //    InfoItemCode = infoList,
                            //};

                            //infoList = new List<ItemInfo>();

                            //list.Add(GetInfo);
                        }
                    //}

                }
            }
            catch (Exception ex)
            {
                string error = "Error. Al obtener objeto Info de item. " + ex.Message.ToString();
                DateTime date = DateTime.Now;
                string fechaFormateada = date.ToString("yyyyMMdd");
                return new List<ItemInfo>();
            }
            return infoList;

        }

    }
}
