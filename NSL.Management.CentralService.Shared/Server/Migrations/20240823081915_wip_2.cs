using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSL.Management.CentralService.Shared.Server.Migrations
{
    /// <inheritdoc />
    public partial class wip_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LogLevel",
                table: "ServerLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogLevel",
                table: "ServerLogs");
        }
    }
}
