using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soltec.axControl.Server.Data;
using Soltec.axControl.Server.Model;

namespace Soltec.axControl.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FilasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FilasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/filas
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var filas = await _context.Filas
            .Include(f => f.Zona)
            .OrderBy(f => f.Nombre)
            .Select(f => new { f.Id, f.Nombre, f.Capacidad, f.TipoOperacion, f.ZonaId,NombreZona=f.Zona.Nombre,f.Cereales })
            .ToListAsync();
        
        return Ok(filas);
    }

    [HttpGet("stock")]
    public async Task<IActionResult> GetStockFilas(int zonaId)
    {
        var filas = await _context.Filas
            .Include(f => f.Zona)            
            .OrderBy(f => f.Nombre)
            .Select(f => new { f.Id, f.Nombre, f.Capacidad,
            Ocupadas=_context.OrdenesTransito.Where(w=>w.FilaId==f.Id && w.EstadoId !=9 && w.EstadoId !=10 ).Count(),
            Stock = f.Capacidad - (_context.OrdenesTransito.Where(w=>w.FilaId==f.Id && w.EstadoId !=9 && w.EstadoId !=10).Count()), f.TipoOperacion, f.ZonaId,NombreZona=f.Zona.Nombre,f.Cereales })
            .ToListAsync();
        
        return Ok(filas);
    }

    // GET: api/filas/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var fila = await _context.Filas
            .Include(f => f.Zona)
            .Include(f => f.Cereales)
            .Where(f => f.Id == id)
            .Select(f => new { f.Id, f.Nombre, f.Capacidad, f.TipoOperacion, f.ZonaId,NombreZona=f.Zona.Nombre, f.Cereales })
            .FirstOrDefaultAsync();

        if (fila == null)
        {
            return NotFound(new { message = "Fila no encontrada" });
        }

        return Ok(fila);
    }

    // POST: api/filas
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FilaCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return BadRequest(new { message = "El nombre es requerido" });
        }

        var existeZona = await _context.Zonas.AnyAsync(z => z.Id == request.ZonaId);
        if (!existeZona)
        {
            return BadRequest(new { message = $"No existe una zona con el ID {request.ZonaId}" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var fila = new Fila
        {
            Nombre = request.Nombre,
            Capacidad = request.Capacidad,
            TipoOperacion = request.TipoOperacion,
            ZonaId = request.ZonaId
        };

        _context.Filas.Add(fila);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = fila.Id }, fila);
    }

    // PUT: api/filas/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] FilaUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return BadRequest(new { message = "El nombre es requerido" });
        }

        var fila = await _context.Filas.FindAsync(id);

        if (fila == null)
        {
            return NotFound(new { message = "Fila no encontrada" });
        }

        var existeZona = await _context.Zonas.AnyAsync(z => z.Id == request.ZonaId);
        if (!existeZona)
        {
            return BadRequest(new { message = $"No existe una zona con el ID {request.ZonaId}" });
        }

        fila.Nombre = request.Nombre;
        fila.Capacidad = request.Capacidad;
        fila.TipoOperacion = request.TipoOperacion;
        fila.ZonaId = request.ZonaId;

        await _context.SaveChangesAsync();

        return Ok(fila);
    }

    // DELETE: api/filas/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var fila = await _context.Filas
            .Include(f => f.Cereales)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (fila == null)
        {
            return NotFound(new { message = "Fila no encontrada" });
        }

        var ordenesAsociadas = await _context.OrdenesTransito
            .Where(o => o.FilaId == id)
            .ToListAsync();

        if (ordenesAsociadas.Any())
        {
            return BadRequest(new { message = "No se puede eliminar una fila que tiene órdenes de tránsito asociadas" });
        }

        if (fila.Cereales.Any())
        {
            fila.Cereales.Clear();
        }

        _context.Filas.Remove(fila);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Fila eliminada correctamente" });
    }

    // POST: api/filas/{id}/cereales
    [HttpPost("{id}/cereales")]
    public async Task<IActionResult> AddCereal(int id, [FromBody] FilaAddCerealRequest request)
    {
        var fila = await _context.Filas
            .Include(f => f.Cereales)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (fila == null)
        {
            return NotFound(new { message = "Fila no encontrada" });
        }

        var cereal = await _context.Cereales.FindAsync(request.CerealId);
        if (cereal == null)
        {
            return BadRequest(new { message = "Cereal no encontrado" });
        }

        if (fila.Cereales.Any(c => c.Id == request.CerealId))
        {
            return BadRequest(new { message = "El cereal ya está asociado a esta fila" });
        }

        fila.Cereales.Add(cereal);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Cereal agregado a la fila correctamente" });
    }

    // DELETE: api/filas/{id}/cereales/{cerealId}
    [HttpDelete("{id}/cereales/{cerealId}")]
    public async Task<IActionResult> RemoveCereal(int id, int cerealId)
    {
        var fila = await _context.Filas
            .Include(f => f.Cereales)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (fila == null)
        {
            return NotFound(new { message = "Fila no encontrada" });
        }

        var cereal = fila.Cereales.FirstOrDefault(c => c.Id == cerealId);
        if (cereal == null)
        {
            return BadRequest(new { message = "El cereal no está asociado a esta fila" });
        }

        fila.Cereales.Remove(cereal);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Cereal eliminado de la fila correctamente" });
    }
}

// DTOs para las solicitudes
public record FilaCreateRequest(string Nombre, int Capacidad, TipoOperacion TipoOperacion, int ZonaId);
public record FilaUpdateRequest(string Nombre, int Capacidad, TipoOperacion TipoOperacion, int ZonaId);
public record FilaAddCerealRequest(int CerealId);