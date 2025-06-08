using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Servicios
{
    public class MiServicio
    {
        private readonly AplicationDbContext _dbContext;
        public MiServicio(AplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ActualizarModelo(Villa modelo)
        {
            // Marcar la entidad como modificada
            _dbContext.Entry(modelo).State = EntityState.Modified;

            // Guardar los cambios
            _dbContext.SaveChanges();
        }
    }
}
