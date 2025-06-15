using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Modelos.Dto
{
    public class UsuarioUpdateDto
    {
        public int IdUsuario { get; set; }

        [MaxLength(200)]
        public required string Nombre { get; set; }

        public string Email { get; set; } = string.Empty;

        public required string Clave { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
