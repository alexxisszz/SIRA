using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaRefuerzo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarMetadataPregunta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Explicacion",
                table: "Preguntas",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Puntaje",
                table: "Preguntas",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Subtema",
                table: "Preguntas",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Preguntas",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Preguntas_TemaId_Subtema",
                table: "Preguntas",
                columns: new[] { "TemaId", "Subtema" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Preguntas_TemaId_Subtema",
                table: "Preguntas");

            migrationBuilder.DropColumn(
                name: "Explicacion",
                table: "Preguntas");

            migrationBuilder.DropColumn(
                name: "Puntaje",
                table: "Preguntas");

            migrationBuilder.DropColumn(
                name: "Subtema",
                table: "Preguntas");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Preguntas");
        }
    }
}
