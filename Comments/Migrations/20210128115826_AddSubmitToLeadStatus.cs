using Microsoft.EntityFrameworkCore.Migrations;

namespace Comments.Migrations
{
    public partial class AddSubmitToLeadStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "StatusID", "Name" },
                values: new object[] { 3, "SubmittedToLead" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Status",
                keyColumn: "StatusID",
                keyValue: 3);
        }
    }
}
