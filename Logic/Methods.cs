using APICRM.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Sap.Data.Hana;

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

        IConfiguration _config;

        public Methods(IConfiguration config)
        {

            _config = config;

            Secreta = _config.GetValue<string>("ApiSettings:Secreta");
            UserApi = _config.GetValue<string>("ApiSettings:UserApi");
            PasswordApi = _config.GetValue<string>("ApiSettings:PasswordApi");
            HanaConec = _config.GetValue<string>("ApiSettings:HanaConec");
            DBYes = _config.GetValue<string>("ApiSettings:DBYes");
            DBNo = _config.GetValue<string>("ApiSettings:DBNo");

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

        public async Task<List<Client>>  GetClients(string productive)
        {
            List<Client> Lista = new List<Client>();

            string Universal = string.Empty;

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
                                    T0.""SlpCode"",

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
                                SlpCode = reader.IsDBNull(5) ? "No data" : reader.GetString(5),
                            };

                            var CreditInformation = new CreditInformation()
                            {
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

    }
}
