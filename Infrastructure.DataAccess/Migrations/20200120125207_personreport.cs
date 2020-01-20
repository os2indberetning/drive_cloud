using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.DataAccess.Migrations
{
    public partial class personreport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persons_Reports_ReportId",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_ReportId",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "Persons");

            migrationBuilder.CreateTable(
                name: "PersonReport",
                columns: table => new
                {
                    PersonId = table.Column<int>(nullable: false),
                    ReportId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonReport", x => new { x.PersonId, x.ReportId });
                    table.ForeignKey(
                        name: "FK_PersonReport_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonReport_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonReport_ReportId",
                table: "PersonReport",
                column: "ReportId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonReport");

            migrationBuilder.AddColumn<int>(
                name: "ReportId",
                table: "Persons",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_ReportId",
                table: "Persons",
                column: "ReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_Reports_ReportId",
                table: "Persons",
                column: "ReportId",
                principalTable: "Reports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
