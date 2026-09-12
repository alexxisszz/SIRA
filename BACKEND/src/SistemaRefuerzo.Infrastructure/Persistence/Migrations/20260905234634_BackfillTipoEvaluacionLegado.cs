using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaRefuerzo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BackfillTipoEvaluacionLegado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"Evaluaciones\" SET \"Tipo\" = 'Diagnostica' WHERE \"Tipo\" = ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
