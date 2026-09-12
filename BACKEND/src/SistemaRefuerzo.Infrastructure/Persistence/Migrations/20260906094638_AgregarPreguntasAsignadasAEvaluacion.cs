using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaRefuerzo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPreguntasAsignadasAEvaluacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreguntasAsignadas",
                table: "Evaluaciones",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreguntasAsignadas",
                table: "Evaluaciones");
        }
    }
}
