using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soltec.axControl.Server.Data;
using Soltec.axControl.Server.Model;

namespace Soltec.axControl.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EstadosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/estados
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var estados = await _context.Estados
            .OrderBy(e => e.Nombre)
            .ToListAsync();
        
        return Ok(estados);
    }

    // GET: api/estados/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var estado = await _context.Estados
            .FirstOrDefaultAsync(e => e.Id == id);

        if (estado == null)
        {
            return NotFound(new { message = "Estado no encontrado" });
        }

        return Ok(estado);
    }

    // POST: api/estados
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EstadoCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return BadRequest(new { message = "El nombre es requerido" });
        }

        var existeId = await _context.Estados.AnyAsync(e => e.Id == request.Id);
        if (existeId)
        {
            return BadRequest(new { message = $"Ya existe un estado con el ID {request.Id}" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var estado = new Estado
        {
            Id = request.Id,
            Nombre = request.Nombre,
            Logo = request.Logo
        };

        _context.Estados.Add(estado);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = estado.Id }, estado);
    }

    // PUT: api/estados/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] EstadoUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return BadRequest(new { message = "El nombre es requerido" });
        }

        var estado = await _context.Estados.FindAsync(id);

        if (estado == null)
        {
            return NotFound(new { message = "Estado no encontrado" });
        }

        estado.Nombre = request.Nombre;
        estado.Logo = request.Logo;

        await _context.SaveChangesAsync();

        return Ok(estado);
    }

    // DELETE: api/estados/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var estado = await _context.Estados
            .FirstOrDefaultAsync(e => e.Id == id);

        if (estado == null)
        {
            return NotFound(new { message = "Estado no encontrado" });
        }

        var transicionesAsociadas = await _context.TransicionesEstado
            .Where(t => t.EstadoOrigenId == id || t.EstadoDestinoId == id)
            .ToListAsync();

        if (transicionesAsociadas.Any())
        {
            return BadRequest(new { message = "No se puede eliminar un estado que tiene transiciones asociadas" });
        }

        _context.Estados.Remove(estado);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Estado eliminado correctamente" });
    }
}

// DTOs para las solicitudes
public record EstadoCreateRequest(int Id, string Nombre, string Logo);
public record EstadoUpdateRequest(int Id, string Nombre, string Logo);