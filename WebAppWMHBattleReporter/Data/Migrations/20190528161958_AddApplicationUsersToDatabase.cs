using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppWMHBattleReporter.Data.Migrations
{
    public partial class AddApplicationUsersToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfGamesLost",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfGamesPlayed",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfGamesWon",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Winrate",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NumberOfGamesLost",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NumberOfGamesPlayed",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NumberOfGamesWon",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Winrate",
                table: "AspNetUsers");
        }
    }
}
