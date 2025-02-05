using APICRM.Logic;
using APICRM.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace APICRM.Controllers
{

    public class ChatBotController : ControllerBase
    {

        private readonly Methods methods;

        public ChatBotController( Methods _methods)
        {
            methods = _methods;
        }

        [HttpGet]
        [SwaggerOperation(
        Summary = "Obtener datos de cliente",
        Description = "Este servicio permite recuperar datos de un cliente mediante la búsqueda de su correo electrónico en la base de datos de SAP. Para su correcto funcionamiento, es imprescindible contar con un token de autenticación válido.")]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("GetClient")]
        public async Task<IActionResult> GetInfoClient([FromQuery] FindEmail mail)
        {
            string Authorization = Request.Headers["Authorization"];

            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!string.IsNullOrEmpty(Authorization))
                {
                    Authorization = Authorization.Replace("Bearer ", "");

                    UserLogin userLogin = methods.desifrarToken(Authorization.Trim());

                    if (userLogin.User.Trim() == methods.UserApi && userLogin.Password.Trim() == methods.PasswordApi.Trim())
                    {

                        var MyClients = await methods.GetClient(mail.Email);

                        if (MyClients.Count < 1)
                        {

                            Conflic conflic = new Conflic()
                            {
                                code = 404,
                                Description = "No se encontraron los datos del cliente. Por favor, intente nuevamente más tarde."
                            };

                            Response<Conflic> response = new Response<Conflic>()
                            {
                                success = false,
                                answer = conflic
                            };

                            return NotFound(response);

                        }
                        else
                        {
                            Response<List<ClientTwo>> response = new Response<List<ClientTwo>>()
                            {
                                success = true,
                                answer = MyClients
                            };

                            return Ok(response);

                        }
                    }
                    else
                    {
                        Conflic conflic = new Conflic()
                        {
                            code = 400,
                            Description = "Token no válido. Por favor, revíselo e inténtelo de nuevo."
                        };

                        Response<Conflic> response = new Response<Conflic>()
                        {
                            success = false,
                            answer = conflic
                        };

                        return BadRequest(response);
                    }
                }
                else
                {
                    Conflic conflic = new Conflic()
                    {
                        code = 400,
                        Description = "Es necesario incluir un token válido para continuar."
                    };

                    Response<Conflic> response = new Response<Conflic>()
                    {
                        success = false,
                        answer = conflic
                    };

                    return BadRequest(response);
                }
            }
            catch (Exception x)
            {
                Conflic conflic = new Conflic()
                {
                    code = 500,
                    Description = "Ha ocurrido un error interno. Por favor, inténtelo de nuevo más tarde."
                };

                Response<Conflic> response = new Response<Conflic>()
                {
                    success = false,
                    answer = conflic
                };

                return StatusCode(500, response);
            }

        }

        [HttpGet]
        [SwaggerOperation(
        Summary = "Obtener las últimas 5 facturas de un cliente.",
        Description = "Este servicio permite recuperar las últimas 5 facturas de un cliente mediante una búsqueda en la base de datos de SAP. Para su funcionamiento adecuado, es necesario contar con un token de autenticación válido.")]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("GetInvoices")]
        public async Task<IActionResult> GetLastInvoices([FromQuery] FindCardCode Code)
        {
            string Authorization = Request.Headers["Authorization"];

            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!string.IsNullOrEmpty(Authorization))
                {
                    Authorization = Authorization.Replace("Bearer ", "");

                    UserLogin userLogin = methods.desifrarToken(Authorization.Trim());

                    if (userLogin.User.Trim() == methods.UserApi && userLogin.Password.Trim() == methods.PasswordApi.Trim())
                    {

                        var Invoices = await methods.Invoices(Code.CardCode);

                        if (Invoices.Count < 1)
                        {

                            Conflic conflic = new Conflic()
                            {
                                code = 404,
                                Description = "No se han encontrado facturas"
                            };

                            Response<Conflic> response = new Response<Conflic>()
                            {
                                success = false,
                                answer = conflic
                            };

                            return NotFound(response);

                        }
                        else
                        {
                            Response<List<LastInvoices>> response = new Response<List<LastInvoices>>()
                            {
                                success = true,
                                answer = Invoices
                            };

                            return Ok(response);

                        }
                    }
                    else
                    {
                        Conflic conflic = new Conflic()
                        {
                            code = 400,
                            Description = "Token no válido. Por favor, revíselo e inténtelo de nuevo."
                        };

                        Response<Conflic> response = new Response<Conflic>()
                        {
                            success = false,
                            answer = conflic
                        };

                        return BadRequest(response);
                    }
                }
                else
                {
                    Conflic conflic = new Conflic()
                    {
                        code = 400,
                        Description = "Es necesario incluir un token válido para continuar."
                    };

                    Response<Conflic> response = new Response<Conflic>()
                    {
                        success = false,
                        answer = conflic
                    };

                    return BadRequest(response);
                }
            }
            catch (Exception x)
            {
                Conflic conflic = new Conflic()
                {
                    code = 500,
                    Description = "Ha ocurrido un error interno. Por favor, inténtelo de nuevo más tarde."
                };

                Response<Conflic> response = new Response<Conflic>()
                {
                    success = false,
                    answer = conflic
                };

                return StatusCode(500, response);
            }

        }

        [HttpPost]
        [SwaggerOperation(
        Summary = "Enviar factura por correo",
        Description = "Este servicio permite enviar el documento PDF de una factura por correo electrónico. Para su funcionamiento adecuado, es necesario contar con un token de autenticación válido.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("SendPdf")]
        public async Task<IActionResult> GetAndSendPdf([FromBody] FindInvoice Factura)
        {
            string Authorization = Request.Headers["Authorization"];

            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!string.IsNullOrEmpty(Authorization))
                {
                    Authorization = Authorization.Replace("Bearer ", "");

                    UserLogin userLogin = methods.desifrarToken(Authorization.Trim());

                    if (userLogin.User.Trim() == methods.UserApi && userLogin.Password.Trim() == methods.PasswordApi.Trim())
                    {

                        var Send = await methods.SearchReporte(Factura.DocNum);

                        if(Send.Contains("false"))
                        {
                            dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(Send);

                            Conflic conflic = new Conflic()
                            {
                                code = 404,
                                Description = jsonObj.answer
                            };

                            Response<Conflic> response = new Response<Conflic>()
                            {
                                success = false,
                                answer = conflic
                            };

                            return NotFound(response);

                        }
                        else
                        {
                            dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(Send);

                            Conflic conflic = new Conflic()
                            {
                                code = 200,
                                Description = jsonObj.answer
                            };

                            Response<Conflic> response = new Response<Conflic>()
                            {
                                success = true,
                                answer = conflic
                            };

                            return Ok(response);

                        }
                    }
                    else
                    {
                        Conflic conflic = new Conflic()
                        {
                            code = 400,
                            Description = "Token no válido. Por favor, revíselo e inténtelo de nuevo."
                        };

                        Response<Conflic> response = new Response<Conflic>()
                        {
                            success = false,
                            answer = conflic
                        };

                        return BadRequest(response);
                    }
                }
                else
                {
                    Conflic conflic = new Conflic()
                    {
                        code = 400,
                        Description = "Es necesario incluir un token válido para continuar."
                    };

                    Response<Conflic> response = new Response<Conflic>()
                    {
                        success = false,
                        answer = conflic
                    };

                    return BadRequest(response);
                }
            }
            catch (Exception x)
            {
                Conflic conflic = new Conflic()
                {
                    code = 500,
                    Description = "Ha ocurrido un error interno. Por favor, inténtelo de nuevo más tarde."
                };

                Response<Conflic> response = new Response<Conflic>()
                {
                    success = false,
                    answer = conflic
                };

                return StatusCode(500, response);
            }

        }

        [HttpPost]
        [SwaggerOperation(
        Summary = "Obtener información detallada de una partida",
        Description = "Este servicio permite acceder a la información de una partida de factura a través de una búsqueda en la base de datos de SAP. Para su correcto funcionamiento, es imprescindible disponer de un token de autenticación válido.")]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("LineInfo")]
        public async Task<IActionResult> GetLineInfo([FromBody] InfoLine info)
        {
            string Authorization = Request.Headers["Authorization"];

            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!string.IsNullOrEmpty(Authorization))
                {
                    Authorization = Authorization.Replace("Bearer ", "");

                    UserLogin userLogin = methods.desifrarToken(Authorization.Trim());

                    if (userLogin.User.Trim() == methods.UserApi && userLogin.Password.Trim() == methods.PasswordApi.Trim())
                    {

                        var foline = await methods.LineInfo(info.DocNum, info.ItemCodes);

                        if (foline.Count < 1)
                        {

                            Conflic conflic = new Conflic()
                            {
                                code = 404,
                                Description = @$"La búsqueda en el documento {info.DocNum} no arrojó resultados para la lista de ítems {info.ItemCodes}."
                            };

                            Response<Conflic> response = new Response<Conflic>()
                            {
                                success = false,
                                answer = conflic
                            };

                            return NotFound(response);

                        }
                        else
                        {
                            Response<List<ItemInfo>> response = new Response<List<ItemInfo>>()
                            {
                                success = true,
                                answer = foline
                            };

                            return Ok(response);

                        }
                    }
                    else
                    {
                        Conflic conflic = new Conflic()
                        {
                            code = 400,
                            Description = "Token no válido. Por favor, revíselo e inténtelo de nuevo."
                        };

                        Response<Conflic> response = new Response<Conflic>()
                        {
                            success = false,
                            answer = conflic
                        };

                        return BadRequest(response);
                    }
                }
                else
                {
                    Conflic conflic = new Conflic()
                    {
                        code = 400,
                        Description = "Es necesario incluir un token válido para continuar."
                    };

                    Response<Conflic> response = new Response<Conflic>()
                    {
                        success = false,
                        answer = conflic
                    };

                    return BadRequest(response);
                }
            }
            catch (Exception x)
            {
                Conflic conflic = new Conflic()
                {
                    code = 500,
                    Description = "Ha ocurrido un error interno. Por favor, inténtelo de nuevo más tarde."
                };

                Response<Conflic> response = new Response<Conflic>()
                {
                    success = false,
                    answer = conflic
                };

                return StatusCode(500, response);
            }

        }

         
    }
}
