using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaRefuerzo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarIntentosEjercicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IntentosEjercicio",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AlumnoId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreguntaId = table.Column<Guid>(type: "uuid", nullable: false),
                    OpcionSeleccionadaId = table.Column<Guid>(type: "uuid", nullable: false),
                    EsCorrecta = table.Column<bool>(type: "boolean", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IntentosEjercicio", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IntentosEjercicio_AlumnoId",
                table: "IntentosEjercicio",
                column: "AlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_IntentosEjercicio_PreguntaId",
                table: "IntentosEjercicio",
                column: "PreguntaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IntentosEjercicio");
        }
    }
}
