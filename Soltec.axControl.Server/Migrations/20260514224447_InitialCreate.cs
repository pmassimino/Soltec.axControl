using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Soltec.axControl.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cereales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cereales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Circuitos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Circuitos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Logo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sectores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sujetos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    NumeroDocumento = table.Column<long>(type: "bigint", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ExternoId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sujetos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposSujeto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposSujeto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zonas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zonas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransicionesEstado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CircuitoId = table.Column<int>(type: "int", nullable: false),
                    EstadoOrigenId = table.Column<int>(type: "int", nullable: false),
                    EstadoDestinoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransicionesEstado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransicionesEstado_Circuitos_CircuitoId",
                        column: x => x.CircuitoId,
                        principalTable: "Circuitos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TransicionesEstado_Estados_EstadoDestinoId",
                        column: x => x.EstadoDestinoId,
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransicionesEstado_Estados_EstadoOrigenId",
                        column: x => x.EstadoOrigenId,
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Choferes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransportistaId = table.Column<int>(type: "int", nullable: false),
                    SujetoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Choferes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Choferes_Sujetos_SujetoId",
                        column: x => x.SujetoId,
                        principalTable: "Sujetos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Choferes_Sujetos_TransportistaId",
                        column: x => x.TransportistaId,
                        principalTable: "Sujetos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Patentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SujetoId = table.Column<int>(type: "int", nullable: false),
                    PatenteCamion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PatenteAcoplado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PatenteOpcional = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patentes_Sujetos_SujetoId",
                        column: x => x.SujetoId,
                        principalTable: "Sujetos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SujetoTiposSujeto",
                columns: table => new
                {
                    SujetosId = table.Column<int>(type: "int", nullable: false),
                    TiposSujetoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SujetoTiposSujeto", x => new { x.SujetosId, x.TiposSujetoId });
                    table.ForeignKey(
                        name: "FK_SujetoTiposSujeto_Sujetos_SujetosId",
                        column: x => x.SujetosId,
                        principalTable: "Sujetos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SujetoTiposSujeto_TiposSujeto_TiposSujetoId",
                        column: x => x.TiposSujetoId,
                        principalTable: "TiposSujeto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Filas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Capacidad = table.Column<int>(type: "int", nullable: false),
                    TipoOperacion = table.Column<int>(type: "int", nullable: false),
                    ZonaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Filas_Zonas_ZonaId",
                        column: x => x.ZonaId,
                        principalTable: "Zonas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosRoles",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosRoles", x => new { x.UsuarioId, x.RolId });
                    table.ForeignKey(
                        name: "FK_UsuariosRoles_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuariosRoles_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FilaCereales",
                columns: table => new
                {
                    CerealesId = table.Column<int>(type: "int", nullable: false),
                    FilasId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilaCereales", x => new { x.CerealesId, x.FilasId });
                    table.ForeignKey(
                        name: "FK_FilaCereales_Cereales_CerealesId",
                        column: x => x.CerealesId,
                        principalTable: "Cereales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilaCereales_Filas_FilasId",
                        column: x => x.FilasId,
                        principalTable: "Filas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesTransito",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CerealId = table.Column<int>(type: "int", nullable: false),
                    CircuitoId = table.Column<int>(type: "int", nullable: false),
                    TransporteId = table.Column<int>(type: "int", nullable: false),
                    ChoferId = table.Column<int>(type: "int", nullable: false),
                    PatenteChasis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatenteAcoplado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatenteOpc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroComprobante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TagRFID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoId = table.Column<int>(type: "int", nullable: false),
                    FilaId = table.Column<int>(type: "int", nullable: false),
                    SectorId = table.Column<int>(type: "int", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesTransito", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrdenesTransito_Cereales_CerealId",
                        column: x => x.CerealId,
                        principalTable: "Cereales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTransito_Circuitos_CircuitoId",
                        column: x => x.CircuitoId,
                        principalTable: "Circuitos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTransito_Estados_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTransito_Filas_FilaId",
                        column: x => x.FilaId,
                        principalTable: "Filas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesTransito_Sectores_SectorId",
                        column: x => x.SectorId,
                        principalTable: "Sectores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistorialesComprobante",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenTransitoId = table.Column<int>(type: "int", nullable: false),
                    Numero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesComprobante", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialesComprobante_OrdenesTransito_OrdenTransitoId",
                        column: x => x.OrdenTransitoId,
                        principalTable: "OrdenesTransito",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistorialesEstado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenTransitoId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstadoId = table.Column<int>(type: "int", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesEstado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialesEstado_Estados_EstadoId",
                        column: x => x.EstadoId,
                        principalTable: "Estados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialesEstado_OrdenesTransito_OrdenTransitoId",
                        column: x => x.OrdenTransitoId,
                        principalTable: "OrdenesTransito",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistorialesFila",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenTransitoId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FilaId = table.Column<int>(type: "int", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesFila", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialesFila_Filas_FilaId",
                        column: x => x.FilaId,
                        principalTable: "Filas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialesFila_OrdenesTransito_OrdenTransitoId",
                        column: x => x.OrdenTransitoId,
                        principalTable: "OrdenesTransito",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistorialesSector",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenTransitoId = table.Column<int>(type: "int", nullable: false),
                    SectorId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesSector", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialesSector_OrdenesTransito_OrdenTransitoId",
                        column: x => x.OrdenTransitoId,
                        principalTable: "OrdenesTransito",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistorialesSector_Sectores_SectorId",
                        column: x => x.SectorId,
                        principalTable: "Sectores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Choferes_SujetoId",
                table: "Choferes",
                column: "SujetoId");

            migrationBuilder.CreateIndex(
                name: "IX_Choferes_TransportistaId",
                table: "Choferes",
                column: "TransportistaId");

            migrationBuilder.CreateIndex(
                name: "IX_FilaCereales_FilasId",
                table: "FilaCereales",
                column: "FilasId");

            migrationBuilder.CreateIndex(
                name: "IX_Filas_ZonaId",
                table: "Filas",
                column: "ZonaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesComprobante_OrdenTransitoId",
                table: "HistorialesComprobante",
                column: "OrdenTransitoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesEstado_EstadoId",
                table: "HistorialesEstado",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesEstado_OrdenTransitoId",
                table: "HistorialesEstado",
                column: "OrdenTransitoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesFila_FilaId",
                table: "HistorialesFila",
                column: "FilaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesFila_OrdenTransitoId",
                table: "HistorialesFila",
                column: "OrdenTransitoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesSector_OrdenTransitoId",
                table: "HistorialesSector",
                column: "OrdenTransitoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesSector_SectorId",
                table: "HistorialesSector",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTransito_CerealId",
                table: "OrdenesTransito",
                column: "CerealId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTransito_CircuitoId",
                table: "OrdenesTransito",
                column: "CircuitoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTransito_EstadoId",
                table: "OrdenesTransito",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTransito_FilaId",
                table: "OrdenesTransito",
                column: "FilaId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTransito_SectorId",
                table: "OrdenesTransito",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Patentes_SujetoId",
                table: "Patentes",
                column: "SujetoId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_UsuarioId",
                table: "Roles",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SujetoTiposSujeto_TiposSujetoId",
                table: "SujetoTiposSujeto",
                column: "TiposSujetoId");

            migrationBuilder.CreateIndex(
                name: "IX_TransicionesEstado_CircuitoId",
                table: "TransicionesEstado",
                column: "CircuitoId");

            migrationBuilder.CreateIndex(
                name: "IX_TransicionesEstado_EstadoDestinoId",
                table: "TransicionesEstado",
                column: "EstadoDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_TransicionesEstado_EstadoOrigenId",
                table: "TransicionesEstado",
                column: "EstadoOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosRoles_RolId",
                table: "UsuariosRoles",
                column: "RolId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Choferes");

            migrationBuilder.DropTable(
                name: "FilaCereales");

            migrationBuilder.DropTable(
                name: "HistorialesComprobante");

            migrationBuilder.DropTable(
                name: "HistorialesEstado");

            migrationBuilder.DropTable(
                name: "HistorialesFila");

            migrationBuilder.DropTable(
                name: "HistorialesSector");

            migrationBuilder.DropTable(
                name: "Patentes");

            migrationBuilder.DropTable(
                name: "SujetoTiposSujeto");

            migrationBuilder.DropTable(
                name: "TransicionesEstado");

            migrationBuilder.DropTable(
                name: "UsuariosRoles");

            migrationBuilder.DropTable(
                name: "OrdenesTransito");

            migrationBuilder.DropTable(
                name: "Sujetos");

            migrationBuilder.DropTable(
                name: "TiposSujeto");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Cereales");

            migrationBuilder.DropTable(
                name: "Circuitos");

            migrationBuilder.DropTable(
                name: "Estados");

            migrationBuilder.DropTable(
                name: "Filas");

            migrationBuilder.DropTable(
                name: "Sectores");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Zonas");
        }
    }
}
