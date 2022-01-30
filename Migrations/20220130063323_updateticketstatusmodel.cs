using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class updateticketstatusmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "TicketStatus");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "TicketStatus",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
