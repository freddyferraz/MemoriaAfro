using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemoriaAfro.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaUrlImagemPonto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlImagem",
                table: "PontosInteresse",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlImagem",
                table: "PontosInteresse");
        }
    }
}
