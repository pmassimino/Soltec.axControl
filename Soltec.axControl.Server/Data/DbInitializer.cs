using Microsoft.EntityFrameworkCore;
using Soltec.axControl.Server.Data;
using Soltec.axControl.Server.Model;
using Soltec.Security;

namespace Soltec.axControl.Server.Data;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Asegurar que la base de datos esté creada
        context.Database.EnsureCreated();      

        // Si ya hay datos, no sembrar
        if (!context.Cereales.Any())
        {   
        // Cereales
        var cerealeSoja = new Cereal { Id = 23, Nombre = "Soja" };
        var cerealTrigo = new Cereal { Id = 15, Nombre = "Trigo" };
        var cerealMaiz = new Cereal { Id = 19, Nombre = "Maiz" };
        var cerealGirasol = new Cereal { Id = 2, Nombre = "Girasol" };

        context.Cereales.AddRange(cerealeSoja, cerealTrigo, cerealMaiz, cerealGirasol);
        }
        if (!context.TiposSujeto.Any())
        {
        // Tipos de Sujeto
        var tipoTransportista = new TipoSujeto { Id = 1, Nombre = "Transportista" };
        var tipoChofer = new TipoSujeto { Id = 2, Nombre = "Chofer" };

        context.TiposSujeto.AddRange(tipoTransportista, tipoChofer);
        }
        if (!context.Roles.Any())
        {
        // Roles
        var rolAdministrador = new Rol { Id = 1, Nombre = "Administrador" };
        var rolUsuario = new Rol { Id = 2, Nombre = "Usuario" };
        context.Roles.AddRange(rolAdministrador, rolUsuario);
        }
        if (!context.Usuarios.Any())
        {
        // Usuarios - Solo para ejemplo, sin seguridad real
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("admin"); // Hash seguro con BCrypt          
        var admin = new Usuario { Id=1,Nombre = "admin", PasswordHash = passwordHash, Email = "admin@example.com" };
        passwordHash = BCrypt.Net.BCrypt.HashPassword("usuario"); 
        var usuario = new Usuario { Id=2, Nombre = "usuario", PasswordHash = passwordHash, Email = "usuario@example.com" };
        context.Usuarios.AddRange(admin, usuario);        
        }
        if (!context.UsuariosRoles.Any())
        {
        // UsuariosRoles        
        var adminRol = new UsuarioRol {  UsuarioId = 1, RolId = 1 }; // admin -> Administrador
        var usuarioRol = new UsuarioRol {  UsuarioId = 2, RolId = 2 }; // usuario -> Usuario
        context.UsuariosRoles.AddRange(adminRol, usuarioRol);
        }
        if (!context.Estados.Any())
        {
        // Estados        
        var estados = new List<Estado>
        {
            new Estado { Id = 1, Nombre = "Ingreso", Logo = "" },
            new Estado { Id = 2, Nombre = "Calado", Logo = "" },
            new Estado { Id = 3, Nombre = "Peso Tara", Logo = "" },
            new Estado { Id = 4, Nombre = "Peso Bruto", Logo = "" },
            new Estado { Id = 5, Nombre = "Pre Descarga", Logo = "" },
            new Estado { Id = 6, Nombre = "Pre Carga", Logo = "" },
            new Estado { Id = 7, Nombre = "Descarga", Logo = "" },
            new Estado { Id = 8, Nombre = "Carga", Logo = "" },
            new Estado { Id = 9, Nombre = "Pendiente Documentacion", Logo = "" },     
            new Estado { Id = 10, Nombre = "Finalizado", Logo = "" },
            new Estado { Id = 11, Nombre = "Cancelado", Logo = "" }
        };
        context.Estados.AddRange(estados);
        }
          if (!context.Sectores.Any())
        {
        // Sectores
        var sectores = new List<Sector>
        {
            new Sector { Id = 1, Nombre = "Ingreso" },
            new Sector { Id = 2, Nombre = "Calado" },
            new Sector { Id = 3, Nombre = "Balanza" },
            new Sector { Id = 4, Nombre = "Pre Carga/Descarga" },
            new Sector { Id = 5, Nombre = "Descarga" },
            new Sector { Id = 6, Nombre = "Semaforo" },
            new Sector { Id = 7, Nombre = "Salida" },

        };        
        context.Sectores.AddRange(sectores);
        }
        if (!context.Zonas.Any())
        {
        // Zonas
        var zonas = new List<Zona>
        {
            new Zona { Id = 1, Nombre = "Ingreso", Descripcion = "Zona de entrada de vehículos", Orden = 1 },
            new Zona { Id = 2, Nombre = "Pre-Carga/Descarga", Descripcion = "Zona de espera antes de carga o descarga", Orden = 2 }            
        };
        context.Zonas.AddRange(zonas);
        }
        if(!context.Circuitos.Any())
        {
        // Circuitos
        var circuitos = new List<Circuito>
         {new Circuito { Id = 1, Nombre = "Descarga"},
         new Circuito { Id = 2, Nombre = "Carga"},
         new Circuito { Id = 3, Nombre = "Acondiconamiento"},};

        context.Circuitos.AddRange(circuitos);  
        }
        if(!context.TransicionesEstado.Any())
        {
        // Transiciones de Estado para el circuito default
        //1-Ingreso, 2-Calado, 3-Peso Tara, 4-Peso Bruto, 5-Pre Descarga, 6-Pre Carga, 7-Descarga, 8-Carga,9-Documentacion,
        //10-Finalizado, 11-Cancelado
        var transiciones = new List<TransicionEstado>
        {// Circuito Descarga            
            new TransicionEstado { CircuitoId = 1, EstadoOrigenId = 1, EstadoDestinoId = 2 },
            new TransicionEstado { CircuitoId = 1, EstadoOrigenId = 2, EstadoDestinoId = 4},
            new TransicionEstado { CircuitoId = 1, EstadoOrigenId = 4, EstadoDestinoId = 5 },
            new TransicionEstado { CircuitoId = 1, EstadoOrigenId = 5, EstadoDestinoId = 7 },
            new TransicionEstado { CircuitoId = 1, EstadoOrigenId = 7, EstadoDestinoId = 10 },
            new TransicionEstado { CircuitoId = 1, EstadoOrigenId = 1, EstadoDestinoId = 11 },
            new TransicionEstado { CircuitoId = 1, EstadoOrigenId = 2, EstadoDestinoId = 11 },
            new TransicionEstado { CircuitoId = 1, EstadoOrigenId = 4, EstadoDestinoId = 11 },
            new TransicionEstado { CircuitoId = 1, EstadoOrigenId = 5, EstadoDestinoId = 11 },
            new TransicionEstado { CircuitoId = 1, EstadoOrigenId = 7, EstadoDestinoId = 11 },
            // Circuito Carga            
            new TransicionEstado { CircuitoId = 2, EstadoOrigenId = 1, EstadoDestinoId = 3 },
            new TransicionEstado { CircuitoId = 2, EstadoOrigenId = 3, EstadoDestinoId = 6 },
            new TransicionEstado { CircuitoId = 2, EstadoOrigenId = 6, EstadoDestinoId = 8 },
            new TransicionEstado { CircuitoId = 2, EstadoOrigenId = 8, EstadoDestinoId = 9 },
            new TransicionEstado { CircuitoId = 2, EstadoOrigenId = 9, EstadoDestinoId = 10 },
            new TransicionEstado { CircuitoId = 2, EstadoOrigenId = 1, EstadoDestinoId = 11 },
            new TransicionEstado { CircuitoId = 2, EstadoOrigenId = 3, EstadoDestinoId = 11 },
            new TransicionEstado { CircuitoId = 2, EstadoOrigenId = 6, EstadoDestinoId = 11},
            new TransicionEstado { CircuitoId = 2, EstadoOrigenId = 8, EstadoDestinoId = 11 },
            new TransicionEstado { CircuitoId = 2, EstadoOrigenId = 9, EstadoDestinoId = 11 },
            // Circuito Acondiconamiento    
            //1-Ingreso, 2-Calado, 4-Peso Bruto, 5-Pre Descarga, 7-Descarga,3-Peso Tara,6-Pre Carga,8-Carga,
            // 9-Documentacion,//10-Finalizado, 11-Cancelado      
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 1, EstadoDestinoId = 2 },
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 2, EstadoDestinoId = 4 },
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 4, EstadoDestinoId = 5 },
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 5, EstadoDestinoId = 7 },
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 7, EstadoDestinoId = 3 },
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 3, EstadoDestinoId = 6 },
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 6, EstadoDestinoId = 8 },
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 8, EstadoDestinoId = 9},
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 9, EstadoDestinoId = 10 },
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 1, EstadoDestinoId = 11 },
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 2, EstadoDestinoId = 11 },
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 4, EstadoDestinoId = 11 },
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 5, EstadoDestinoId = 11 },
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 7, EstadoDestinoId = 11 },
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 3, EstadoDestinoId = 11 },
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 6, EstadoDestinoId = 11 },
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 8, EstadoDestinoId = 11 },
            new TransicionEstado { CircuitoId = 3, EstadoOrigenId = 9, EstadoDestinoId = 11 },            
        };
        context.TransicionesEstado.AddRange(transiciones);
        }
        context.SaveChanges();
    }
}