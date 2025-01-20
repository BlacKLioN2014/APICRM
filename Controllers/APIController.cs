using APICRM.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Swashbuckle.AspNetCore.Annotations;
using APICRM.Logic;
using System.Collections;
using Swashbuckle.AspNetCore.Filters;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace APICRM.Controllers
{

    [ApiController]
    //[Route("[controller]")]
    public class APIController : ControllerBase
    {


        private readonly Methods _methods;


        public APIController(Methods methods)
        {
            _methods = methods;
        }


        [HttpGet]
        ///[HttpGet("GetToken/{usuario},{contraseña}", Name = "GetToken")]
        [SwaggerOperation(
        Summary = "Obtener Token",
        Description = "Este servicio es responsable de generar un token de autenticación utilizando las credenciales del usuario, específicamente su nombre de usuario y contraseña.")]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("GetToken")]
        public IActionResult  GetToken([FromQuery] UserLogin Login)
        {

            try
            {

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                //Buscar Token
                string Token =  _methods.generarToKen(Login);

                //Nunca pasa, pero...
                if (Token.Contains("Error"))
                {
                    Conflic conflicto = new Conflic()
                    {
                        Description = $@"Ocurrió un error interno en el servidor. Por favor, inténtelo nuevamente más tarde. {Token}",
                        code = 500
                    };

                    Response<Conflic> Response = new Response<Conflic>()
                    {
                        success = false,
                        answer = conflicto
                    };
                    return StatusCode(500, Response);
                }

                Response<string> response = new Response<string>()
                {
                    success = true,
                    answer = Token
                };

                return Ok(response);

            }
            catch (Exception x)
            {

                Conflic conflicto = new Conflic()
                {
                    Description = $@"Ocurrió un error interno en el servidor. Por favor, inténtelo nuevamente más tarde. {x.ToString()}",
                    code = 500
                };

                Response<Conflic> response = new Response<Conflic>()
                {
                    success = false,
                    answer = conflicto
                };

                return StatusCode(500, response);

                //return StatusCode(StatusCodes.Status500InternalServerError, "Ocurrió un error interno en el servidor. Por favor, inténtelo nuevamente más tarde.");
            }

        }


        [HttpGet]
        [SwaggerOperation(
        Summary = "Obtener listado de clientes",
        Description = "Este servicio se encarga de recuperar todos los clientes disponibles en la base de datos de SAP. Para su funcionamiento, es necesario contar con un token de autenticación.")]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("GetClients")]
        public async Task <IActionResult> GetAllClients()
        {
            string Authorization = Request.Headers["Authorization"];

            try
            {

                if (!string.IsNullOrEmpty(Authorization))
                {
                    Authorization = Authorization.Replace("Bearer ", "");

                    UserLogin userLogin = _methods.desifrarToken(Authorization.Trim());

                    if (userLogin.User.Trim() == _methods.UserApi && userLogin.Password.Trim() == _methods.PasswordApi.Trim())
                    {

                        var MyClients = await _methods.GetClients();

                        if (MyClients.Count < 1)
                        {

                            Conflic conflic = new Conflic()
                            {
                                code = 404,
                                Description = "No se encontraron datos de clientes. Por favor, intente nuevamente más tarde."
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
                            Response<List<Client>> response = new Response<List<Client>>()
                            {
                                success = true,
                                answer = MyClients
                            };

                            return Ok(response);

                        }
                    }
                    #region Lista de clientes en base de pruebas
                    //else if (ValidTokenTest == Authorization)
                    //{
                    //    List<Producto> a = Data.obtenerProductos("NO");

                    //    if (a.Count < 1)
                    //    {
                    //        Response<Conflicto> response = new Response<Conflicto>();
                    //        response.Exito = false;
                    //        response.Respuesta.Descripcion = "Error al obtener listado de productos";
                    //        response.Respuesta.Codigo = 404;
                    //        return NotFound(response);
                    //    }
                    //    else
                    //    {
                    //        Response<List<Producto>> response = new Response<List<Producto>>()
                    //        {
                    //            Exito = true,
                    //            Respuesta = a
                    //        };

                    //        // Calcular el tamaño de la respuesta en bytes
                    //        //string jsonResponse = JsonSerializer.Serialize(response);
                    //        //long responseSize = System.Text.Encoding.UTF8.GetByteCount(jsonResponse);

                    //        // Opcional: Puedes registrar el tamaño o devolverlo en el encabezado
                    //        //Response.Headers.Add("X-Response-Size", responseSize.ToString());

                    //        return Ok(response);
                    //    }
                    //}
                    #endregion
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
        Summary = "Obtener datos de cliente",
        Description = "Este servicio permite recuperar algunos datos de un cliente mediante la búsqueda de un correo electrónico en la base de datos de SAP. Para su correcto funcionamiento, es imprescindible disponer de un token de autenticación válido.")]
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

                    UserLogin userLogin = _methods.desifrarToken(Authorization.Trim());

                    if (userLogin.User.Trim() == _methods.UserApi && userLogin.Password.Trim() == _methods.PasswordApi.Trim())
                    {

                        var MyClients = await _methods.GetClient(mail.Email);

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
                    #region Lista de clientes en base de pruebas
                    //else if (ValidTokenTest == Authorization)
                    //{
                    //    List<Producto> a = Data.obtenerProductos("NO");

                    //    if (a.Count < 1)
                    //    {
                    //        Response<Conflicto> response = new Response<Conflicto>();
                    //        response.Exito = false;
                    //        response.Respuesta.Descripcion = "Error al obtener listado de productos";
                    //        response.Respuesta.Codigo = 404;
                    //        return NotFound(response);
                    //    }
                    //    else
                    //    {
                    //        Response<List<Producto>> response = new Response<List<Producto>>()
                    //        {
                    //            Exito = true,
                    //            Respuesta = a
                    //        };

                    //        // Calcular el tamaño de la respuesta en bytes
                    //        //string jsonResponse = JsonSerializer.Serialize(response);
                    //        //long responseSize = System.Text.Encoding.UTF8.GetByteCount(jsonResponse);

                    //        // Opcional: Puedes registrar el tamaño o devolverlo en el encabezado
                    //        //Response.Headers.Add("X-Response-Size", responseSize.ToString());

                    //        return Ok(response);
                    //    }
                    //}
                    #endregion
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
        Summary = "Obtener datos de factura",
        Description = "Este servicio permite recuperar algunos datos de una factura mediante la búsqueda por folio en la base de datos de SAP. Para su correcto funcionamiento, es imprescindible disponer de un token de autenticación válido.")]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("GetInvoice")]
        public async Task<IActionResult> GetInfoInvoice([FromQuery] FindInvoice invoice)
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

                    UserLogin userLogin = _methods.desifrarToken(Authorization.Trim());

                    if (userLogin.User.Trim() == _methods.UserApi && userLogin.Password.Trim() == _methods.PasswordApi.Trim())
                    {

                        var MyClients = await _methods.GetInvoice(invoice.DocNum);

                        if (MyClients.CardCode == null || MyClients == null)
                        {

                            Conflic conflic = new Conflic()
                            {
                                code = 404,
                                Description = "No se encontraron los datos de la factura. Por favor, intente nuevamente más tarde."
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
                            Response<Invoice> response = new Response<Invoice>()
                            {
                                success = true,
                                answer = MyClients
                            };

                            return Ok(response);

                        }
                    }
                    #region Lista de clientes en base de pruebas
                    //else if (ValidTokenTest == Authorization)
                    //{
                    //    List<Producto> a = Data.obtenerProductos("NO");

                    //    if (a.Count < 1)
                    //    {
                    //        Response<Conflicto> response = new Response<Conflicto>();
                    //        response.Exito = false;
                    //        response.Respuesta.Descripcion = "Error al obtener listado de productos";
                    //        response.Respuesta.Codigo = 404;
                    //        return NotFound(response);
                    //    }
                    //    else
                    //    {
                    //        Response<List<Producto>> response = new Response<List<Producto>>()
                    //        {
                    //            Exito = true,
                    //            Respuesta = a
                    //        };

                    //        // Calcular el tamaño de la respuesta en bytes
                    //        //string jsonResponse = JsonSerializer.Serialize(response);
                    //        //long responseSize = System.Text.Encoding.UTF8.GetByteCount(jsonResponse);

                    //        // Opcional: Puedes registrar el tamaño o devolverlo en el encabezado
                    //        //Response.Headers.Add("X-Response-Size", responseSize.ToString());

                    //        return Ok(response);
                    //    }
                    //}
                    #endregion
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
        Summary ="Generar solicitud de devolucion en SAP",
        Description = "Este servicio facilita la generación de un documento de solicitud de devolución en SAP. Para su uso, es indispensable contar con un token de autenticación válido.")]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("CreateReturnRequest")]
        [SwaggerRequestExample(typeof(returnRequest), typeof(RequestExample))]  // Aquí agregas el ejemplo
        public async Task<IActionResult> returnRequest([FromBody] returnRequest request)
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

                    UserLogin userLogin = _methods.desifrarToken(Authorization.Trim());

                    if (userLogin.User.Trim() == _methods.UserApi && userLogin.Password.Trim() == _methods.PasswordApi.Trim())
                    {
                        //returnRequest Truerequest = JsonConvert.DeserializeObject<returnRequest>(jsonRecibido);
                        //string jsonParaEnvio = JsonConvert.SerializeObject(Truerequest);

                        var Creating = await _methods.CreatingReturnRequest(request);
                        //string CreatingString = JsonConvert.SerializeObject(Creating);

                        if (string.IsNullOrEmpty(Creating) || Creating.Contains("Error. "))
                        {

                            Conflic conflic = new Conflic()
                            {
                                code = 404,
                                Description = Creating
                            };

                            Response<Conflic> response = new Response<Conflic>()
                            {
                                success = false,
                                answer = conflic
                            };

                            return StatusCode(500, response);

                        }
                        else
                        {

                            dynamic jsonObj = JsonConvert.DeserializeObject<dynamic>(Creating);

                            Response<dynamic> response = new Response<dynamic>()
                            {
                                success = true,
                                answer = jsonObj
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
