using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soltec.axControl.Server.Data;
using Soltec.axControl.Server.Model;

namespace Soltec.axControl.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SectoresController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SectoresController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/sectores
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var sectores = await _context.Sectores
            .OrderBy(s => s.Nombre)
            .ToListAsync();
        
        return Ok(sectores);
    }

    // GET: api/sectores/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var sector = await _context.Sectores
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sector == null)
        {
            return NotFound(new { message = "Sector no encontrado" });
        }

        return Ok(sector);
    }

    // POST: api/sectores
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SectorCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return BadRequest(new { message = "El nombre es requerido" });
        }

        var existeId = await _context.Sectores.AnyAsync(s => s.Id == request.Id);
        if (existeId)
        {
            return BadRequest(new { message = $"Ya existe un sector con el ID {request.Id}" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var sector = new Sector
        {
            Id = request.Id,
            Nombre = request.Nombre
        };

        _context.Sectores.Add(sector);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = sector.Id }, sector);
    }

    // PUT: api/sectores/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SectorUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nombre))
        {
            return BadRequest(new { message = "El nombre es requerido" });
        }

        var sector = await _context.Sectores.FindAsync(id);

        if (sector == null)
        {
            return NotFound(new { message = "Sector no encontrado" });
        }

        sector.Nombre = request.Nombre;

        await _context.SaveChangesAsync();

        return Ok(sector);
    }

    // DELETE: api/sectores/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var sector = await _context.Sectores
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sector == null)
        {
            return NotFound(new { message = "Sector no encontrado" });
        }

        _context.Sectores.Remove(sector);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Sector eliminado correctamente" });
    }
}

// DTOs para las solicitudes
public record SectorCreateRequest(int Id, string Nombre);
public record SectorUpdateRequest(int Id, string Nombre);