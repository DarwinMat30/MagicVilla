using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        private readonly ILogger<VillaController> _logger;
        private readonly AplicationDbContext _dbContext;
        private readonly IMapper _mapper; // Assuming you have AutoMapper configured
        public VillaController(ILogger<VillaController> logger, AplicationDbContext dbContext, IMapper mapper)
        {
            _logger = logger;
            _dbContext = dbContext;
            _mapper = mapper; // Initialize the AutoMapper instance
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {
            _logger.LogInformation("Obteniendo lista de villas");
            IEnumerable<Villa> listaVillas = await _dbContext.Villas.ToListAsync(); 
            return Ok(_mapper.Map<IEnumerable<VillaDto>>(listaVillas)); // Assuming _dbContext.Villas is a DbSet<VillaDto> or similar
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDto>> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Id de villa no puede ser 0");
                return BadRequest();
            }
            var villa = await _dbContext.Villas.FirstOrDefaultAsync(v => v.Id == id); // Assuming _dbContext.Villas is a DbSet<VillaDto> or similar
            if (villa == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<VillaDto>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> CrearVilla([FromBody] VillaCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await _dbContext.Villas.FirstOrDefaultAsync(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("Errores Personalizados", "Villa con ese nombre ya existe!"); // Custom error message
                return BadRequest(ModelState);
            }
            if (createDto == null)
            {
                return BadRequest(createDto);
            }

            Villa modelo = _mapper.Map<Villa>(createDto); // Map the DTO to the Villa model using AutoMapper

            /*
            Con el mapper se puede hacer directamente sin necesidad de crear un nuevo objeto Villa
            Villa modelo = new()
            {
                Nombre = villaDto.Nombre,
                Ocupantes = villaDto.Ocupantes,
                Superficie = villaDto.Superficie,
                ImagenUrl = villaDto.ImagenUrl,
                Amenidad = villaDto.Amenidad,
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now
            };
            */

            await _dbContext.Villas.AddAsync(modelo); // Add the new villa to the DbSet
            await _dbContext.SaveChangesAsync(); // Save changes to the database

            // Return the created villa with a 201 Created status code
            return CreatedAtRoute("GetVilla", new { id = modelo.Id }, modelo); //Llamada al método GetVilla para obtener la URL de la nueva villa creada
        }

        [HttpDelete("{id:int}", Name = "EliminarVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EliminarVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var villa = await _dbContext.Villas.FirstOrDefaultAsync(v => v.Id == id); // Assuming _dbContext.Villas is a DbSet<VillaDto> or similar
            if (villa == null)
            {
                return NotFound();
            }
            _dbContext.Villas.Remove(villa); // Remove the villa from the DbSet 
            await _dbContext.SaveChangesAsync(); // Save changes to the database   
            return NoContent(); // Return 204 No Content status code
        }

        [HttpPut("{id:int}", Name = "ActualizarVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActualizarVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
           
            if (updateDto == null || id != updateDto.Id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            VillaServicio miServicio = new VillaServicio(_dbContext);
            // Obtener un modelo existente
            
            // Fix for CS8600: Ensure null safety by using null conditional operator and null coalescing operator
            Villa? villaActual = await _dbContext.Villas.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (villaActual == null)
            {
                // Handle the case where villaActual is null, e.g., return an error response or log the issue
                return BadRequest("La villa no existe.");
            }

            villaActual = _mapper.Map<Villa>(updateDto); // Map the DTO to the Villa model using AutoMapper

            /*
            Esto ya no es necesario si se usa AutoMapper, pero se deja como referencia
            villaActual.Nombre = "Nuevo Nombre";
            villaActual.FechaActualizacion = DateTime.Now;
            villaActual.Nombre = villaDto.Nombre;
            villaActual.Ocupantes = villaDto.Ocupantes;
            villaActual.Superficie = villaDto.Superficie;
            villaActual.ImagenUrl = villaDto.ImagenUrl;
            villaActual.Amenidad = villaDto.Amenidad;
            villaActual.Detalle = villaDto.Detalle;
            villaActual.Tarifa = villaDto.Tarifa;
            villaActual.FechaActualizacion = DateTime.Now;
            */

            // Actualizar el modelo
            await miServicio.ActualizarModelo(villaActual);

            // Update other properties as needed
            return NoContent(); // Return 204 No Content status code
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ActualizarVillaParcialmente(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var villa = await _dbContext.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id); // Assuming _dbContext.Villas is a DbSet<VillaDto> or similar
            if (villa == null)
            {
                return BadRequest("Error al actualizar la villa, no existe.");
            }

            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa); // Map the existing villa to a VillaDto object using AutoMapper

            /*
            Ya no es necesario crear un nuevo objeto VillaDto, se puede usar el mapper directamente
            VillaUpdateDto villaDto = new ()
            { 
                Id = villa.Id
                , Nombre = villa.Nombre 
                , Detalle = villa.Detalle
                , Amenidad = villa.Amenidad
                , ImagenUrl = villa.ImagenUrl
                , Tarifa = villa.Tarifa
                , Ocupantes = villa.Ocupantes   
                , Superficie = (int)villa.Superficie
            }; // Create a new VillaDto object to hold the current values    
            */
            
            patchDto.ApplyTo(villaDto, ModelState); // Apply the patch to the villa object
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa modelo = _mapper.Map<Villa>(villaDto); // Map the updated DTO back to the Villa model using AutoMapper
            
            /*
            Ya no es necesario crear un nuevo objeto Villa, se puede usar el mapper directamente
            Villa modelo = new()
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Ocupantes = villaDto.Ocupantes,
                Superficie = villaDto.Superficie,
                ImagenUrl = villaDto.ImagenUrl,
                Amenidad = villaDto.Amenidad,
                Detalle = villaDto.Detalle,
                Tarifa = villaDto.Tarifa,
                FechaActualizacion = DateTime.Now
            };
            */

            _dbContext.Villas.Update(modelo); // Update the villa in the DbSet
            await _dbContext.SaveChangesAsync(); // Save changes to the database

            // Update other properties as needed
            return NoContent(); // Return 204 No Content status code
        }
    }
}
