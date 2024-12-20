using APICRM.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Swashbuckle.AspNetCore.Annotations;
using APICRM.Models;
using APICRM.Logic;

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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        Summary = "Obtener la lista de clientes desde SAP",
        Description = "Este servicio se encarga de recuperar todos los clientes disponibles en la base de datos de SAP. Para su funcionamiento, es necesario contar con un token de autenticación.")]
        [ResponseCache(Duration = 10)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorization]
        //[CustomHeaderRequired] // Este endpoint requiere el encabezado
        [Route("GetClients")]
        public IActionResult GetAllClients()
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

                        var MyClients = _methods.GetClients("NO");

                        if (MyClients.Result.Count < 1)
                        {

                            Conflic conflic = new Conflic()
                            {
                                code = 404,
                                Description = "Error al obtener el listado de clientes. Intente nuevamente más tarde."
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
                                answer = MyClients.Result
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

                return StatusCode(500, "Ha ocurrido un error interno. Por favor, inténtelo de nuevo más tarde.)");
            }

        }

    }
}
