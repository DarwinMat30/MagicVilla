using MagicVilla_API.Modelos;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Datos
{
    public class AplicationDbContext : DbContext
    {
        public AplicationDbContext(DbContextOptions<AplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Villa> Villas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Villa>().HasData(
                new Villa
                {
                    Id = 1,
                    Nombre = "Villa Real",
                    Detalle = "Villa con vista al mar",
                    Tarifa = 200.00,
                    Ocupantes = 4,
                    Superficie = 150.5,
                    ImagenUrl = "https://example.com/villa1.jpg",
                    Amenidad = "Piscina, WiFi, Desayuno incluido",
                    FechaActualizacion = DateTime.Now,
                    FechaCreacion = DateTime.Now
                },
                new Villa
                {
                    Id = 2,
                    Nombre = "Villa Luna",
                    Detalle = "Villa en la montaña",
                    Tarifa = 250.00,
                    Ocupantes = 6,
                    Superficie = 200.0,
                    ImagenUrl = "https://example.com/villa2.jpg",
                    Amenidad = "Jacuzzi, Vista panorámica",
                    FechaActualizacion = DateTime.Now,
                    FechaCreacion = DateTime.Now
                }
            );
        }
       
    }
}
