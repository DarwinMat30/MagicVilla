using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Repositorio;
using MagicVilla_API.Repositorio.IRepositorio;
using MagicVilla_API.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading.Tasks;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        private readonly IVillaRepositorio _villaRepo;
        private readonly IMapper _mapper; // Assuming you have AutoMapper configured
        protected APIResponse _response; // Assuming you have a custom APIResponse class for consistent responses
        public VillaController(ILogger<VillaController> logger, IVillaRepositorio villaRepo, IMapper mapper)
        {
            _logger = logger;
            _villaRepo = villaRepo;
            _mapper = mapper; // Initialize the AutoMapper instance
            _response = new APIResponse(); // Initialize the APIResponse instance
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                _logger.LogInformation("Obteniendo lista de villas");
                IEnumerable<Villa> listaVillas = await _villaRepo.ObtenerTodos();
                _response.Resultado = _mapper.Map<IEnumerable<VillaDto>>(listaVillas); // Map the list of Villa to VillaDto using AutoMapper
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

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.LogError("Id de villa no puede ser 0");
                    _response.statusCode = HttpStatusCode.BadRequest; // Set the status code to BadRequest
                    _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                    return BadRequest(_response);
                }
                var villa = await _villaRepo.Obtener(v => v.Id == id); // Assuming _dbContext.Villas is a DbSet<VillaDto> or similar
                if (villa == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound; // Set the status code to NotFound
                    _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                    return NotFound(_response);
                }

                _response.Resultado = _mapper.Map<VillaDto>(villa); // Map the Villa to VillaDto using AutoMapper
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
        public async Task<ActionResult<APIResponse>> CrearVilla([FromBody] VillaCreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (await _villaRepo.Obtener(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
                {
                    ModelState.AddModelError("Errores Personalizados", "Villa con ese nombre ya existe!"); // Custom error message
                    return BadRequest(ModelState);
                }
                if (createDto == null) return BadRequest(createDto);
                Villa modelo = _mapper.Map<Villa>(createDto); // Map the DTO to the Villa model using AutoMapper
                modelo.FechaCreacion = DateTime.Now; // Set the creation date
                await _villaRepo.Crear(modelo); // Add the new villa to the DbSet
                _response.Resultado = modelo; // Set the created villa as the result in the response
                _response.statusCode = HttpStatusCode.Created; // Set the status code to Created
                return CreatedAtRoute("GetVilla", new { id = modelo.Id }, _response); 
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                _response.Errores = new List<string> { ex.ToString() }; // Add the error message to the response
            }
            return _response; // Return the response with the error details
        }

        [HttpDelete("{id:int}", Name = "EliminarVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EliminarVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                    _response.statusCode = HttpStatusCode.BadRequest; // Set the status code to BadRequest
                    return BadRequest(_response);
                }
                var villa = await _villaRepo.Obtener(v => v.Id == id); // Assuming _dbContext.Villas is a DbSet<VillaDto> or similar
                if (villa == null)
                {
                    _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                    _response.statusCode = HttpStatusCode.NotFound; // Set the status code to NotFound
                    return NotFound(_response);
                }
                await _villaRepo.Remover(villa); // Remove the villa from the DbSet 
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

        [HttpPut("{id:int}", Name = "ActualizarVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActualizarVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
            try
            {
                if (updateDto == null || id != updateDto.Id)
                {
                    _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                    _response.statusCode = HttpStatusCode.BadRequest; // Set the status code to BadRequest
                    return BadRequest(_response);
                }

                Villa modelo = _mapper.Map<Villa>(updateDto); // Map the DTO to the Villa model using AutoMapper
                modelo.FechaActualizacion = DateTime.Now; // Set the update date
                await _villaRepo.Actualizar(modelo); // Update the villa in the DbSet
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

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActualizarVillaParcialmente(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            try
            {
                if (patchDto == null || id == 0) return BadRequest();
                var villa = await _villaRepo.Obtener(v => v.Id == id, tracked: false); // Assuming _dbContext.Villas is a DbSet<VillaDto> or similar
                if (villa == null) return BadRequest();
                VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa); // Map the existing villa to a VillaDto object using AutoMapper
                patchDto.ApplyTo(villaDto, ModelState); // Apply the patch to the villa object
                if (!ModelState.IsValid) return BadRequest(ModelState);
                Villa modelo = _mapper.Map<Villa>(villaDto); // Map the updated DTO back to the Villa model using AutoMapper
                modelo.FechaActualizacion = DateTime.Now; // Set the update date    
                await _villaRepo.Actualizar(modelo); // Update the villa in the DbSet
                _response.statusCode = HttpStatusCode.NoContent; // Set the status code to NoContent
                return Ok(_response); // Return 204 No Content status code
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false; // Set IsExitoso to false in case of an error
                _response.statusCode = HttpStatusCode.BadRequest; // Set the status code to BadRequest
                _response.Errores = new List<string> { ex.ToString() }; // Add the error message to th
            }
            return Ok(_response); // Return the response with the error details
        }
    }
}
