using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soltec.axControl.Server.Data;
using Soltec.axControl.Server.Model;

namespace Soltec.axControl.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TiposSujetoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TiposSujetoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/tipossujeto
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var tiposSujeto = await _context.TiposSujeto
            .OrderBy(t => t.Nombre)
            .Select(t => new { t.Id, t.Nombre })
            .ToListAsync();
        
        return Ok(tiposSujeto);
    }

    // GET: api/tipossujeto/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var tipoSujeto = await _context.TiposSujeto
            .Where(t => t.Id == id)
            .Select(t => new { t.Id, t.Nombre })
            .FirstOrDefaultAsync();

        if (tipoSujeto == null)
        {
            return NotFound(new { message = "Tipo de sujeto no encontrado" });
        }

        return Ok(tipoSujeto);
    }

    // POST: api/tipossujeto
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TipoSujetoCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return BadRequest(new { message = "El nombre es requerido" });
        }

        var existeId = await _context.TiposSujeto.AnyAsync(t => t.Id == request.Id);
        if (existeId)
        {
            return BadRequest(new { message = $"Ya existe un tipo de sujeto con el ID {request.Id}" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var tipoSujeto = new TipoSujeto
        {
            Id = request.Id,
            Nombre = request.Nombre
        };

        _context.TiposSujeto.Add(tipoSujeto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = tipoSujeto.Id }, tipoSujeto);
    }

    // PUT: api/tipossujeto/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TipoSujetoUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return BadRequest(new { message = "El nombre es requerido" });
        }

        var tipoSujeto = await _context.TiposSujeto.FindAsync(id);

        if (tipoSujeto == null)
        {
            return NotFound(new { message = "Tipo de sujeto no encontrado" });
        }

        tipoSujeto.Nombre = request.Nombre;

        await _context.SaveChangesAsync();

        return Ok(tipoSujeto);
    }

    // DELETE: api/tipossujeto/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tipoSujeto = await _context.TiposSujeto
            .Include(t => t.Sujetos)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tipoSujeto == null)
        {
            return NotFound(new { message = "Tipo de sujeto no encontrado" });
        }

        if (tipoSujeto.Sujetos != null && tipoSujeto.Sujetos.Any())
        {
            return BadRequest(new { message = "No se puede eliminar un tipo de sujeto que tiene sujetos asociados" });
        }

        _context.TiposSujeto.Remove(tipoSujeto);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Tipo de sujeto eliminado correctamente" });
    }
}

// DTOs para las solicitudes
public record TipoSujetoCreateRequest(int Id, string Nombre);
public record TipoSujetoUpdateRequest(int Id, string Nombre);