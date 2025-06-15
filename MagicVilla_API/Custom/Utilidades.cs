using MagicVilla_API.Modelos.Dto;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace MagicVilla_API.Custom
{
    public class Utilidades
    {
        private readonly IConfiguration _configuration;
        public Utilidades(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string encriptarSHA256(string texto)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(texto));
                
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public string desencriptarSHA256(string textoEncriptado)
        {
            // SHA-256 es un algoritmo de hash unidireccional, no se puede desencriptar.
            // En su lugar, se compara el hash de la entrada con el hash almacenado.
            throw new NotSupportedException("SHA-256 is a one-way hashing algorithm and cannot be decrypted.");
        }

        public string GenerarJwToken(UsuarioDto usuarioDto)
        {
            //Crear la información del Usuario para el token
            var UserClaims = new[]
            {
                new Claim(ClaimTypes.Name, usuarioDto.Nombre),
                new Claim(ClaimTypes.Email, usuarioDto.Email ?? string.Empty),
                new Claim("FechaActualizacion", usuarioDto.FechaActualizacion.ToString()),
                new Claim("FechaCreacion", usuarioDto.FechaCreacion.ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //Crear el detalle del token
            var jwtConfig = new JwtSecurityToken(
                claims: UserClaims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }

    }
}
