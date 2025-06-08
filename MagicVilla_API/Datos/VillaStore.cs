using MagicVilla_API.Modelos.Dto;
namespace MagicVilla_API.Datos
{
    public static class VillaStore
    {
        public static List<VillaDto> listaVillas = new List<VillaDto>
        {
            new VillaDto
            {
                Id = 1,
                Nombre = "Vista a la Piscina",
                Ocupantes = 4,
                Superficie = 50
            },
            new VillaDto
            {
                Id = 2,
                Nombre = "Vista a la Playa",
                Ocupantes = 6,
                Superficie = 75
            }
        };
    }
}
