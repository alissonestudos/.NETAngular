using Microsoft.EntityFrameworkCore.Migrations;

namespace IntroducaoEFC.Migrations
{
    public partial class testeesquecido : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Esquecida",
                table: "Clientes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Esquecida",
                table: "Clientes");
        }
    }
}
