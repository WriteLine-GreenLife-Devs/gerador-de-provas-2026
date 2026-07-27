using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeradorDeProvas.Infra.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class Add_Materias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBMateria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Serie = table.Column<int>(type: "int", nullable: false),
                    DisciplinaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBMateria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBMateria_TBDisciplina",
                        column: x => x.DisciplinaId,
                        principalTable: "TBDisciplina",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBMateria_DisciplinaId",
                table: "TBMateria",
                column: "DisciplinaId");

            migrationBuilder.CreateIndex(
                name: "UQ_TBMateria_UserId_Nome",
                table: "TBMateria",
                columns: new[] { "UserId", "Nome" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBMateria");
        }
    }
}
