using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Soltec.axControl.Server.Middlewares
{
    public class JwtCustomMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtCustomMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 1. Extraer el encabezado 'Authorization'
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader != null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                // Extraemos el string puro del token (sacando la palabra 'Bearer ')
                var token = authHeader.Substring("Bearer ".Length).Trim();

                try
                {
                    // 2. Adjuntar el usuario al contexto si el token es válido
                    AttachUserToContext(context, token);
                }
                catch (Exception ex)
                {
                    // Si el token falló por expiración, firma inválida, etc., podés auditar el error acá:
                    Console.WriteLine($"[JWT Middleware Error]: {ex.Message}");
                    
                    // Opcional: Podés retornar un 401 personalizado acá mismo si querés romper el flujo,
                    // pero es mejor dejar que siga y que el atributo [Authorize] del controlador se encargue.
                }
            }

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            
            // Obtenemos la clave desde el appsettings
            var jwtKey = _configuration["Jwt:Key"] ?? "DefaultSecretKeyForJwtTokenGeneration";
            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

            // Configuramos los mismos parámetros de validación de antes
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = false, 
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            // Validar Token. Si algo está mal (firma, expiración, issuer), acá salta una excepción.
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            // 3. Mapeo explícito y seguro de los Claims para tu Controlador
            // Como tu token viene con URLs largas de SOAP, las buscamos y las normalizamos a nombres cortos
            var claims = principal.Claims.ToList();

            var userId = claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" || c.Type == "sub")?.Value;
            var userName = claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" || c.Type == "name")?.Value;
            var userRole = claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" || c.Type == "role")?.Value;

            // Creamos una nueva identidad limpia con nombres estandarizados cortos
            var identityLimpiada = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId ?? ""),
                new Claim(ClaimTypes.Name, userName ?? ""),
                new Claim(ClaimTypes.Role, userRole ?? "")
            }, "CustomJwtAuth");

            // Le asignamos esta identidad al contexto de la petición actual
            context.User = new ClaimsPrincipal(identityLimpiada);
        }
    }
}