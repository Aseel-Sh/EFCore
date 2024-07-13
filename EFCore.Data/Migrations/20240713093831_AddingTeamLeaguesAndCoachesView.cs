using Microsoft.EntityFrameworkCore.Migrations;
using System.Runtime.InteropServices;

#nullable disable

namespace EFCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingTeamLeaguesAndCoachesView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW vw_TeamsAndLeagues
                AS
                SELECT t.Name, l.Name AS leagueName
                FROM Teams AS t
                LEFT JOIN Leagues AS l ON t.leagueId = l.Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW vw_TeamsAndLeagues");
        }
    }
}
