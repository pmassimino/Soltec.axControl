using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soltec.axControl.Server.Data;
using Soltec.axControl.Server.Model;

namespace Soltec.axControl.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransicionesEstadoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TransicionesEstadoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/transicionesestado
    [HttpGet]
public async Task<IActionResult> Index()
{
    var transiciones = await _context.TransicionesEstado
        .Select(t => new {
            t.Id,
            CircuitoId = t.CircuitoId,
            CircuitoNombre = t.Circuito.Nombre, // Solo traés lo que necesitás
            EstadoOrigenId = t.EstadoOrigenId,  
            EstadoOrigen = t.EstadoOrigen.Nombre,
            EstadoDestinoId = t.EstadoDestinoId,
            EstadoDestino = t.EstadoDestino.Nombre,            
        })
        .ToListAsync();
    
    return Ok(transiciones);
}
    // GET: api/transicionesestado/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var transicion = await _context.TransicionesEstado
        .Select(t => new {
            t.Id,
            CircuitoId = t.CircuitoId,
            CircuitoNombre = t.Circuito.Nombre, // Solo traés lo que necesitás
            EstadoOrigenId = t.EstadoOrigenId,  
            EstadoOrigen = t.EstadoOrigen.Nombre,
            EstadoDestinoId = t.EstadoDestinoId,
            EstadoDestino = t.EstadoDestino.Nombre,            
        })
        .FirstOrDefaultAsync(t => t.Id == id)  ;        
        
        if (transicion == null)
        {
            return NotFound(new { message = "Transición no encontrada" });
        }

        return Ok(transicion);
    }

    // POST: api/transicionesestado
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TransicionEstadoCreateRequest request)
    {
        // Validamos que existan las entidades relacionadas
        if (!await _context.Circuitos.AnyAsync(c => c.Id == request.CircuitoId))
        {
            return BadRequest(new { message = $"No existe un circuito con el ID {request.CircuitoId}" });
        }

        if (!await _context.Estados.AnyAsync(e => e.Id == request.EstadoOrigenId))
        {
            return BadRequest(new { message = $"No existe un estado de origen con el ID {request.EstadoOrigenId}" });
        }

        if (!await _context.Estados.AnyAsync(e => e.Id == request.EstadoDestinoId))
        {
            return BadRequest(new { message = $"No existe un estado de destino con el ID {request.EstadoDestinoId}" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var transicion = new TransicionEstado
        {
            CircuitoId = request.CircuitoId,
            EstadoOrigenId = request.EstadoOrigenId,
            EstadoDestinoId = request.EstadoDestinoId
        };

        _context.TransicionesEstado.Add(transicion);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = transicion.Id }, transicion);
    }

    // PUT: api/transicionesestado/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TransicionEstadoUpdateRequest request)
    {
        var transicion = await _context.TransicionesEstado.FindAsync(id);

        if (transicion == null)
        {
            return NotFound(new { message = "Transición no encontrada" });
        }

        transicion.CircuitoId = request.CircuitoId;
        transicion.EstadoOrigenId = request.EstadoOrigenId;
        transicion.EstadoDestinoId = request.EstadoDestinoId;

        await _context.SaveChangesAsync();

        return Ok(transicion);
    }

    // GET: api/transicionesestado/disponibles?circuitoId=1&estadoOrigenId=2
    [HttpGet("disponibles")]
    public async Task<IActionResult> GetEstadosDisponibles(int circuitoId, int estadoOrigenId)
    {
        // Validar que el circuito existe
        var circuito = await _context.Circuitos.FindAsync(circuitoId);
        if (circuito == null)
        {
            return BadRequest(new { message = $"No existe un circuito con el ID {circuitoId}" });
        }

        // Validar que el estado origen existe
        var estadoOrigen = await _context.Estados.FindAsync(estadoOrigenId);
        if (estadoOrigen == null)
        {
            return BadRequest(new { message = $"No existe un estado origen con el ID {estadoOrigenId}" });
        }

        // Obtener las transiciones disponibles desde el estado origen en el circuito especificado
        var transicionesDisponibles = await _context.TransicionesEstado
            .Where(t => t.CircuitoId == circuitoId && t.EstadoOrigenId == estadoOrigenId)
            .Select(t => new {
                t.Id,
                t.CircuitoId,
                CircuitoNombre = t.Circuito.Nombre,
                t.EstadoOrigenId,
                EstadoOrigen = t.EstadoOrigen.Nombre,
                t.EstadoDestinoId,
                EstadoDestino = t.EstadoDestino.Nombre,
                EstadoDestinoLogo = t.EstadoDestino.Logo
            })
            .ToListAsync();

        if (!transicionesDisponibles.Any())
        {
            return Ok(new { 
                message = $"No hay transiciones disponibles desde el estado '{estadoOrigen.Nombre}' en el circuito '{circuito.Nombre}'",
                transiciones = transicionesDisponibles
            });
        }

        return Ok(transicionesDisponibles);
    }

    // DELETE: api/transicionesestado/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var transicion = await _context.TransicionesEstado.FindAsync(id);

        if (transicion == null)
        {
            return NotFound(new { message = "Transición no encontrada" });
        }

        _context.TransicionesEstado.Remove(transicion);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Transición eliminada correctamente" });
    }
}

// DTOs para las solicitudes
public record TransicionEstadoCreateRequest(int CircuitoId, int EstadoOrigenId, int EstadoDestinoId);
public record TransicionEstadoUpdateRequest(int CircuitoId, int EstadoOrigenId, int EstadoDestinoId);