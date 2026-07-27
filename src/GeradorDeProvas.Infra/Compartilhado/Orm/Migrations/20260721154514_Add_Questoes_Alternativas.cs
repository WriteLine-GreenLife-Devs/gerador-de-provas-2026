using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeradorDeProvas.Infra.Compartilhado.Orm.Migrations
{
    /// <inheritdoc />
    public partial class Add_Questoes_Alternativas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TBQuestao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Enunciado = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    MateriaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBQuestao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBQuestao_TBMateria",
                        column: x => x.MateriaId,
                        principalTable: "TBMateria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TBAlternativa",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Texto = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Correta = table.Column<bool>(type: "bit", nullable: false),
                    QuestaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBAlternativa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TBAlternativa_TBQuestao",
                        column: x => x.QuestaoId,
                        principalTable: "TBQuestao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TBAlternativa_QuestaoId",
                table: "TBAlternativa",
                column: "QuestaoId");

            migrationBuilder.CreateIndex(
                name: "IX_TBQuestao_MateriaId",
                table: "TBQuestao",
                column: "MateriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBAlternativa");

            migrationBuilder.DropTable(
                name: "TBQuestao");
        }
    }
}
