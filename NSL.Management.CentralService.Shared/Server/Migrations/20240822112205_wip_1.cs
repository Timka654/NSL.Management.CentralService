using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NSL.Management.CentralService.Shared.Server.Migrations
{
    /// <inheritdoc />
    public partial class wip_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Servers_OwnerId",
                table: "Servers");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_OwnerId",
                table: "Servers",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Servers_OwnerId",
                table: "Servers");

            migrationBuilder.CreateIndex(
                name: "IX_Servers_OwnerId",
                table: "Servers",
                column: "OwnerId",
                unique: true);
        }
    }
}
