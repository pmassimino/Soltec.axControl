using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soltec.axControl.Server.Data;
using Soltec.axControl.Server.Model;

namespace Soltec.axControl.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ZonasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ZonasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/zonas
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var zonas = await _context.Zonas
            .Include(z => z.Filas)
            .OrderBy(z => z.Orden)
            .ToListAsync();
        
        return Ok(zonas);
    }

    // GET: api/zonas/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var zona = await _context.Zonas
            .Include(z => z.Filas)
            .FirstOrDefaultAsync(z => z.Id == id);

        if (zona == null)
        {
            return NotFound(new { message = "Zona no encontrada" });
        }

        return Ok(zona);
    }

    // POST: api/zonas
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ZonaCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return BadRequest(new { message = "El nombre es requerido" });
        }

        var existeId = await _context.Zonas.AnyAsync(z => z.Id == request.Id);
        if (existeId)
        {
            return BadRequest(new { message = $"Ya existe una zona con el ID {request.Id}" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var zona = new Zona
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            Orden = request.Orden
        };

        _context.Zonas.Add(zona);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = zona.Id }, zona);
    }

    // PUT: api/zonas/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ZonaUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return BadRequest(new { message = "El nombre es requerido" });
        }

        var zona = await _context.Zonas.FindAsync(id);

        if (zona == null)
        {
            return NotFound(new { message = "Zona no encontrada" });
        }

        zona.Nombre = request.Nombre;
        zona.Descripcion = request.Descripcion;
        zona.Orden = request.Orden;

        await _context.SaveChangesAsync();

        return Ok(zona);
    }

    // DELETE: api/zonas/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var zona = await _context.Zonas
            .Include(z => z.Filas)
            .FirstOrDefaultAsync(z => z.Id == id);

        if (zona == null)
        {
            return NotFound(new { message = "Zona no encontrada" });
        }

        if (zona.Filas != null && zona.Filas.Any())
        {
            return BadRequest(new { message = "No se puede eliminar una zona que tiene filas asociadas" });
        }

        _context.Zonas.Remove(zona);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Zona eliminada correctamente" });
    }
}

// DTOs para las solicitudes
public record ZonaCreateRequest(int Id,string Nombre, string Descripcion, int Orden);
public record ZonaUpdateRequest(int Id,string Nombre, string Descripcion, int Orden);