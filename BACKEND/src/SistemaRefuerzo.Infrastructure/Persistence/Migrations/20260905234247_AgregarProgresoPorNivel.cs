using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaRefuerzo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProgresoPorNivel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NivelEvaluado",
                table: "Evaluaciones",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Evaluaciones",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NivelEvaluado",
                table: "Evaluaciones");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Evaluaciones");
        }
    }
}
