using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soltec.axControl.Server.Data;
using Soltec.axControl.Server.Model;


namespace Soltec.axControl.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsuarioController(ApplicationDbContext context)
    {        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {     var usuarios = await _context.Usuarios
            .Select(u => new { u.Id, u.Nombre })
            .ToListAsync();
        
        return Ok(usuarios);
    }
}