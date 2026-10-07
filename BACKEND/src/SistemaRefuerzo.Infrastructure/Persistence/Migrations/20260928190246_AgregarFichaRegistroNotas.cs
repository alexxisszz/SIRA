using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaRefuerzo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFichaRegistroNotas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Indicador",
                table: "Preguntas",
                type: "character varying(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "I1");

            migrationBuilder.CreateTable(
                name: "FichasRegistroNotas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AlumnoId = table.Column<Guid>(type: "uuid", nullable: false),
                    TemaId = table.Column<Guid>(type: "uuid", nullable: false),
                    TipoEvaluacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    D1I1 = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    D1I2 = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    D1I3 = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    D2I1 = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    D2I2 = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    D2I3 = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    D3I1 = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    D3I2 = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    D3I3 = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FichasRegistroNotas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FichasRegistroNotas_AlumnoId_TemaId_TipoEvaluacion",
                table: "FichasRegistroNotas",
                columns: new[] { "AlumnoId", "TemaId", "TipoEvaluacion" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FichasRegistroNotas_TemaId_TipoEvaluacion",
                table: "FichasRegistroNotas",
                columns: new[] { "TemaId", "TipoEvaluacion" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FichasRegistroNotas");

            migrationBuilder.DropColumn(
                name: "Indicador",
                table: "Preguntas");
        }
    }
}
