using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CriadorDeCaes.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Criadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NomeComercial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Morada = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodPostal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Telemovel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Criadores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Racas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Racas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Animais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sexo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataNasc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataCompra = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegistoLOP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RacaFK = table.Column<int>(type: "int", nullable: false),
                    CriadorFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Animais_Criadores_CriadorFK",
                        column: x => x.CriadorFK,
                        principalTable: "Criadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Animais_Racas_RacaFK",
                        column: x => x.RacaFK,
                        principalTable: "Racas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CriadoresRacas",
                columns: table => new
                {
                    ListaCriadoresId = table.Column<int>(type: "int", nullable: false),
                    ListaRacasId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriadoresRacas", x => new { x.ListaCriadoresId, x.ListaRacasId });
                    table.ForeignKey(
                        name: "FK_CriadoresRacas_Criadores_ListaCriadoresId",
                        column: x => x.ListaCriadoresId,
                        principalTable: "Criadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CriadoresRacas_Racas_ListaRacasId",
                        column: x => x.ListaRacasId,
                        principalTable: "Racas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fotografias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ficheiro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Local = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AnimalFK = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fotografias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fotografias_Animais_AnimalFK",
                        column: x => x.AnimalFK,
                        principalTable: "Animais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Animais_CriadorFK",
                table: "Animais",
                column: "CriadorFK");

            migrationBuilder.CreateIndex(
                name: "IX_Animais_RacaFK",
                table: "Animais",
                column: "RacaFK");

            migrationBuilder.CreateIndex(
                name: "IX_CriadoresRacas_ListaRacasId",
                table: "CriadoresRacas",
                column: "ListaRacasId");

            migrationBuilder.CreateIndex(
                name: "IX_Fotografias_AnimalFK",
                table: "Fotografias",
                column: "AnimalFK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CriadoresRacas");

            migrationBuilder.DropTable(
                name: "Fotografias");

            migrationBuilder.DropTable(
                name: "Animais");

            migrationBuilder.DropTable(
                name: "Criadores");

            migrationBuilder.DropTable(
                name: "Racas");
        }
    }
}
