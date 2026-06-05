
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soltec.axControl.Server.Data;
using Soltec.axControl.Server.Model;
using Soltec.axControl.Server.Code;
using Microsoft.AspNetCore.Authorization;

namespace Soltec.axControl.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SujetosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SujetosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/sujetos
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var sujetos = await _context.Sujetos
            .Include(s => s.TiposSujeto)
            .OrderBy(s => s.Nombre)
            .Select(s => new
            {
                s.Id,
                s.Nombre,
                s.NumeroDocumento,
                s.Email,
                s.Telefono,
                s.ExternoId,
                TiposSujeto = s.TiposSujeto.Select(t => new { t.Id, t.Nombre }),
                Choferes = s.Choferes.Select(c => new { c.Id, c.SujetoId, c.Sujeto.Nombre }),
                 Patente = s.Patentes.Select(p => new { p.Id, p.PatenteCamion, p.PatenteAcoplado, p.PatenteOpcional }
                )   
            })
            .ToListAsync();

        return Ok(sujetos);
    }

    // GET: api/sujetos/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var sujeto = await _context.Sujetos
            .Include(s => s.TiposSujeto)
            .Include(s => s.Patentes)
            .Where(s => s.Id == id)
            .Select(s => new
            {
                s.Id,
                s.Nombre,
                s.NumeroDocumento,
                s.Email,
                s.Telefono,
                s.ExternoId,
                TiposSujeto = s.TiposSujeto.Select(t => new { t.Id, t.Nombre }),
                Patente = s.Patentes.Select(p => new { p.Id, p.PatenteCamion, p.PatenteAcoplado, p.PatenteOpcional }
                )
            })
            .FirstOrDefaultAsync();

        if (sujeto == null)
        {
            return NotFound(new { message = "Sujeto no encontrado" });
        }

        return Ok(sujeto);
    }

    // POST: api/sujetos
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SujetoCreateRequest request)
    {
        // 1. Validaciones básicas de entrada
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(request.Nombre))
            return BadRequest(new { message = "El nombre es requerido" });

        if (!CoreServices.IsValidCuit("80", request.NumeroDocumento))
            return BadRequest(new { message = "El número de documento no es válido" });
        var existingSujeto = await _context.Sujetos.FirstOrDefaultAsync(s => s.NumeroDocumento == request.NumeroDocumento);
        if (existingSujeto != null)
        {
            return BadRequest(new { message = "Ya existe un sujeto con el mismo número de documento" });
        }
        if (!CoreServices.IsValidEmail(request.Email))
            return BadRequest(new { message = "El email no es válido" });


        var sujeto = new Sujeto
        {
            Nombre = request.Nombre,
            NumeroDocumento = request.NumeroDocumento,
            Email = request.Email,
            Telefono = request.Telefono,
            ExternoId = request.ExternoId,
        };
        _context.Sujetos.Add(sujeto);
        try
        {
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = sujeto.Id }, sujeto);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new { message = "Error al guardar en la base de datos", details = ex.Message });
        }

    }

    // PUT: api/sujetos/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SujetoCreateRequest request)
    {
        // 1. Validaciones de entrada
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(request.Nombre))
            return BadRequest(new { message = "El nombre es requerido" });

        if (!CoreServices.IsValidCuit("80", request.NumeroDocumento))
            return BadRequest(new { message = "El número de documento no es válido" });

        if (!CoreServices.IsValidEmail(request.Email))
            return BadRequest(new { message = "El email no es válido" });

        // 2. Buscar el sujeto existente por ID
        var sujeto = await _context.Sujetos.FindAsync(id);
        if (sujeto == null)
        {
            return NotFound(new { message = $"No se encontró el sujeto con ID {id}" });
        }

        // 3. Validar que el nuevo NumeroDocumento no esté siendo usado por OTRO sujeto
        var documentoEnUso = await _context.Sujetos
            .AnyAsync(s => s.NumeroDocumento == request.NumeroDocumento && s.Id != id);

        if (documentoEnUso)
        {
            return BadRequest(new { message = "Ya existe otro sujeto con ese número de documento" });
        }

        // 4. Actualizar las propiedades (Mapeo)
        sujeto.Nombre = request.Nombre;
        sujeto.NumeroDocumento = request.NumeroDocumento;
        sujeto.Email = request.Email;
        sujeto.Telefono = request.Telefono;
        sujeto.ExternoId = request.ExternoId;

        // 5. Persistencia
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return StatusCode(500, new { message = "Error de concurrencia al actualizar" });
        }

        return NoContent(); // Respuesta estándar 204 para PUT exitoso sin contenido
    }

    // DELETE: api/sujetos/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var sujeto = await _context.Sujetos.FindAsync(id);

        if (sujeto == null)
        {
            return NotFound(new { message = "Sujeto no encontrado" });
        }

        // Verificar si está asociado en la tabla Choferes (como chofer o como transportista)
        var estaEnChoferes = await _context.Choferes.AnyAsync(c => c.SujetoId == id || c.TransportistaId == id);
        if (estaEnChoferes)
        {
            return BadRequest(new { message = "No se puede eliminar un sujeto que está asociado como Chofer o Transportista" });
        }

        _context.Sujetos.Remove(sujeto);
        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { message = "Sujeto eliminado correctamente" });
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new { message = "Error al eliminar el sujeto", details = ex.Message });
        }
    }
    [HttpPost("{SujetoId}/TipoSujeto")]
    public async Task<IActionResult> AddTipoSujeto(int SujetoId, [FromBody] int TipoSujetoId)
    {
        var sujeto = await _context.Sujetos.Include(s => s.TiposSujeto).FirstOrDefaultAsync(s => s.Id == SujetoId);
        if (sujeto == null) return NotFound(new { message = "Sujeto no encontrado" });

        var tipoSujeto = await _context.TiposSujeto.FindAsync(TipoSujetoId);
        if (tipoSujeto == null) return NotFound(new { message = "Tipo de Sujeto no encontrado" });

        if (sujeto.TiposSujeto.Contains(tipoSujeto))
            return BadRequest(new { message = "El sujeto ya tiene asignado este tipo" });

        sujeto.TiposSujeto.Add(tipoSujeto);
        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { message = "Tipo de Sujeto agregado correctamente" });
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new { message = "Error al guardar en la base de datos", details = ex.Message });
        }

    }
    [HttpDelete("{id}/TipoSujeto")]
    public async Task<IActionResult> DelTipoSujeto(int SujetoId, [FromBody] int TipoSujetoId)
    {
        var sujeto = await _context.Sujetos.Include(s => s.TiposSujeto).FirstOrDefaultAsync(s => s.Id == SujetoId);
        if (sujeto == null) return NotFound(new { message = "Sujeto no encontrado" });

        var tipoSujeto = await _context.TiposSujeto.FindAsync(TipoSujetoId);
        if (tipoSujeto == null) return NotFound(new { message = "Tipo de Sujeto no encontrado" });

        if (!sujeto.TiposSujeto.Contains(tipoSujeto))
            return BadRequest(new { message = "El sujeto no tiene asignado este tipo" });

        sujeto.TiposSujeto.Remove(tipoSujeto);
        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { message = "Tipo de Sujeto borrado correctamente" });
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new { message = "Error al guardar en la base de datos", details = ex.Message });
        }

    }

    [HttpPost("{SujetoId}/Patente")]
    public async Task<IActionResult> AddPatente(int SujetoId, [FromBody] PatenteRequest request)
    {
        var sujeto = await _context.Sujetos
        .Include(s => s.Patentes)
        .FirstOrDefaultAsync(s => s.Id == SujetoId);
        if (sujeto == null) return NotFound(new { message = "Sujeto no encontrado" });

        if (sujeto.Patentes.Any(p => p.PatenteCamion == request.PatenteCamion && p.PatenteAcoplado == request.PatenteAcoplado))
            return BadRequest(new { message = "El sujeto ya tiene asignada esta combinacion de patente" });

        var patente = new Patente
        {
            PatenteCamion = request.PatenteCamion,
            PatenteAcoplado = request.PatenteAcoplado,
            PatenteOpcional = request.PatenteOpcional
        };

        sujeto.Patentes.Add(patente);
        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { message = "Patente agregada correctamente", data = patente });
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new { message = "Error al guardar en la base de datos", details = ex.Message });
        }
    }
    [HttpPut("{SujetoId}/Patente/{PatenteId}")]
    public async Task<IActionResult> UpdatePatente(int SujetoId, int PatenteId, [FromBody] PatenteRequest request)
    {
        var sujeto = await _context.Sujetos
            .Include(s => s.Patentes)
            .FirstOrDefaultAsync(s => s.Id == SujetoId);

        if (sujeto == null)
            return NotFound(new { message = "Sujeto no encontrado" });

        // 1. Buscamos la referencia real del objeto dentro de la lista
        var patente = sujeto.Patentes.FirstOrDefault(p => p.Id == PatenteId);

        if (patente == null)
            return NotFound(new { message = "La patente a actualizar no existe para este sujeto" });

        // 2. Validación de duplicados 
        // Ojo: Debemos excluir la patente que estamos editando actualmente de esta búsqueda
        bool yaExiste = sujeto.Patentes.Any(p =>
            p.Id != PatenteId && // Que no sea la misma que estamos editando
            p.PatenteCamion == request.PatenteCamion &&
            p.PatenteAcoplado == request.PatenteAcoplado);

        if (yaExiste)
            return BadRequest(new { message = "Otra entrada ya tiene esta combinación de patentes." });

        // 3. Actualizamos las propiedades sobre el objeto rastreado
        patente.PatenteCamion = request.PatenteCamion;
        patente.PatenteAcoplado = request.PatenteAcoplado;
        patente.PatenteOpcional = request.PatenteOpcional;

        try
        {
            // EF detecta los cambios automáticamente porque 'patente' viene de 'sujeto.Patentes'
            await _context.SaveChangesAsync();
            return Ok(new
            {
                message = "Patente actualizada correctamente",
                data = new
                {
                    patente.Id,
                    patente.PatenteCamion,
                    patente.PatenteAcoplado,
                    patente.PatenteOpcional
                }
            });
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new { message = "Error en la base de datos", details = ex.InnerException?.Message ?? ex.Message });
        }
    }
    [HttpDelete("{SujetoId}/Patente")]
    public async Task<IActionResult> DelPatente(int SujetoId, [FromBody] int id)
    {
        var sujeto = await _context.Sujetos.
        Include(s => s.TiposSujeto).
        Include(p => p.Patentes).
        FirstOrDefaultAsync(s => s.Id == SujetoId);
        if (sujeto == null) return NotFound(new { message = "Sujeto no encontrado" });

        var patente = _context.Patentes.FirstOrDefault(p => p.Id == id && p.SujetoId == SujetoId);

        if (patente == null) return NotFound(new { message = "Patente no encontrada" });

        sujeto.Patentes.Remove(patente);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Patente borrada correctamente" });
    }
    [HttpPost("{SujetoId}/Chofer")]
    public async Task<IActionResult> AddChofer(int SujetoId, [FromBody] int ChoferId)
    {
        var sujeto = await _context.Sujetos
        .Include(s => s.Choferes)
        .FirstOrDefaultAsync(s => s.Id == SujetoId);
        if (sujeto == null) return NotFound(new { message = "Sujeto no encontrado" });

        if (sujeto.Choferes.Any(p => p.SujetoId == ChoferId))
            return BadRequest(new { message = "El sujeto ya tiene asignado este chofer" });

        var chofer = await _context.Sujetos.FindAsync(ChoferId);
        if (chofer == null) return NotFound(new { message = "Chofer no encontrado" });

        var newChofer = new Chofer
        {
            SujetoId = ChoferId,
            TransportistaId = SujetoId
        };

        sujeto.Choferes.Add(newChofer);
        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { message = "Chofer agregado correctamente", data = new { newChofer.SujetoId, newChofer.TransportistaId ,Transportista = newChofer.Transportista } });
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new { message = "Error al guardar en la base de datos", details = ex.Message });
        }
    }
  [HttpDelete("{SujetoId}/Chofer")]
public async Task<IActionResult> DelChofer(int SujetoId, [FromBody] int id)
    {
        var sujeto = await _context.Sujetos.
        Include(s => s.TiposSujeto).
        Include(p => p.Choferes).
        FirstOrDefaultAsync(s => s.Id == SujetoId);
        if (sujeto == null) return NotFound(new { message = "Sujeto no encontrado" });

        var chofer = _context.Choferes.FirstOrDefault(c => c.Id == id && c.SujetoId == SujetoId);

        if (chofer == null) return NotFound(new { message = "Chofer no encontrado" });

        sujeto.Choferes.Remove(chofer);
        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { message = "Chofer borrado correctamente" });
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new { message = "Error al eliminar el chofer", details = ex.Message });
        }
    }
}



// DTOs para las solicitudes
public record SujetoCreateRequest(string Nombre, long NumeroDocumento, string Email, string Telefono, string ExternoId);
public record SujetoUpdateRequest(int Id, string Nombre, int NumeroDocumento, string Email, string Telefono, string ExternoId);
public record PatenteRequest(string PatenteCamion, string PatenteAcoplado, string PatenteOpcional);