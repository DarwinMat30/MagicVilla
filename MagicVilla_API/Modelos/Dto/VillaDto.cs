using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Modelos.Dto
{
    public class VillaDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Nombre { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;

        [Required]
        public double Tarifa { get; set; }
        public int Ocupantes { get; set; }
        public double Superficie { get; set; }
        public string ImagenUrl { get; set; } = string.Empty;
        public string Amenidad { get; set; } = string.Empty;
    }
}
