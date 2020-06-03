using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.DataAccess.Migrations
{
    public partial class OrgIdChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrgOUID",
                table: "OrgUnits");

            migrationBuilder.AlterColumn<string>(
                name: "OrgId",
                table: "OrgUnits",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OrgId",
                table: "OrgUnits",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "OrgOUID",
                table: "OrgUnits",
                nullable: true);
        }
    }
}
