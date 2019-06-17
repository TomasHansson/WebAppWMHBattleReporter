using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebAppWMHBattleReporter.Data.Migrations
{
    public partial class AddBattleReportsTableToDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BattleReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DatePlayed = table.Column<DateTime>(nullable: false),
                    PostersUsername = table.Column<string>(nullable: false),
                    OpponentsUsername = table.Column<string>(nullable: false),
                    ConfirmedByOpponent = table.Column<bool>(nullable: false),
                    ConfirmationKey = table.Column<int>(nullable: false),
                    GameSize = table.Column<int>(nullable: false),
                    Scenario = table.Column<string>(nullable: false),
                    PostersFaction = table.Column<string>(nullable: false),
                    PostersCaster = table.Column<string>(nullable: false),
                    PostersTheme = table.Column<string>(nullable: false),
                    PostersControlPoints = table.Column<int>(nullable: false),
                    PostersArmyPoints = table.Column<int>(nullable: false),
                    PostersArmyList = table.Column<string>(nullable: false),
                    OpponentsFaction = table.Column<string>(nullable: false),
                    OpponentsCaster = table.Column<string>(nullable: false),
                    OpponentsTheme = table.Column<string>(nullable: false),
                    OpponentsControlPoints = table.Column<int>(nullable: false),
                    OpponentsArmyPoints = table.Column<int>(nullable: false),
                    OpponentsArmyList = table.Column<string>(nullable: false),
                    EndCondition = table.Column<string>(nullable: false),
                    WinnersUsername = table.Column<string>(nullable: false),
                    WinningFaction = table.Column<string>(nullable: false),
                    WinningCaster = table.Column<string>(nullable: false),
                    WinningTheme = table.Column<string>(nullable: false),
                    LosersUsername = table.Column<string>(nullable: false),
                    LosingFaction = table.Column<string>(nullable: false),
                    LosingCaster = table.Column<string>(nullable: false),
                    LosingTheme = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleReports", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BattleReports");
        }
    }
}
