using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaRefuerzo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AgregarVideosApoyo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VideosApoyo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TemaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideosApoyo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VideosApoyo_TemaId",
                table: "VideosApoyo",
                column: "TemaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VideosApoyo");
        }
    }
}
