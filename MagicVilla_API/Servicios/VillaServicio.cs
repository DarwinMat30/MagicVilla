using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MagicVilla_API.Servicios
{
    public class VillaServicio
    {
        private readonly AplicationDbContext _dbContext;
        public VillaServicio(AplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ActualizarModelo(Villa modelo)
        {
            // Marcar la entidad como modificada
            _dbContext.Entry(modelo).State = EntityState.Modified;

            // Guardar los cambios
            await _dbContext.SaveChangesAsync();
        }
    }
}
