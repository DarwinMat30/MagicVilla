using AutoMapper;
using MagicVilla_API.Custom;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_API.Controllers
{
    [Route ("api/[controller]")]
    [AllowAnonymous] //Vamos a permitir el acceso a todos los usuarios, incluso no autenticados
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly ILogger<UsuarioController> _logger;
        private readonly IUsuarioRepositorio _usuarioRepo;
        private readonly IMapper _mapper; // Assuming you have AutoMapper configured
        protected APIResponse _response; // Assuming you have a custom APIResponse class for consistent responses
        private readonly Utilidades _utilidades;
        public UsuarioController(Utilidades utilidades, ILogger<UsuarioController> logger, IUsuarioRepositorio usuarioRepo, IMapper mapper)
        {
            _utilidades = utilidades;
            _logger = logger;
            _usuarioRepo = usuarioRepo;
            _mapper = mapper;
            _response = new APIResponse();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetUsuarios()
        {
            try
            {
                _logger.LogInformation("Obteniendo lista de Usuarios");
                IEnumerable<Usuario> listaUsuarios = await _usuarioRepo.ObtenerTodos();
                _response.Resultado = _mapper.Map<List<Usuario>>(listaUsuarios);
                _response.statusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                _response.Errores = new List<string> { ex.Message }; // Add the error message to the response
            }
            return _response;
        }

        [HttpGet("{id:int}", Name = "GetUsuario")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetUsuario(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Error al obtener Usuario con Id: " + id);
                    _response.statusCode = HttpStatusCode.BadRequest; // Set the status code to BadRequest
                    _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                    return BadRequest(_response);
                }
                var usuario = await _usuarioRepo.Obtener(u => u.IdUsuario == id);
                if (usuario == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound; // Set the status code to NotFound
                    _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                    return NotFound(_response);
                }
                _response.Resultado = _mapper.Map<Usuario>(usuario);
                _response.statusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                _response.Errores = new List<string> { ex.Message }; // Add the error message to the response
            }
            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CrearUsuario([FromBody] UsuarioCreateDto usuarioCreateDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (await _usuarioRepo.Obtener(v => v.Nombre.ToLower() == usuarioCreateDto.Nombre.ToLower()) != null)
                {
                    ModelState.AddModelError("Errores en Usuario", "Usuario con ese nombre ya existe!"); // Custom error message
                    return BadRequest(ModelState);
                }
                if (usuarioCreateDto == null)
                {
                    _logger.LogError("Error al crear Usuario: objeto nulo");
                    _response.statusCode = HttpStatusCode.BadRequest; // Set the status code to BadRequest
                    _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                    return BadRequest(_response);
                }
                usuarioCreateDto.Clave = _utilidades.encriptarSHA256(usuarioCreateDto.Clave); // Encrypt the password
                Usuario usuario = _mapper.Map<Usuario>(usuarioCreateDto);
                usuario.FechaCreacion = DateTime.Now;
                await _usuarioRepo.Crear(usuario);
                _response.Resultado = usuario;
                _response.statusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetUsuario", new { id = usuario.IdUsuario }, _response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                _response.Errores = new List<string> { ex.Message }; // Add the error message to the response
            }
            return _response;
        }

        [HttpPut("{id:int}", Name = "ActualizarUsuario")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ActualizarUsuario(int id, [FromBody] UsuarioUpdateDto usuarioUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (usuarioUpdateDto == null || id != usuarioUpdateDto.IdUsuario || id == 0)
                {
                    _logger.LogError("Error al actualizar Usuario: objeto nulo o Id no coincide");
                    _response.statusCode = HttpStatusCode.BadRequest; // Set the status code to BadRequest
                    _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                    return BadRequest(_response);
                }
                usuarioUpdateDto.Clave = _utilidades.encriptarSHA256(usuarioUpdateDto.Clave); // Encrypt the password
                Usuario usuario = _mapper.Map<Usuario>(usuarioUpdateDto);
                var resultado = await _usuarioRepo.Actualizar(usuario);
                if (resultado == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound; // Set the status code to NotFound
                    _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                    return NotFound(_response);
                }
                _response.Resultado = resultado;
                _response.statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                _response.Errores = new List<string> { ex.Message }; // Add the error message to the response
            }
            return Ok(_response);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<APIResponse>> Login(LoginDto usuarioLoginDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var usuario = await _usuarioRepo.Obtener(u => 
                        u.Email.ToLower() == usuarioLoginDto.Correo.ToLower() &&
                        u.Clave == _utilidades.encriptarSHA256(usuarioLoginDto.Clave)
                    );

                if(usuario == null)
                {
                    _response.statusCode = HttpStatusCode.Unauthorized; // Set the status code to Unauthorized
                    _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                    _response.Token = ""; // Set token to null in case of an error
                    return Unauthorized(_response);
                }

                _response.Resultado = new UsuarioDto
                {
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Clave = "", // Do not return the password
                    FechaActualizacion = usuario.FechaActualizacion,
                    FechaCreacion = usuario.FechaCreacion,
                    Token = _utilidades.GenerarJwToken(_mapper.Map<UsuarioDto>(usuario))
                };
                _response.statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                _response.Errores = new List<string> { ex.Message }; // Add the error message to the response
            }
            return Ok(_response);
        }
    }
}
