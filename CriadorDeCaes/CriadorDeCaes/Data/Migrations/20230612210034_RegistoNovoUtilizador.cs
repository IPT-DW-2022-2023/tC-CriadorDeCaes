using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CriadorDeCaes.Data.Migrations
{
    /// <inheritdoc />
    public partial class RegistoNovoUtilizador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Criadores",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Criadores");
        }
    }
}
