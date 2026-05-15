using Microsoft.EntityFrameworkCore;
using Soltec.axControl.Server.Model;

namespace Soltec.axControl.Server.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<UsuarioRol> UsuariosRoles { get; set; }
    public DbSet<Sector> Sectores { get; set; }
    public DbSet<Zona> Zonas { get; set; }
    public DbSet<Estado> Estados { get; set; }
    public DbSet<Circuito> Circuitos { get; set; }
    public DbSet<TransicionEstado> TransicionesEstado { get; set; }
    public DbSet<Cereal> Cereales { get; set; }
    public DbSet<TipoSujeto> TiposSujeto { get; set; }
    public DbSet<Sujeto> Sujetos { get; set; }
    public DbSet<Chofer> Choferes { get; set; }
    public DbSet<Patente> Patentes { get; set; }
    public DbSet<Fila> Filas { get; set; }
    public DbSet<OrdenTransito> OrdenesTransito { get; set; }
    public DbSet<EstadoHistorial> HistorialesEstado { get; set; }
    public DbSet<SectorHistorial> HistorialesSector { get; set; }
    public DbSet<FilaHistorial> HistorialesFila { get; set; }
    public DbSet<ComprobanteHistorial> HistorialesComprobante { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Las configuraciones de IDs autogenerados ahora están en las anotaciones del modelo
        // Solo configuramos las relaciones y restricciones
        // 1. Definir clave primaria compuesta para la tabla intermedia
        modelBuilder.Entity<UsuarioRol>()
            .HasKey(ur => new { ur.UsuarioId, ur.RolId });

        // 2. Configurar relación Usuario -> UsuarioRol
        modelBuilder.Entity<UsuarioRol>()
            .HasOne(ur => ur.Usuario)
            .WithMany(u => u.UsuarioRoles)
            .HasForeignKey(ur => ur.UsuarioId);

        // 3. Configurar relación Rol -> UsuarioRol
        modelBuilder.Entity<UsuarioRol>()
            .HasOne(ur => ur.Rol)
            .WithMany(r => r.UsuarioRoles)
            .HasForeignKey(ur => ur.RolId);


        // Zona -> Fila (1 a n)
        modelBuilder.Entity<Zona>()
            .HasMany(z => z.Filas)
            .WithOne(f => f.Zona)
            .HasForeignKey(f => f.ZonaId);

        // Circuito -> TransicionEstado (1 a n)
        modelBuilder.Entity<Circuito>()
            .HasMany(c => c.Transiciones)
            .WithOne(t => t.Circuito)
            .HasForeignKey(t => t.CircuitoId);

        // TransicionEstado -> Estado Origen (n a 1)
        modelBuilder.Entity<TransicionEstado>()
            .HasOne(t => t.EstadoOrigen)
            .WithMany()
            .HasForeignKey(t => t.EstadoOrigenId)
            .OnDelete(DeleteBehavior.Restrict);

        // TransicionEstado -> Estado Destino (n a 1)
        modelBuilder.Entity<TransicionEstado>()
            .HasOne(t => t.EstadoDestino)
            .WithMany()
            .HasForeignKey(t => t.EstadoDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Fila -> Cereal (n a n)
        modelBuilder.Entity<Fila>()
            .HasMany(f => f.Cereales)
            .WithMany(c => c.Filas)
            .UsingEntity(j => j.ToTable("FilaCereales"));

        // OrdenTransito -> Cereal (n a 1)
        modelBuilder.Entity<OrdenTransito>()
            .HasOne(o => o.Cereal)
            .WithMany()
            .HasForeignKey(o => o.CerealId)
            .OnDelete(DeleteBehavior.Restrict);

        // OrdenTransito -> Circuito (n a 1)
        modelBuilder.Entity<OrdenTransito>()
            .HasOne(o => o.Circuito)
            .WithMany()
            .HasForeignKey(o => o.CircuitoId)
            .OnDelete(DeleteBehavior.Restrict);

        // OrdenTransito -> Estado (n a 1)
        modelBuilder.Entity<OrdenTransito>()
            .HasOne(o => o.Estado)
            .WithMany()
            .HasForeignKey(o => o.EstadoId)
            .OnDelete(DeleteBehavior.Restrict);

        // OrdenTransito -> Fila (n a 1)
        modelBuilder.Entity<OrdenTransito>()
            .HasOne(o => o.Fila)
            .WithMany()
            .HasForeignKey(o => o.FilaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Sujeto -> TipoSujeto (n a n)
        modelBuilder.Entity<Sujeto>()
            .HasMany(s => s.TiposSujeto)
            .WithMany(t => t.Sujetos)
            .UsingEntity(j => j.ToTable("SujetoTiposSujeto"));

        modelBuilder.Entity<Chofer>()
        .HasOne(c => c.Sujeto)
        .WithMany()
        .HasForeignKey(c => c.SujetoId)
        .OnDelete(DeleteBehavior.Restrict); // Desactiva el borrado en cascada

        modelBuilder.Entity<Chofer>()
            .HasOne(c => c.Transportista)
            .WithMany(s => s.Choferes) // Esta es la colección que sí mantuvimos
            .HasForeignKey(c => c.TransportistaId)
            .OnDelete(DeleteBehavior.Restrict); // Desactiva el borrado en cascada

        modelBuilder.Entity<Patente>()
            .HasOne(p => p.Sujeto)
            .WithMany(c => c.Patentes)
            .HasForeignKey(p => p.SujetoId)
            .OnDelete(DeleteBehavior.Cascade);

        // EstadoHistorial -> OrdenTransito (n a 1)
        modelBuilder.Entity<EstadoHistorial>()
            .HasOne(e => e.OrdenTransito)
            .WithMany(o => o.HistorialEstados)
            .HasForeignKey(e => e.OrdenTransitoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SectorHistorial>()
        .HasOne(e => e.OrdenTransito)
        .WithMany(o => o.HistorialSectores)
        .HasForeignKey(e => e.OrdenTransitoId)
        .OnDelete(DeleteBehavior.Cascade);

        // EstadoHistorial -> Estado (n a 1)
        modelBuilder.Entity<EstadoHistorial>()
            .HasOne(e => e.Estado)
            .WithMany()
            .HasForeignKey(e => e.EstadoId)
            .OnDelete(DeleteBehavior.Restrict);

        // EstadoHistorial -> Estado (n a 1)
        modelBuilder.Entity<SectorHistorial>()
            .HasOne(e => e.Sector)
            .WithMany()
            .HasForeignKey(e => e.SectorId)
            .OnDelete(DeleteBehavior.Restrict);

        // FilaHistorial -> OrdenTransito (n a 1)
        modelBuilder.Entity<FilaHistorial>()
            .HasOne(f => f.OrdenTransito)
            .WithMany(o => o.HistorialFilas)
            .HasForeignKey(f => f.OrdenTransitoId)
            .OnDelete(DeleteBehavior.Cascade);

        // FilaHistorial -> Fila (n a 1)
        modelBuilder.Entity<FilaHistorial>()
            .HasOne(f => f.Fila)
            .WithMany()
            .HasForeignKey(f => f.FilaId)
            .OnDelete(DeleteBehavior.Restrict);

        // ComprobanteHistorial -> OrdenTransito (n a 1)
        modelBuilder.Entity<ComprobanteHistorial>()
            .HasOne(c => c.OrdenTransito)
            .WithMany(o => o.HistorialComprobantes)
            .HasForeignKey(c => c.OrdenTransitoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}