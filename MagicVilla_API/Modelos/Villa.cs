using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_API.Modelos
{
    public class Villa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;

        [Required]
        public double Tarifa { get; set; }

        public int Ocupantes { get; set; }

        public double Superficie { get; set; }

        public string ImagenUrl { get; set; } = string.Empty;
        public string Amenidad { get; set; } = string.Empty;

        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
