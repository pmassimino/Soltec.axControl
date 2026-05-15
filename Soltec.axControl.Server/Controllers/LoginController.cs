using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
    var usuario = await _context.Usuarios
        .FirstOrDefaultAsync(u => u.Nombre == request.usuario);

    // 3. Verificamos la contraseña usando BCrypt.Verify
    // Esta función extrae el Salt del 'usuario.PasswordHash' automáticamente
    if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.password, usuario.PasswordHash))
    {
        return Unauthorized(new { message = "Credenciales inválidas" });
    }

    // 4. Si pasó el Verify, las credenciales son correctas
    var token = GenerarJwtToken(usuario);
    
    return Ok(new { 
        token = token,
        usuario = usuario.Nombre 
    });
}
    private string GenerarJwtToken(Usuario usuario)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
        new Claim(ClaimTypes.Name, usuario.Nombre),
        // Puedes agregar roles aquí si los tienes
        new Claim(ClaimTypes.Role, "Usuario Estándar")
    };

        // La llave debe venir preferiblemente de tu appsettings.json
        var jwtKey = _configuration["Jwt:Key"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30), // Tiempo de vida del token
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
public record LoginRequest(string usuario, string password);