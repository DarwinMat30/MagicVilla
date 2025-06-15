using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Modelos.Dto
{
    public class VillaCreateDto
    {
        [MaxLength(30)]
        public required string Nombre { get; set; }
        public string Detalle { get; set; } = string.Empty;

        public required double Tarifa { get; set; }
        public int Ocupantes { get; set; }
        public double Superficie { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
        public string Amenidad { get; set; } = string.Empty;
    }
}
