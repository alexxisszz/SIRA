using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaRefuerzo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSubtemaYTemaAIntentoEjercicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subtema",
                table: "IntentosEjercicio",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TemaId",
                table: "IntentosEjercicio",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_IntentosEjercicio_AlumnoId_Subtema",
                table: "IntentosEjercicio",
                columns: new[] { "AlumnoId", "Subtema" });

            migrationBuilder.CreateIndex(
                name: "IX_IntentosEjercicio_AlumnoId_TemaId",
                table: "IntentosEjercicio",
                columns: new[] { "AlumnoId", "TemaId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IntentosEjercicio_AlumnoId_Subtema",
                table: "IntentosEjercicio");

            migrationBuilder.DropIndex(
                name: "IX_IntentosEjercicio_AlumnoId_TemaId",
                table: "IntentosEjercicio");

            migrationBuilder.DropColumn(
                name: "Subtema",
                table: "IntentosEjercicio");

            migrationBuilder.DropColumn(
                name: "TemaId",
                table: "IntentosEjercicio");
        }
    }
}
