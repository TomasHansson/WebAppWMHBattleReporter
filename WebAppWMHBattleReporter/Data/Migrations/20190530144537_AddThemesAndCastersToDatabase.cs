using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppWMHBattleReporter.Data.Migrations
{
    public partial class AddThemesAndCastersToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Casters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    FactionId = table.Column<int>(nullable: false),
                    NumberOfGamesPlayed = table.Column<int>(nullable: false),
                    NumberOfGamesWon = table.Column<int>(nullable: false),
                    NumberOfGamesLost = table.Column<int>(nullable: false),
                    Winrate = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Casters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Casters_Factions_FactionId",
                        column: x => x.FactionId,
                        principalTable: "Factions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    FactionId = table.Column<int>(nullable: false),
                    NumberOfGamesPlayed = table.Column<int>(nullable: false),
                    NumberOfGamesWon = table.Column<int>(nullable: false),
                    NumberOfGamesLost = table.Column<int>(nullable: false),
                    Winrate = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Themes_Factions_FactionId",
                        column: x => x.FactionId,
                        principalTable: "Factions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Casters_FactionId",
                table: "Casters",
                column: "FactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Themes_FactionId",
                table: "Themes",
                column: "FactionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Casters");

            migrationBuilder.DropTable(
                name: "Themes");
        }
    }
}
