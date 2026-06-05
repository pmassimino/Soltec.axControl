using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soltec.axControl.Server.Data;
using Soltec.axControl.Server.Model;

namespace Soltec.axControl.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CerealesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CerealesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/cereales
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var cereales = await _context.Cereales            
            .OrderBy(c => c.Nombre)
            .Select(t => new { t.Id, t.Nombre })
            .ToListAsync();
        
        return Ok(cereales);
    }

    // GET: api/cereales/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var cereal = await _context.Cereales          
            .Select(c => new { c.Id, c.Nombre })
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cereal == null)
        {
            return NotFound(new { message = "Cereal no encontrado" });
        }

        return Ok(cereal);
    }

    // POST: api/cereales
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CerealCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return BadRequest(new { message = "El nombre es requerido" });
        }

        var existeId = await _context.Cereales.AnyAsync(c => c.Id == request.Id);
        if (existeId)
        {
            return BadRequest(new { message = $"Ya existe un cereal con el ID {request.Id}" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var cereal = new Cereal
        {
            Id = request.Id,
            Nombre = request.Nombre
        };

        _context.Cereales.Add(cereal);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = cereal.Id }, cereal);
    }

    // PUT: api/cereales/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CerealUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return BadRequest(new { message = "El nombre es requerido" });
        }

        var cereal = await _context.Cereales.FindAsync(id);

        if (cereal == null)
        {
            return NotFound(new { message = "Cereal no encontrado" });
        }

        cereal.Nombre = request.Nombre;

        await _context.SaveChangesAsync();

        return Ok(cereal);
    }

    // DELETE: api/cereales/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cereal = await _context.Cereales            
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cereal == null)
        {
            return NotFound(new { message = "Cereal no encontrado" });
        }

        var filasAsociadas = await _context.Filas
            .Where(f => f.Cereales.Contains(cereal))
            .ToListAsync();

        if (filasAsociadas.Any())
        {
            return BadRequest(new { message = "No se puede eliminar un cereal que tiene filas asociadas" });
        }

        _context.Cereales.Remove(cereal);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Cereal eliminado correctamente" });
    }
}

// DTOs para las solicitudes
public record CerealCreateRequest(int Id, string Nombre);
public record CerealUpdateRequest(string Nombre);