using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Agendamiento.Api.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Citas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteNombre = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    ClienteTelefono = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Servicio = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    FechaHora = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    Notas = table.Column<string>(type: "TEXT", nullable: true),
                    CreadaEn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 160, nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Rol = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "Nombre", "PasswordHash", "Rol" },
                values: new object[,]
                {
                    { 1, "admin@agendamiento.app", "Administrador Demo", "$2a$11$kSg0JBib7cgq.yS9QbDwf.iy5dEAA0WjyWYLjmEh.S9qm//nqIg4S", 0 },
                    { 2, "recepcion@agendamiento.app", "Recepción Demo", "$2a$11$bTxBxKMu8hdpf59zEjQMpuXy2WdFWVCPDDLRXhSo5bWSAIK8iRQ7q", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Citas_FechaHora",
                table: "Citas",
                column: "FechaHora");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Citas");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
