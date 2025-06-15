using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Repositorio
{
    public class UsuarioRepositorio : Repositorio<Usuario>, IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _db;
        public UsuarioRepositorio(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public async Task<Usuario> Actualizar(Usuario entidad) // Mark return type as nullable
        {
            entidad.FechaActualizacion = DateTime.Now;
            _db.Usuario.Update(entidad); // Use Update method to track changes
            await _db.SaveChangesAsync(); // Save changes to the database
            return entidad;
        }
    }
}
