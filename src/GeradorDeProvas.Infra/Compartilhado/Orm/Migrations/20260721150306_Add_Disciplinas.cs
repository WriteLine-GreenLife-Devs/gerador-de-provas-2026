using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeradorDeProvas.Infra.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class Add_Disciplinas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBDisciplina",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBDisciplina", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "UQ_TBDisciplina_UserId_Nome",
                table: "TBDisciplina",
                columns: new[] { "UserId", "Nome" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBDisciplina");
        }
    }
}
