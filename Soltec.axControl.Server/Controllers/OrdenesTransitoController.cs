using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soltec.axControl.Server.Data;
using Soltec.axControl.Server.Model;

namespace Soltec.axControl.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdenesTransitoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OrdenesTransitoController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/ordenes-transito
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var ordenes = await _context.OrdenesTransito
            .Include(o => o.Cereal)
            .Include(o => o.Circuito)
            .Include(o => o.Estado)
            .Include(o => o.Fila)
            .OrderBy(o => o.Fecha)
            .ToListAsync();

        return Ok(ordenes);
    }

    // GET: api/estados/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var orden = await _context.OrdenesTransito
    .Where(o => o.Id == id) // Primero filtramos
    .Select(o => new
    {
        o.Id,
        o.Fecha,
        Cereal = new { o.Cereal.Id, o.Cereal.Nombre },
        Circuito = new { o.Circuito.Id, o.Circuito.Nombre },
        // Usamos las propiedades de navegación si existen, sino dejamos la subconsulta
        Transporte = new
        {
            o.TransporteId,
            Nombre = _context.Sujetos.Where(s => s.Id == o.TransporteId).Select(s => s.Nombre).FirstOrDefault()
        },
        Chofer = new
        {
            o.ChoferId,
            Nombre = _context.Sujetos.Where(s => s.Id == o.ChoferId).Select(s => s.Nombre).FirstOrDefault()
        },
        o.PatenteChasis,
        o.PatenteAcoplado,
        o.PatenteOpc,
        o.NumeroComprobante,
        o.TagRFID,
        Estado = new { o.Estado.Id, o.Estado.Nombre },
        Fila = new { o.Fila.Id, o.Fila.Nombre },
        o.Observacion,
        HistorialEstados = o.HistorialEstados.Select(he => new
        {
            he.Id,
            Estado = new { he.Estado.Id, he.Estado.Nombre },
            he.Fecha,
            he.Observacion
        }),
        HistorialFilas = o.HistorialFilas.Select(hf => new
        {
            hf.Id,
            Fila = new { hf.Fila.Id, hf.Fila.Nombre },
            hf.Fecha,
            hf.Observacion
        })
    })
    .FirstOrDefaultAsync(); // Ejecutamos la consulta al final

        if (orden == null)
        {
            return NotFound(new { message = "Orden de tránsito no encontrada" });
        }

        return Ok(orden);
    }

    // POST: api/ordenes-transito
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrdenTransitoDto request)
    {
        var existeCereal = await _context.Cereales.AnyAsync(c => c.Id == request.CerealId);
        if (!existeCereal)
        {
            return BadRequest(new { message = $"No existe un cereal con el ID {request.CerealId}" });
        }
        var existeCircuito = await _context.Circuitos.AnyAsync(c => c.Id == request.CircuitoId);
        if (!existeCircuito)
        {
            return BadRequest(new { message = $"No existe un circuito con el ID {request.CircuitoId}" });
        }
        var existeEstado = await _context.Estados.AnyAsync(e => e.Id == request.EstadoId);
        if (!existeEstado)
        {
            return BadRequest(new { message = $"No existe un estado con el ID {request.EstadoId}" });
        }
        if (request.EstadoId != 1 && request.EstadoId != 9) // Suponiendo que el estado "Pendiente" tiene ID 1 y "Cancelado" tiene ID 11
        {
            return BadRequest(new { message = "La orden de tránsito debe comenzar en estado Ingreso o Pendiente Documentacion" });
        }
        var existeFila = await _context.Filas.AnyAsync(f => f.Id == request.FilaId);
        if (!existeFila)
        {
            return BadRequest(new { message = $"No existe una fila con el ID {request.FilaId}" });
        }
        var existeTransporte = await _context.Sujetos.AnyAsync(t => t.Id == request.TransporteId);
        if (!existeTransporte)
        {
            return BadRequest(new { message = $"No existe un transporte con el ID {request.TransporteId}" });
        }
        var existeChofer = await _context.Sujetos.AnyAsync(t => t.Id == request.ChoferId);
        if (!existeChofer)
        {
            return BadRequest(new { message = $"No existe un chofer con el ID {request.ChoferId}" });
        }
        if (string.IsNullOrWhiteSpace(request.PatenteChasis))
        {
            return BadRequest(new { message = "La patente del chasis es requerida" });
        }
        var existeOrdenAbierta = await _context.OrdenesTransito
            .AnyAsync(o => o.TransporteId == request.TransporteId && o.ChoferId == request.ChoferId && (o.EstadoId != 10 && o.EstadoId != 11));
        if (existeOrdenAbierta)
        {
            return BadRequest(new { message = "Ya existe una orden de tránsito abierta para este transporte y chofer" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var ordenTransito = new OrdenTransito
        {
            Id = request.Id,
            Fecha = DateTime.Now,
            CerealId = request.CerealId,
            CircuitoId = request.CircuitoId,
            TransporteId = request.TransporteId,
            ChoferId = request.ChoferId,
            PatenteChasis = request.PatenteChasis,
            PatenteAcoplado = request.PatenteAcoplado,
            PatenteOpc = request.PatenteOpc,
            NumeroComprobante = request.NumeroComprobante,
            TagRFID = request.TagRFID,
            EstadoId = request.EstadoId,
            SectorId = request.SectorId,
            FilaId = request.FilaId,
            Observacion = request.Observacion
        };
        ordenTransito.HistorialEstados.Add(new EstadoHistorial
        {
            EstadoId = request.EstadoId,
            Fecha = DateTime.Now,
            Observacion = $"Orden creada con estado {request.EstadoId}"
        });
        ordenTransito.HistorialSectores.Add(new SectorHistorial
        {
            SectorId = request.SectorId,
            Fecha = DateTime.Now,
            Observacion = ""            
        });
        ordenTransito.HistorialComprobantes.Add(new ComprobanteHistorial
        {
                Numero = request.NumeroComprobante,
                Fecha = DateTime.Now,
                Observacion = ""            
        });       
        ordenTransito.HistorialFilas.Add(new FilaHistorial
        {
            FilaId = request.FilaId,
            Fecha = DateTime.Now,
            Observacion = $"Orden asignada a fila {request.FilaId}"
        });

        _context.OrdenesTransito.Add(ordenTransito);
        try
        {
            await _context.SaveChangesAsync();

        }
        catch (DbUpdateException ex)
        {
            return BadRequest(new { message = $"Error al guardar la orden de tránsito: {ex.Message}" });
        }
        return CreatedAtAction(nameof(Get), new { id = ordenTransito.Id },
         new { ordenTransito.Id ,ordenTransito.Fecha,ordenTransito.CerealId,ordenTransito.CircuitoId,ordenTransito.TransporteId,ordenTransito.ChoferId,ordenTransito.PatenteChasis,ordenTransito.PatenteAcoplado,ordenTransito.PatenteOpc,ordenTransito.NumeroComprobante,ordenTransito.TagRFID,ordenTransito.EstadoId,ordenTransito.SectorId,ordenTransito.FilaId,ordenTransito.Observacion    });
    }
    [HttpPost]
    [Route("{id}/cambiarestado")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] CambiarEstadoRequest request)
    {        var ordenTransito = await _context.OrdenesTransito
            .Include(o => o.HistorialEstados)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (ordenTransito == null)
        {
            return NotFound(new { message = "Orden de tránsito no encontrada" });
        }
        var existeEstado = await _context.Estados.AnyAsync(e => e.Id == request.EstadoId);
        if (!existeEstado)
        {
            return BadRequest(new { message = $"No existe un estado con el ID {request.EstadoId}" });
        }
        // Validar que la transición de estado sea válida según el circuito
        var esTransicionValida = await _context.TransicionesEstado.AnyAsync(te =>
            te.CircuitoId == ordenTransito.CircuitoId &&
            te.EstadoOrigenId == ordenTransito.EstadoId &&
            te.EstadoDestinoId == request.EstadoId);

        if (!esTransicionValida)
        {
            return BadRequest(new { message = "La transición de estado no es válida para el circuito actual" });
        }

        ordenTransito.EstadoId = request.EstadoId;
        ordenTransito.HistorialEstados.Add(new EstadoHistorial
        {
            EstadoId = request.EstadoId,
            Fecha = DateTime.Now,
            Observacion = request.Observacion
        });

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(new { message = $"Error al cambiar el estado de la orden de tránsito: {ex.Message}" });
        }

        return Ok(new { message = "Estado de la orden de tránsito actualizado correctamente" });
    }   
    [HttpPost]
    [Route("{id}/cambiarfila")]                 
    public async Task<IActionResult> CambiarFila(int id, [FromBody] CambiarFilaRequest request)
    {
        var ordenTransito = await _context.OrdenesTransito
            .Include(o => o.HistorialFilas)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (ordenTransito == null)
        {
            return NotFound(new { message = "Orden de tránsito no encontrada" });
        }
        if (ordenTransito.FilaId==request.FilaId)
        {
            return BadRequest(new { message = "La orden de tránsito ya está asignada a esta fila" });
        }
        var existeFila = await _context.Filas.AnyAsync(f => f.Id == request.FilaId);
        if (!existeFila)
        {
            return BadRequest(new { message = $"No existe una fila con el ID {request.FilaId}" });
        }

        ordenTransito.FilaId = request.FilaId;
        ordenTransito.HistorialFilas.Add(new FilaHistorial
        {
            FilaId = request.FilaId,
            Fecha = DateTime.Now,
            Observacion = request.Observacion
        });

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(new { message = $"Error al cambiar la fila de la orden de tránsito: {ex.Message}" });
        }

        return Ok(new { message = "Fila de la orden de tránsito actualizada correctamente" });
    }
    
    [HttpGet("{id}/estadosDisponibles")]
    public async Task<IActionResult> GetEstadosDisponibles(int id)
    {
        var ordenTransito = await _context.OrdenesTransito
            .FirstOrDefaultAsync(o => o.Id == id);

        if (ordenTransito == null)
        {
            return NotFound(new { message = "Orden de tránsito no encontrada" });
        }

        var estadosDisponibles = await _context.TransicionesEstado
            .Include(i=>i.EstadoDestino)
            .Where(t => t.CircuitoId == ordenTransito.CircuitoId && t.EstadoOrigenId == ordenTransito.EstadoId)
            .Select(t => new {t.EstadoDestinoId, t.EstadoDestino.Nombre, t.EstadoDestino.Logo})
            .ToListAsync();

        return Ok(new { estadosDisponibles });
    }
    // DELETE: api/estados/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ordenTransito = await _context.OrdenesTransito
            .FirstOrDefaultAsync(o => o.Id == id);

        if (ordenTransito == null)
        {
            return NotFound(new { message = "Orden de tránsito no encontrada" });
        }
        if (ordenTransito.EstadoId != 1) // Suponiendo que el estado "Pendiente" tiene ID 1
        {
            return BadRequest(new { message = "No se puede eliminar una orden de tránsito en este estado" });
        }
        _context.OrdenesTransito.Remove(ordenTransito);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return BadRequest(new { message = $"Error al eliminar la orden de tránsito: {ex.Message}" });
        }

        return Ok(new { message = "Orden de tránsito eliminada correctamente" });
    }
}

// DTOs para las solicitudes
public record OrdenTransitoDto(
    int Id,
    DateTime Fecha,
    int CerealId,
    int CircuitoId,
    int TransporteId,
    int ChoferId,
    string PatenteChasis,
    string PatenteAcoplado,
    string PatenteOpc,
    string NumeroComprobante,
    string TagRFID,
    int EstadoId,
    int SectorId,
    int FilaId,
    string Observacion
);
public record CambiarEstadoRequest(
    int EstadoId,
    string Observacion
);
public record CambiarFilaRequest(
    int FilaId,
    string Observacion
);