using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CriadorDeCaes.Data.Migrations
{
    /// <inheritdoc />
    public partial class PrecoCompra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PrecoCompra",
                table: "Animais",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrecoCompra",
                table: "Animais");
        }
    }
}
