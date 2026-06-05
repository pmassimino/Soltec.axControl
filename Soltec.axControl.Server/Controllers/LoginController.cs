using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Soltec.axControl.Server.Data;
using Soltec.axControl.Server.Model;


namespace Soltec.axControl.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public LoginController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        // 1. Validaciones básicas
        if (string.IsNullOrEmpty(request.usuario) || string.IsNullOrEmpty(request.password))
        {
            return BadRequest(new { message = "Usuario y contraseña son requeridos" });
        }

        // 2. Buscamos al usuario SOLO por el nombre
        var usuario = await _context.Usuarios.
                            Include(u => u.Roles).ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(u => u.Nombre == request.usuario);

        // 3. Verificamos la contraseña usando BCrypt.Verify
        // Esta función extrae el Salt del 'usuario.PasswordHash' automáticamente
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.password, usuario.PasswordHash))
        {
            return Unauthorized(new { message = "Credenciales inválidas" });
        }

        // 4. Si pasó el Verify, las credenciales son correctas
        var token = GenerarTokenJWT(usuario);

        return Ok(new
        {
            token = token
        });
    }

    private string GenerarTokenJWT(Usuario user)
    {
        var claims = new List<Claim>()
{
    new Claim(JwtRegisteredClaimNames.Sub, user.Nombre),

    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Name, user.Nombre),
    new Claim(ClaimTypes.Email, user.Email ?? "")
};

        if (user.Roles != null)
        {
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Rol?.Nombre));
            }
        }
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

        return new JwtSecurityTokenHandler().WriteToken(token);

    }


    [HttpGet("test")]
    [Authorize]
    public IActionResult Test()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = User.Identity?.Name;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        return Ok(new { message = $"Autenticado como {userName} (ID: {userId})" });
    }
}
public record LoginRequest(string usuario, string password);