using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.DataAccess.Migrations
{
    public partial class unique_appuuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AppUuid",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reports_AppUuid",
                table: "Reports",
                column: "AppUuid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reports_AppUuid",
                table: "Reports");

            migrationBuilder.AlterColumn<string>(
                name: "AppUuid",
                table: "Reports",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
