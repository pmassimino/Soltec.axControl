using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Soltec.axControl.Server.Model;

public class Usuario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    [MaxLength(60)]
    [Required]
    public string Nombre { get; set; }
    [MaxLength(60)]
    [Required]
    public string PasswordHash { get; set; } // Aquí guardas TODO (Algoritmo + Salt + Hash)    

    public string Email { get; set; }    
    
    public ICollection<UsuarioRol> Roles { get; set; }   

}
public class Rol
{
    [Key]    
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    [MaxLength(60)]
    [Required]
    public string Nombre { get; set; }
    // Agrega esto para completar la relación
    public ICollection<UsuarioRol> UsuarioRoles { get; set; }
}
public class UsuarioRol
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; }
    public int RolId { get; set; }
    public Rol Rol { get; set; }
}
public class Sector
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    [MaxLength(60)]
    [Required]
    public string Nombre { get; set; }
}

public class Zona
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    [MaxLength(60)]
    [Required]
    public string Nombre { get; set; } // Ej: "Entrada", "Pre-Descarga", "Descarga", "Salida"
    public string Descripcion { get; set; }
    public int Orden { get; set; } // Para ordenar visualmente
    public ICollection<Fila> Filas { get; set; }
}

public class Estado
{
    [Key]    
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    [MaxLength(60)]
    [Required]
    public string Nombre { get; set; }
    public string Logo { get; set; } // Puede ser una URL o Base64
}

public class Circuito
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    [MaxLength(60)]
    [Required]
    public string Nombre { get; set; }
    public ICollection<TransicionEstado> Transiciones { get; set; }
}

public class TransicionEstado
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int CircuitoId { get; set; }
    public Circuito Circuito { get; set; }

    public int EstadoOrigenId { get; set; }
    public Estado EstadoOrigen { get; set; }

    public int EstadoDestinoId { get; set; }
    public Estado EstadoDestino { get; set; }
}

public class Cereal
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    [MaxLength(60)]
    [Required]
    public string Nombre { get; set; }
    
    public ICollection<Fila> Filas { get; set; }
}

public class TipoSujeto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    [MaxLength(60)]
    [Required]
    public string Nombre { get; set; } // Ej: "Chofer", "Transportista"
    public ICollection<Sujeto> Sujetos { get; set; }
}

public class Sujeto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [MaxLength(60)]
    [Required]
    public string Nombre { get; set; }
    public long NumeroDocumento { get; set; }
    [MaxLength(200)]
    public string Email { get; set; }
    [MaxLength(60)]
    public string Telefono { get; set; }
    [MaxLength(20)]
    public string ExternoId { get; set; } // Para vincular con sistemas externos (ej: ERP)
    // Relación n a n: un sujeto puede tener múltiples tipos (Chofer, Transportista, o ambos)
    public ICollection<TipoSujeto> TiposSujeto { get; set; }  
    public ICollection<Patente> Patentes { get; set; }
    [InverseProperty("Transportista")]
    public ICollection<Chofer> Choferes { get; set; }
   
}

public class Chofer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int TransportistaId { get; set; }
    [ForeignKey("TransportistaId")]
    public Sujeto Transportista { get; set; }

    public int SujetoId { get; set; }
    [ForeignKey("SujetoId")]
    public Sujeto Sujeto { get; set; }
   
}

public class Patente
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int SujetoId { get; set; }
    public Sujeto Sujeto { get; set; }

    [MaxLength(20)]
    [Required]
    public string PatenteCamion { get; set; }

    [MaxLength(20)]
    public string PatenteAcoplado { get; set; }

    [MaxLength(20)]
    public string PatenteOpcional { get; set; }
}

public enum TipoOperacion
{
    Descarga,
    Carga,
    Otros
}

public class Fila
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [MaxLength(60)]
    [Required]
    public string Nombre { get; set; }
    public int Capacidad { get; set; }
    public TipoOperacion TipoOperacion { get; set; }
    public ICollection<Cereal> Cereales { get; set; }
    public int ZonaId { get; set; }
    public Zona Zona { get; set; }
}

public class OrdenTransito
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public int CerealId { get; set; }
    public int CircuitoId { get; set; }
    public Circuito Circuito { get; set; }

    // Datos de Transporte
    public int TransporteId { get; set; }
    public int ChoferId { get; set; }
    public string PatenteChasis { get; set; }
    public string PatenteAcoplado { get; set; }
    public string PatenteOpc { get; set; }
    public string NumeroComprobante { get; set; }
    public string TagRFID { get; set; }

    // Estado Actual
    public int EstadoId { get; set; }
    public Estado Estado { get; set; }

    public Cereal Cereal { get; set; }
    // Fila Actual
    public int FilaId { get; set; }
    public Fila Fila { get; set; }
    public int SectorId { get; set; }
    public Sector Sector { get; set; }
    public string Observacion { get; set; }

    // Relaciones de Historial
    public List<EstadoHistorial> HistorialEstados { get; set; } = new();
    public List<SectorHistorial> HistorialSectores { get; set; } = new();
    public List<FilaHistorial> HistorialFilas { get; set; } = new();
    public List<ComprobanteHistorial> HistorialComprobantes { get; set; } = new();
}

public class EstadoHistorial
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int OrdenTransitoId { get; set; }
    public DateTime Fecha { get; set; }
    public int EstadoId { get; set; }
    public Estado Estado { get; set; }
    public string Observacion { get; set; }
    public OrdenTransito OrdenTransito { get; set; }
}

public class FilaHistorial
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int OrdenTransitoId { get; set; }
    public DateTime Fecha { get; set; }
    public int FilaId { get; set; }
    public Fila Fila { get; set; }
    public string Observacion { get; set; }
    public OrdenTransito OrdenTransito { get; set; }
}

public class ComprobanteHistorial
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int OrdenTransitoId { get; set; }
    public string Numero { get; set; }
    public DateTime Fecha { get; set; }
    public string Observacion { get; set; }
    public OrdenTransito OrdenTransito { get; set; }
}
public class SectorHistorial
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int OrdenTransitoId { get; set; }
    public int SectorId { get; set; }
    public DateTime Fecha { get; set; }
    public string Observacion { get; set; }
    public OrdenTransito OrdenTransito { get; set; }
    public Sector Sector { get; set; }
}