using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Felhasznalok",
                columns: table => new {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                    ,
                    Jelszo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Engedelyek = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Felhasznalok", x => x.Id);
                }
            );
            migrationBuilder.CreateTable(
                name: "Macskak",
                columns: table => new {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                    ,
                    Nev = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Szin = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Kor = table.Column<byte>(type: "tinyint", nullable: false),
                    Ar = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Macskak", x => x.Id);
                }
            );
            migrationBuilder.CreateTable(
                name: "Rendelesek",
                columns: table => new {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                    ,
                    Felhasznalo = table.Column<int>(type: "int", nullable: false),
                    SzallitasiCim = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RendelesIdeje = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Rendelesek", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rendelesek_Felhasznalok_Felhasznalo",
                        column: x => x.Felhasznalo,
                        principalTable: "Felhasznalok",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );
            migrationBuilder.CreateTable(
                name: "RendeleshezTartozikok",
                columns: table => new {
                    Rendeles = table.Column<int>(type: "int", nullable: false),
                    Macska = table.Column<int>(type: "int", nullable: false),
                    Mennyiseg = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_RendeleshezTartozikok", x => new {
                        x.Rendeles,
                        x.Macska
                    });
                    table.ForeignKey(
                        name: "FK_RendeleshezTartozikok_Macskak_Macska",
                        column: x => x.Macska,
                        principalTable: "Macskak",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_RendeleshezTartozikok_Rendelesek_Rendeles",
                        column: x => x.Rendeles,
                        principalTable: "Rendelesek",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );
            migrationBuilder.CreateIndex(
                name: "IX_Felhasznalok_Email",
                table: "Felhasznalok",
                column: "Email",
                unique: true
            );
            migrationBuilder.CreateIndex(
                name: "IX_Rendelesek_Felhasznalo",
                table: "Rendelesek",
                column: "Felhasznalo"
            );
            migrationBuilder.CreateIndex(
                name: "IX_RendeleshezTartozikok_Macska",
                table: "RendeleshezTartozikok",
                column: "Macska"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RendeleshezTartozikok"
            );
            migrationBuilder.DropTable(
                name: "Macskak"
            );
            migrationBuilder.DropTable(
                name: "Rendelesek"
            );
            migrationBuilder.DropTable(
                name: "Felhasznalok"
            );
        }
    }
}
