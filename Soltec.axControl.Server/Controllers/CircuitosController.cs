using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soltec.axControl.Server.Data;
using Soltec.axControl.Server.Model;

namespace Soltec.axControl.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CircuitosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CircuitosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/circuitos
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var circuitos = await _context.Circuitos
            .Include(c => c.Transiciones)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
        
        return Ok(circuitos);
    }

    // GET: api/circuitos/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var circuito = await _context.Circuitos
            .Include(c => c.Transiciones)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (circuito == null)
        {
            return NotFound(new { message = "Circuito no encontrado" });
        }

        return Ok(circuito);
    }

    // POST: api/circuitos
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CircuitoCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return BadRequest(new { message = "El nombre es requerido" });
        }

        var existeId = await _context.Circuitos.AnyAsync(c => c.Id == request.Id);
        if (existeId)
        {
            return BadRequest(new { message = $"Ya existe un circuito con el ID {request.Id}" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var circuito = new Circuito
        {
            Id = request.Id,
            Nombre = request.Nombre
        };

        _context.Circuitos.Add(circuito);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = circuito.Id }, circuito);
    }

    // PUT: api/circuitos/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CircuitoUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return BadRequest(new { message = "El nombre es requerido" });
        }

        var circuito = await _context.Circuitos.FindAsync(id);

        if (circuito == null)
        {
            return NotFound(new { message = "Circuito no encontrado" });
        }

        circuito.Nombre = request.Nombre;

        await _context.SaveChangesAsync();

        return Ok(circuito);
    }

    // DELETE: api/circuitos/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var circuito = await _context.Circuitos
            .Include(c => c.Transiciones)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (circuito == null)
        {
            return NotFound(new { message = "Circuito no encontrado" });
        }

        if (circuito.Transiciones != null && circuito.Transiciones.Any())
        {
            return BadRequest(new { message = "No se puede eliminar un circuito que tiene transiciones asociadas" });
        }

        _context.Circuitos.Remove(circuito);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Circuito eliminado correctamente" });
    }
}

// DTOs para las solicitudes
public record CircuitoCreateRequest(int Id, string Nombre);
public record CircuitoUpdateRequest(int Id, string Nombre);