using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Repositorio;
using MagicVilla_API.Repositorio.IRepositorio;
using MagicVilla_API.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading.Tasks;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NumeroVillaController : ControllerBase
    {
        private readonly ILogger<NumeroVillaController> _logger;
        private readonly IVillaRepositorio _villaRepo;
        private readonly INumeroVillaRepositorio _numeroVillaRepo; // Assuming you have a repository for NumeroVilla
        private readonly IMapper _mapper; // Assuming you have AutoMapper configured
        protected APIResponse _response; // Assuming you have a custom APIResponse class for consistent responses
        public NumeroVillaController(ILogger<NumeroVillaController> logger, IVillaRepositorio villaRepo
            , INumeroVillaRepositorio villaNumeroRepo, IMapper mapper)
        {
            _logger = logger;
            _villaRepo = villaRepo;
            _numeroVillaRepo = villaNumeroRepo; // Initialize the NumeroVilla repository
            _mapper = mapper; // Initialize the AutoMapper instance
            _response = new APIResponse(); // Initialize the APIResponse instance
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetNumeroVillas()
        {
            try
            {
                _logger.LogInformation("Obteniendo lista de villas");
                IEnumerable<NumeroVilla> listaNumeroVillas = await _numeroVillaRepo.ObtenerTodos();
                _response.Resultado = _mapper.Map<IEnumerable<NumeroVillaDto>>(listaNumeroVillas); // Map the list of Villa to VillaDto using AutoMapper
                _response.statusCode = HttpStatusCode.OK; // Set the status code to OK
                return Ok(_response); // Assuming _dbContext.Villas is a DbSet<VillaDto> or similar
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                _response.Errores = new List<string> { ex.Message }; // Add the error message to the response
            }
            return _response; // Return the response with the error details
        }

        [HttpGet("{id:int}", Name = "GetNumeroVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetNumeroVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Id Número de Villa no puede ser 0");
                    _response.statusCode = HttpStatusCode.BadRequest; // Set the status code to BadRequest
                    _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                    return BadRequest(_response);
                }
                var numeroVilla = await _numeroVillaRepo.Obtener(v => v.VillaNumero == id); // Assuming _dbContext.Villas is a DbSet<VillaDto> or similar
                if (numeroVilla == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound; // Set the status code to NotFound
                    _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                    return NotFound(_response);
                }

                _response.Resultado = _mapper.Map<NumeroVillaDto>(numeroVilla); // Map the Villa to VillaDto using AutoMapper
                _response.statusCode = HttpStatusCode.OK; // Set the status code to OK
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                _response.Errores = new List<string> { ex.ToString() }; // Add a generic error message to the response
            }
            return _response; // Return the response with the error details
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CrearNumeroVilla([FromBody] NumeroVillaCreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (await _numeroVillaRepo.Obtener(v => v.VillaNumero == createDto.VillaNumero) != null)
                {
                    ModelState.AddModelError("Errores Personalizados", "Número de Villa ya existe!"); // Custom error message
                    return BadRequest(ModelState); 
                }

                //Si no existe el id de la villa, retornar BadRequest
                if (await _villaRepo.Obtener(n => n.Id == createDto.VillaId) == null)
                { 
                    ModelState.AddModelError("Error de Clave Foránea", "Id de Villa no existe!"); // Custom error message
                    return BadRequest(ModelState); // Return BadRequest with the model state errors
                }

                if (createDto == null) return BadRequest(createDto);
                NumeroVilla modelo = _mapper.Map<NumeroVilla>(createDto); // Map the DTO to the Villa model using AutoMapper
                modelo.FechaCreacion = DateTime.Now; // Set the creation date
                await _numeroVillaRepo.Crear(modelo); // Add the new villa to the DbSet
                _response.Resultado = modelo; // Set the created villa as the result in the response
                _response.statusCode = HttpStatusCode.Created; // Set the status code to Created
                return CreatedAtRoute("GetNumeroVilla", new { id = modelo.VillaNumero }, _response); 
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                _response.Errores = new List<string> { ex.ToString() }; // Add the error message to the response
            }
            return _response; // Return the response with the error details
        }

        [HttpDelete("{id:int}", Name = "EliminarNumeroVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EliminarNumeroVilla(int id)
        {
            try 
            {
                if (id == 0)
                {
                    _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                    _response.statusCode = HttpStatusCode.BadRequest; // Set the status code to BadRequest
                    return BadRequest(_response);
                }
                var numeroVilla = await _numeroVillaRepo.Obtener(v => v.VillaNumero == id); 
                if (numeroVilla == null)
                {
                    _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                    _response.statusCode = HttpStatusCode.NotFound; // Set the status code to NotFound
                    return NotFound(_response);
                }
                await _numeroVillaRepo.Remover(numeroVilla); // Remove the villa from the DbSet 
                _response.statusCode = HttpStatusCode.NoContent; // Set the status code to NoContent
                return Ok(_response); // Return the response with the status code
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                _response.Errores = new List<string> { ex.ToString() }; // Add the error message to the response
            }
            return BadRequest(_response); // Return the response with the error details
        }

        [HttpPut("{id:int}", Name = "ActualizarNumeroVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActualizarNumeroVilla(int id, [FromBody] NumeroVillaUpdateDto updateDto)
        {
            try
            {
                if (updateDto == null || id != updateDto.VillaNumero)
                {
                    _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                    _response.statusCode = HttpStatusCode.BadRequest; // Set the status code to BadRequest
                    return BadRequest(_response);
                }
                if(await _villaRepo.Obtener(n => n.Id == updateDto.VillaId) == null)
                {
                    ModelState.AddModelError("Error de Clave Foránea", "Id de Villa no existe!"); // Custom error message
                    return BadRequest(ModelState); // Return BadRequest with the model state errors
                }
                NumeroVilla modelo = _mapper.Map<NumeroVilla>(updateDto); // Map the DTO to the NumeroVilla model using AutoMapper
                modelo.FechaActualizacion = DateTime.Now; // Set the update date
                await _numeroVillaRepo.Actualizar(modelo); // Update the numeroVilla in the DbSet
                _response.statusCode = HttpStatusCode.NoContent; // Set the status code to NoContent
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                _response.statusCode = HttpStatusCode.BadRequest; // Set the status code to BadRequest
                _response.Errores = new List<string> { ex.ToString() }; // Add the error message to the response
            }

            return Ok(_response); 
        }

    }
}
